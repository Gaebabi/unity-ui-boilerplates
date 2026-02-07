using UnityEngine;
using Lgui.Core;
using Lgui.Components;
using System.Collections.Generic;

namespace Lgui.Examples
{
    public class LguiShowcase : LguiBehaviour
    {
        private List<System.Type> _exampleTypes = new List<System.Type>();
        private int _selectedIndex = 0;
        private Dictionary<System.Type, LguiBehaviour> _instances = new Dictionary<System.Type, LguiBehaviour>();
        
        public LguiShowcase()
        {
            settings.PanelSize = new Vector2(1000, 700);
            settings.UseBackgroundBlocker = false;
            settings.PanelColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        protected override void Awake()
        {
            base.Awake();
            RefreshExampleList();
        }

        private void RefreshExampleList()
        {
            _exampleTypes.Clear();
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(LguiBehaviour)) && !type.IsAbstract && type != typeof(LguiShowcase))
                    {
                        _exampleTypes.Add(type);
                    }
                }
            }
        }

        protected override void OnLgui()
        {
            if (_exampleTypes.Count == 0) RefreshExampleList();

            using (Lgui.Core.Lgui.Horizontal(spacing: 0, padding: new RectOffset(0, 0, 0, 0), name: "ShowcaseRoot"))
            {
                // SIDEBAR
                using (Lgui.Core.Lgui.Container(new Color(0.12f, 0.12f, 0.15f), padding: new RectOffset(15, 15, 30, 15), spacing: 10, name: "Sidebar"))
                {
                    Lgui.Core.Lgui.Text("LGUI GALLERY", style: Lstyle.Default.WithFontSize(24).WithColor(Color.cyan).WithAlignment(TextAnchor.MiddleLeft));
                    Lgui.Core.Lgui.Space(20);

                    for (int i = 0; i < _exampleTypes.Count; i++)
                    {
                        string typeName = _exampleTypes[i].Name;
                        bool isSelected = _selectedIndex == i;
                        Color btnColor = isSelected ? new Color(0.2f, 0.5f, 1f) : new Color(0.2f, 0.2f, 0.2f);

                        if (Lgui.Core.Lgui.Button(typeName, style: Lstyle.Default.WithSize(220, 40).WithColor(btnColor)))
                        {
                            _selectedIndex = i;
                        }
                    }

                    Lgui.Core.Lgui.FlexibleSpace();
                    if (Lgui.Core.Lgui.Button("Refresh List", style: Lstyle.Default.WithSize(220, 35).WithColor(new Color(0.3f,0.3f,0.3f))))
                    {
                        RefreshExampleList();
                    }
                }

                // MAIN CONTENT
                using (Lgui.Core.Lgui.Container(new Color(0.08f, 0.08f, 0.08f), padding: new RectOffset(40, 40, 40, 40), name: "MainArea"))
                {
                    if (_exampleTypes.Count > 0 && _selectedIndex < _exampleTypes.Count)
                    {
                        var targetType = _exampleTypes[_selectedIndex];
                        Lgui.Core.Lgui.Text(targetType.Name, style: Lstyle.Default.WithFontSize(32).WithColor(new Color(1,1,1,0.5f)).WithAlignment(TextAnchor.MiddleLeft));
                        Lgui.Core.Lgui.Space(20);

                        using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.MiddleCenter))
                        {
                            Lgui.Core.Lgui.FlexibleSpace();
                            RenderExample(targetType);
                            Lgui.Core.Lgui.FlexibleSpace();
                        }
                    }
                    Lgui.Core.Lgui.FlexibleSpace();
                }
            }
        }

        private void RenderExample(System.Type type)
        {
            if (!_instances.TryGetValue(type, out var instance) || instance == null)
            {
                var go = new GameObject($"__Preview_{type.Name}");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.SetParent(this.transform);
                instance = (LguiBehaviour)go.AddComponent(type);
                _instances[type] = instance;
            }

            // Manually call OnLgui using reflection
            var method = type.GetMethod("OnLgui", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(instance, null);
        }

        private void OnDestroy()
        {
            foreach (var inst in _instances.Values)
            {
                if (inst != null) DestroyImmediate(inst.gameObject);
            }
            _instances.Clear();
        }
    }
}
