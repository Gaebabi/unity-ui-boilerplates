using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lgui.Examples;
using Lgui.Components;

namespace Lgui.Editor
{
    public static class LguiMenuItems
    {
        [MenuItem("GameObject/UI/Lgui/Showcase Gallery", false, 10)]
        public static void CreateShowcase(MenuCommand menuCommand) => CreateLguiComponent<LguiShowcase>("Lgui Showcase", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Top Currency Bar", false, 20)]
        public static void CreateTopCurrency(MenuCommand menuCommand) => CreateLguiComponent<TopCurrencyUI>("Top Currency UI", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Profile", false, 21)]
        public static void CreateProfile(MenuCommand menuCommand) => CreateLguiComponent<ProfileUI>("Profile UI", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Inventory", false, 22)]
        public static void CreateInventory(MenuCommand menuCommand) => CreateLguiComponent<InventoryUI>("Inventory UI", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Settings", false, 23)]
        public static void CreateSettings(MenuCommand menuCommand) => CreateLguiComponent<SettingsUI>("Settings UI", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Dialog", false, 24)]
        public static void CreateDialog(MenuCommand menuCommand) => CreateLguiComponent<DialogUI>("Dialog UI", menuCommand);

        [MenuItem("GameObject/UI/Lgui/Examples/Bottom Nav", false, 25)]
        public static void CreateBottomNav(MenuCommand menuCommand) => CreateLguiComponent<BottomNavigationUI>("Bottom Nav UI", menuCommand);

        private static void CreateLguiComponent<T>(string name, MenuCommand menuCommand) where T : LguiBehaviour
        {
            // Create root object
            GameObject go = new GameObject($"[Lgui] {name}");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            // Add Canvas and setup
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<GraphicRaycaster>();

            // Configure Canvas Scaler as requested
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170, 2532);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1.0f; // Match Height

            // Add the Lgui component
            go.AddComponent<T>();

            // Ensure EventSystem exists
            CreateEventSystem();

            // Register Undo and Select
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
            }
        }
    }
}
