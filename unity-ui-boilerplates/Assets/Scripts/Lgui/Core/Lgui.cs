using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lgui.Core
{
    /// <summary>
    /// Lgui (L-Gooey)는 IMGUI 스타일의 선언적 문법으로 uGUI를 제어하는 코어 정적 클래스입니다.
    /// </summary>
    public static class Lgui
    {
        private static LguiContext _context;
        
        public static GlobalSettings Settings = new GlobalSettings();

        public enum PanelPosition { Center, Top, Bottom, Left, Right, Custom }

        [Serializable]
        public class GlobalSettings
        {
            [Range(0.1f, 5f)]
            public float GlobalScale = 1.0f;
            public float DefaultSpacing = 20f;
            public Lpadding DefaultPadding = new Lpadding(10, 10, 10, 10);
            public int DefaultFontSize = 24;

            [Header("Layout Structure")]
            public bool UseBackgroundBlocker = true;
            public Color BlockerColor = new Color(0, 0, 0, 0.5f);
            public PanelPosition Position = PanelPosition.Center;
            public Vector2 PanelSize = new Vector2(600, 400);
            public bool UseSafeArea = true;
            public Vector2 CustomAnchorPosition = Vector2.zero;
            public Color PanelColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        }

        /// <summary>
        /// UI 렌더링 프레임을 시작합니다.
        /// </summary>
        public static void Begin(LguiContext context)
        {
            _context = context;
            _context.Begin();
        }

        /// <summary>
        /// UI 렌더링 프레임을 종료하고 사용되지 않는 요소를 정리합니다.
        /// </summary>
        public static void End()
        {
            _context.End();
            _context = null;
        }

        /// <summary>
        /// 텍스트 요소를 렌더링합니다.
        /// </summary>
        public static void Text(string content, Lstyle? style = null, string name = "Text")
        {
            var s = style ?? Lstyle.Default;
            var text = _context.GetOrCreateElement<Text>(name);
            text.text = content;
            text.color = s.Color ?? Color.white;
            text.fontSize = Mathf.RoundToInt((s.FontSize ?? Settings.DefaultFontSize) * Settings.GlobalScale);
            text.alignment = s.Alignment ?? TextAnchor.MiddleCenter;
            text.raycastTarget = false;

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            var element = text.GetComponent<LayoutElement>();
            if (element == null) element = text.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = s.Size.HasValue ? s.Size.Value.x * Settings.GlobalScale : -1;
            element.preferredHeight = s.Size.HasValue ? s.Size.Value.y * Settings.GlobalScale : 40 * Settings.GlobalScale;
        }

        /// <summary>
        /// 이미지 요소를 렌더링합니다.
        /// </summary>
        public static void Image(Sprite sprite, Lstyle? style = null, string name = "Image")
        {
            var s = style ?? Lstyle.Default;
            var img = _context.GetOrCreateElement<Image>(name);
            img.sprite = sprite;
            img.color = s.Color ?? Color.white;
            img.preserveAspect = true;
            img.raycastTarget = false;

            var element = img.GetComponent<LayoutElement>();
            if (element == null) element = img.gameObject.AddComponent<LayoutElement>();
            if (s.Size.HasValue)
            {
                element.preferredWidth = s.Size.Value.x * Settings.GlobalScale;
                element.preferredHeight = s.Size.Value.y * Settings.GlobalScale;
            }
        }

        /// <summary>
        /// 버튼을 렌더링하고 클릭 여부를 반환합니다.
        /// </summary>
        public static bool Button(string label, Action onClick = null, Lstyle? style = null, string name = "Button")
        {
            var s = style ?? Lstyle.Default;
            var btn = _context.GetOrCreateElement<Button>(name);
            var img = btn.GetComponent<Image>();
            if (img == null) img = btn.gameObject.AddComponent<Image>();
            
            Color baseColor = s.Color ?? Color.white;
            img.color = Color.white; // Use white image to allow pure color tinting
            img.raycastTarget = true;

            // Setup Transition Visuals
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseColor * 1.1f;
            colors.pressedColor = baseColor * 0.8f;
            colors.selectedColor = baseColor; 
            btn.colors = colors;
            
            // Disable navigation to avoid persistent selection highlight
            var nav = btn.navigation;
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            var element = btn.GetComponent<LayoutElement>();
            if (element == null) element = btn.gameObject.AddComponent<LayoutElement>();
            if (s.Size.HasValue)
            {
                element.preferredWidth = s.Size.Value.x * Settings.GlobalScale;
                element.preferredHeight = s.Size.Value.y * Settings.GlobalScale;
            }

            // Push current button as parent for its internal layout
            _context.PushParent(btn.transform);
            
            using (Horizontal(name: "BtnLayout", padding: new RectOffset(
                Mathf.RoundToInt(10 * Settings.GlobalScale), 
                Mathf.RoundToInt(10 * Settings.GlobalScale), 
                Mathf.RoundToInt(5 * Settings.GlobalScale), 
                Mathf.RoundToInt(5 * Settings.GlobalScale)),
                alignment: TextAnchor.MiddleCenter))
            {
                Text(label, style: new Lstyle() { FontSize = s.FontSize }, name: "Label");
            }

            // Pop button parent
            _context.PopParent();

            bool clicked = _context.CheckClick(btn);
            var capturedContext = _context; // Capture context for the closure
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                capturedContext.RegisterClick(btn);
                onClick?.Invoke();
            });

            return clicked;
        }

        /// <summary>
        /// 레이아웃 엔진 없이 자유로운 배치를 위한 빈 박스를 생성합니다.
        /// </summary>
        public static LayoutScope Box(string name = "Box")
        {
            var go = _context.GetOrCreateElement<RectTransform>(name);
            // Ensure no layout groups are on this object
            var vGroup = go.GetComponent<VerticalLayoutGroup>();
            if (vGroup != null) UnityEngine.Object.DestroyImmediate(vGroup);
            var hGroup = go.GetComponent<HorizontalLayoutGroup>();
            if (hGroup != null) UnityEngine.Object.DestroyImmediate(hGroup);
            var fitter = go.GetComponent<ContentSizeFitter>();
            if (fitter != null) UnityEngine.Object.DestroyImmediate(fitter);

            _context.PushParent(go.transform);
            return new LayoutScope(_context);
        }

        /// <summary>
        /// 슬라이더 요소를 렌더링하고 현재 값을 반환합니다.
        /// </summary>
        public static float Slider(float value, float min, float max, Lstyle? style = null, string name = "Slider")
        {
            var s = style ?? Lstyle.Default;
            var slider = _context.GetOrCreateElement<Slider>(name);
            
            // UI structure for Slider if not exists
            if (slider.transform.childCount == 0)
            {
                using (Horizontal(name: "Background", padding: new RectOffset(0,0,0,0)))
                {
                    var bg = _context.GetOrCreateElement<Image>("BgImg");
                    bg.color = new Color(0.2f, 0.2f, 0.2f);
                    
                    // Hidden fill area for UGUI slider logic
                    var fillArea = new GameObject("Fill Area", typeof(RectTransform)).transform;
                    fillArea.SetParent(slider.transform, false);
                    var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                    fill.transform.SetParent(fillArea, false);
                    fill.color = s.Color ?? Color.green;
                    slider.fillRect = fill.rectTransform;

                    var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform)).transform;
                    handleArea.SetParent(slider.transform, false);
                    var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                    handle.transform.SetParent(handleArea, false);
                    handle.color = Color.white;
                    slider.handleRect = handle.rectTransform;
                }
            }

            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
            
            // Interaction setup
            slider.transition = Selectable.Transition.ColorTint;
            var sNav = slider.navigation;
            sNav.mode = Navigation.Mode.None;
            slider.navigation = sNav;

            var element = slider.GetComponent<LayoutElement>();
            if (element == null) element = slider.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = (s.Size?.x ?? 200) * Settings.GlobalScale;
            element.preferredHeight = (s.Size?.y ?? 20) * Settings.GlobalScale;

            return slider.value;
        }

        /// <summary>
        /// 토글 요소를 렌더링하고 체크 여부를 반환합니다.
        /// </summary>
        public static bool Toggle(bool value, string label = "", Lstyle? style = null, string name = "Toggle")
        {
            var s = style ?? Lstyle.Default;
            var toggle = _context.GetOrCreateElement<Toggle>(name);
            
            if (toggle.transform.childCount == 0)
            {
                using (Horizontal(spacing: 10 * Settings.GlobalScale, name: "ToggleLayout"))
                {
                    var bg = _context.GetOrCreateElement<Image>("Background");
                    bg.color = new Color(0.1f, 0.1f, 0.1f);
                    bg.raycastTarget = true;
                    
                    var check = new GameObject("Checkmark", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                    check.transform.SetParent(bg.transform, false);
                    check.color = Color.cyan;
                    toggle.graphic = check;
                    toggle.targetGraphic = bg;
                    
                    if (!string.IsNullOrEmpty(label)) Text(label, style: s, name: "Label");
                }
            }

            // Interaction setup
            toggle.transition = Selectable.Transition.ColorTint;
            var tNav = toggle.navigation;
            tNav.mode = Navigation.Mode.None;
            toggle.navigation = tNav;

            toggle.isOn = value;
            return toggle.isOn;
        }

        /// <summary>
        /// 수평 레이아웃 그룹을 시작합니다.
        /// </summary>
        public static LayoutScope Horizontal(float? spacing = null, RectOffset padding = null, TextAnchor alignment = TextAnchor.UpperLeft, string name = "Horizontal")
        {
            var group = _context.GetOrCreateElement<HorizontalLayoutGroup>(name);
            group.spacing = (spacing ?? Settings.DefaultSpacing) * Settings.GlobalScale;
            
            var p = padding ?? Settings.DefaultPadding.ToRectOffset();
            group.padding = new RectOffset(
                Mathf.RoundToInt(p.left * Settings.GlobalScale),
                Mathf.RoundToInt(p.right * Settings.GlobalScale),
                Mathf.RoundToInt(p.top * Settings.GlobalScale),
                Mathf.RoundToInt(p.bottom * Settings.GlobalScale)
            );
            group.childAlignment = alignment;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            var fitter = group.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = group.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _context.PushParent(group.transform);
            return new LayoutScope(_context);
        }

        /// <summary>
        /// 수직 레이아웃 그룹을 시작합니다.
        /// </summary>
        public static LayoutScope Vertical(float? spacing = null, RectOffset padding = null, TextAnchor alignment = TextAnchor.UpperLeft, string name = "Vertical")
        {
            var group = _context.GetOrCreateElement<VerticalLayoutGroup>(name);
            group.spacing = (spacing ?? Settings.DefaultSpacing) * Settings.GlobalScale;

            var p = padding ?? Settings.DefaultPadding.ToRectOffset();
            group.padding = new RectOffset(
                Mathf.RoundToInt(p.left * Settings.GlobalScale),
                Mathf.RoundToInt(p.right * Settings.GlobalScale),
                Mathf.RoundToInt(p.top * Settings.GlobalScale),
                Mathf.RoundToInt(p.bottom * Settings.GlobalScale)
            );
            group.childAlignment = alignment;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            var fitter = group.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = group.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _context.PushParent(group.transform);
            return new LayoutScope(_context);
        }

        /// <summary>
        /// 배경 박스(컨테이너)를 시작합니다.
        /// </summary>
        public static LayoutScope Container(Color color, RectOffset padding = null, float? spacing = null, string name = "Container")
        {
            var img = _context.GetOrCreateElement<Image>(name);
            img.color = color;

            var group = img.GetComponent<VerticalLayoutGroup>();
            if (group == null) group = img.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing = (spacing ?? Settings.DefaultSpacing) * Settings.GlobalScale;
            
            var p = padding ?? Settings.DefaultPadding.ToRectOffset();
            group.padding = new RectOffset(
                Mathf.RoundToInt(p.left * Settings.GlobalScale),
                Mathf.RoundToInt(p.right * Settings.GlobalScale),
                Mathf.RoundToInt(p.top * Settings.GlobalScale),
                Mathf.RoundToInt(p.bottom * Settings.GlobalScale)
            );
            group.childAlignment = TextAnchor.UpperLeft;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            var fitter = img.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = img.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _context.PushParent(img.transform);
            return new LayoutScope(_context);
        }

        /// <summary>
        /// 화면 전체를 덮는 클릭 차단 배경을 생성합니다.
        /// </summary>
        public static void FullScreenBlocker(Color color)
        {
            var img = _context.GetOrCreateElement<Image>("BackgroundBlocker");
            img.color = color;
            img.raycastTarget = true;
            var rect = img.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        public static LayoutScope ClickableContainer(out bool clicked, Lstyle? style = null, RectOffset padding = null, float? spacing = null, string name = "ClickableContainer")
        {
            var s = style ?? Lstyle.Default;
            var btn = _context.GetOrCreateElement<Button>(name);
            var img = btn.GetComponent<Image>();
            if (img == null) img = btn.gameObject.AddComponent<Image>();
            img.color = s.Color ?? new Color(0, 0, 0, 0); // Default transparent
            img.raycastTarget = true;

            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = img;
            var nav = btn.navigation;
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            var group = btn.GetComponent<VerticalLayoutGroup>();
            if (group == null) group = btn.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing = (spacing ?? Settings.DefaultSpacing) * Settings.GlobalScale;
            
            var p = padding ?? Settings.DefaultPadding.ToRectOffset();
            group.padding = new RectOffset(
                Mathf.RoundToInt(p.left * Settings.GlobalScale),
                Mathf.RoundToInt(p.right * Settings.GlobalScale),
                Mathf.RoundToInt(p.top * Settings.GlobalScale),
                Mathf.RoundToInt(p.bottom * Settings.GlobalScale)
            );
            group.childAlignment = TextAnchor.MiddleCenter;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            var element = btn.GetComponent<LayoutElement>();
            if (element == null) element = btn.gameObject.AddComponent<LayoutElement>();
            if (s.Size.HasValue)
            {
                element.preferredWidth = s.Size.Value.x * Settings.GlobalScale;
                element.preferredHeight = s.Size.Value.y * Settings.GlobalScale;
            }

            clicked = _context.CheckClick(btn);
            var capturedContext = _context; // Capture context for the closure
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => capturedContext.RegisterClick(btn));

            _context.PushParent(btn.transform);
            return new LayoutScope(_context);
        }
        public static LayoutScope MainPanel(PanelPosition pos, Vector2 size, Vector2 customPos, Color color)
        {
            var img = _context.GetOrCreateElement<Image>("MainPanel");
            img.color = color;
            var rect = img.rectTransform;

            switch (pos)
            {
                case PanelPosition.Center:
                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case PanelPosition.Top:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    break;
                case PanelPosition.Bottom:
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    break;
                case PanelPosition.Left:
                    rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
                    rect.pivot = new Vector2(0f, 0.5f);
                    break;
                case PanelPosition.Right:
                    rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
                    rect.pivot = new Vector2(1f, 0.5f);
                    break;
                case PanelPosition.Custom:
                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }

            // 1. Calculate final position (Custom Offset * Global Scale)
            Vector2 finalPos = customPos * Settings.GlobalScale;
            Vector2 scaledSize = size * Settings.GlobalScale;
            if (rect.sizeDelta != scaledSize) rect.sizeDelta = scaledSize;

            // 2. Apply Safe Area offset if enabled
            if (Settings.UseSafeArea)
            {
                var canvas = img.canvas;
                if (canvas != null)
                {
                    var scaler = canvas.GetComponent<CanvasScaler>();
                    if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    {
                        float screenHeight = Screen.height;
                        if (screenHeight > 0)
                        {
                            float factor = scaler.referenceResolution.y / screenHeight;
                            if (pos == PanelPosition.Top)
                            {
                                float safeTopPixel = screenHeight - Screen.safeArea.yMax;
                                if (safeTopPixel > 0) finalPos += new Vector2(0, -safeTopPixel * factor);
                            }
                            else if (pos == PanelPosition.Bottom)
                            {
                                float safeBottomPixel = Screen.safeArea.yMin;
                                if (safeBottomPixel > 0) finalPos += new Vector2(0, safeBottomPixel * factor);
                            }
                        }
                    }
                }
            }

            // 3. Set the position exactly once to avoid conflicts
            rect.anchoredPosition = finalPos;

            var group = img.GetComponent<VerticalLayoutGroup>();
            if (group == null) group = img.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing = Settings.DefaultSpacing;
            group.padding = Settings.DefaultPadding.ToRectOffset();
            group.childAlignment = TextAnchor.UpperCenter;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            _context.PushParent(img.transform);
            return new LayoutScope(_context);
        }

        public static void Space(float size) => Space(size, size);

        public static void Space(float width, float height)
        {
            var go = _context.GetOrCreateElement<LayoutElement>("Space");
            var element = go.GetComponent<LayoutElement>();
            element.minWidth = width * Settings.GlobalScale;
            element.minHeight = height * Settings.GlobalScale;
            element.preferredWidth = width * Settings.GlobalScale;
            element.preferredHeight = height * Settings.GlobalScale;
        }

        public static void FlexibleSpace()
        {
            var go = _context.GetOrCreateElement<LayoutElement>("FlexSpace");
            var element = go.GetComponent<LayoutElement>();
            element.flexibleWidth = 10000;
            element.flexibleHeight = 10000;
        }

        /// <summary>
        /// 레이아웃 계층을 관리하는 구조체입니다. Using 구문과 함께 사용되어 GC를 발생시키지 않습니다.
        /// </summary>
        public struct LayoutScope : IDisposable
        {
            private readonly LguiContext _context;
            public LayoutScope(LguiContext context) => _context = context;
            public void Dispose() => _context?.PopParent();
        }
    }
}
