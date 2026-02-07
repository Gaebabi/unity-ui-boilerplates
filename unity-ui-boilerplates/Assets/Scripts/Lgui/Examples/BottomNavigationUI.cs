using UnityEngine;
using Lgui.Core;
using Lgui.Components;
using System.Collections.Generic;

namespace Lgui.Examples
{
    public class BottomNavigationUI : LguiBehaviour
    {
        [System.Serializable]
        public class NavItem
        {
            public string label;
            public Sprite icon;
            public Color activeColor = Color.cyan;
        }

        public List<NavItem> items = new List<NavItem>();
        public int selectedIndex = 2;
        
        [Header("Animation")]
        public float animationSpeed = 12f;
        public float hoverLift = 20f;
        
        private float _currentIndicatorPos;
        private float _lastSelectedIndex = -1;

        public BottomNavigationUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Bottom;
            settings.PanelSize = new Vector2(1170, 220); 
            settings.UseSafeArea = false;
            settings.PanelColor = new Color(0, 0, 0, 0); // Transparent background
            settings.DefaultSpacing = 0;
            settings.DefaultPadding = new Lpadding(40, 40, 0, 0); // No bottom padding to stick to floor
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeDefaultItems();
            _currentIndicatorPos = selectedIndex;
        }

        [ContextMenu("Refresh Icons")]
        public void InitializeDefaultItems()
        {
            if (items.Count == 0)
            {
                items.Add(new NavItem { label = "SHOP", activeColor = Color.yellow });
                items.Add(new NavItem { label = "INVEN", activeColor = Color.green });
                items.Add(new NavItem { label = "HOME", activeColor = Color.white });
                items.Add(new NavItem { label = "BATTLE", activeColor = Color.red });
                items.Add(new NavItem { label = "SOCIAL", activeColor = Color.magenta });
            }

            // Always try to load icons if they are null, using robust matching
            foreach (var item in items)
            {
                if (item.icon != null) continue;
                
                string tag = item.label.ToUpper();
                if (tag.Contains("SHOP")) item.icon = Resources.Load<Sprite>("Icons/nav_icon_shop");
                else if (tag.Contains("INVEN")) item.icon = Resources.Load<Sprite>("Icons/nav_icon_inventory");
                else if (tag.Contains("HOME")) item.icon = Resources.Load<Sprite>("Icons/nav_icon_home");
                else if (tag.Contains("BATTLE")) item.icon = Resources.Load<Sprite>("Icons/GemRed");
                else if (tag.Contains("SOCIAL")) item.icon = Resources.Load<Sprite>("Icons/GemBlue");
            }
        }

        protected void Reset()
        {
            InitializeDefaultItems();
        }

        protected override void Update()
        {
            base.Update();
            // Smooth interpolation for the highlight indicator
            if (Mathf.Abs(_currentIndicatorPos - selectedIndex) > 0.001f)
            {
                _currentIndicatorPos = Mathf.Lerp(_currentIndicatorPos, selectedIndex, Time.deltaTime * animationSpeed);
            }
            else
            {
                _currentIndicatorPos = selectedIndex;
            }
        }

        protected override void OnLgui()
        {
            if (items.Count == 0) return;

            // 1. Calculate dimensions
            // Use specialized padding for the button container to keep it within view
            float availableWidth = settings.PanelSize.x - (settings.DefaultPadding.Left + settings.DefaultPadding.Right);
            float itemWidth = availableWidth / items.Count;

            // 2. Render Layers
            // We use a root container to ensure relative positioning of sub-layers
            using (Lgui.Core.Lgui.Container(new Color(0,0,0,0), name: "ContentRoot", padding: new RectOffset(0,0,0,0)))
            {
                DrawButtonLayer(itemWidth);
            }
        }

        private void DrawButtonLayer(float itemWidth)
        {
            using (Lgui.Core.Lgui.Horizontal(spacing: 0, padding: new RectOffset(0,0,0,0), name: "ButtonLayer"))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    DrawNavItem(items[i], i, itemWidth);
                }
            }
        }

        private void DrawNavItem(NavItem item, int index, float width)
        {
            float proximity = Mathf.Clamp01(1.0f - Mathf.Abs(_currentIndicatorPos - index));
            
            bool clicked;
            Lstyle itemStyle = Lstyle.Default.WithSize(width, 180);
            
            using (Lgui.Core.Lgui.ClickableContainer(out clicked, style: itemStyle, name: $"Nav_{item.label}"))
            {
                if (clicked) selectedIndex = index;

                // Dynamic visual style
                Color tint = Color.Lerp(new Color(0.4f, 0.4f, 0.4f), item.activeColor, proximity);
                float iconSize = (70 + (15 * proximity)) * settings.GlobalScale;
                float lift = -hoverLift * proximity; // Lift up when selected

                // Visual Content
                Lgui.Core.Lgui.Space(0, lift); 
                Lgui.Core.Lgui.Image(item.icon, style: Lstyle.Default.WithSize(iconSize, iconSize).WithColor(tint), name: "Icon");
                Lgui.Core.Lgui.Space(0, 12);
                Lgui.Core.Lgui.Text(item.label, style: Lstyle.Default.WithFontSize(Mathf.RoundToInt(24 + (6 * proximity))).WithColor(tint), name: "Label");
            }
        }
    }
}
