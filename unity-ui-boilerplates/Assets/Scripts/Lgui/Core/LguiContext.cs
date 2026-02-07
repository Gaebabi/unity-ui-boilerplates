using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lgui.Core
{
    /// <summary>
    /// UI 요소를 관리하고 재활용(Reconciliation)하는 Lgui 전용 컨텍스트 클래스입니다.
    /// </summary>
    public class LguiContext
    {
        private readonly Transform _root;
        private readonly Stack<Transform> _parentStack = new Stack<Transform>();
        private readonly Dictionary<Transform, int> _siblingCounters = new Dictionary<Transform, int>();
        private readonly HashSet<GameObject> _activeElements = new HashSet<GameObject>();
        private readonly HashSet<Button> _clickedButtons = new HashSet<Button>();
        private readonly HashSet<Button> _consumedClicks = new HashSet<Button>();

        public LguiContext(Transform root)
        {
            _root = root;
        }

        public void Begin()
        {
            _parentStack.Clear();
            _parentStack.Push(_root);
            _siblingCounters.Clear();
            _activeElements.Clear();
            
            _consumedClicks.Clear();
            foreach (var btn in _clickedButtons) _consumedClicks.Add(btn);
            _clickedButtons.Clear();
        }

        public void End()
        {
            CleanupUnused(_root);
        }

        public void PushParent(Transform parent) => _parentStack.Push(parent);
        public void PopParent() => _parentStack.Pop();

        public void RegisterClick(Button btn) => _clickedButtons.Add(btn);
        public bool CheckClick(Button btn) => _consumedClicks.Contains(btn);

        public T GetOrCreateElement<T>(string name) where T : Component
        {
            Transform currentParent = _parentStack.Peek();
            if (!_siblingCounters.ContainsKey(currentParent)) _siblingCounters[currentParent] = 0;
            
            int index = _siblingCounters[currentParent]++;
            string uniqueName = $"{name}_{index}";
            
            Transform child = index < currentParent.childCount ? currentParent.GetChild(index) : null;

            if (child == null || child.name != uniqueName)
            {
                bool isNew = false;
                GameObject go;
                if (child != null && child.name.StartsWith(name))
                {
                    go = child.gameObject;
                }
                else
                {
                    go = new GameObject(uniqueName, typeof(RectTransform));
                    isNew = true;
                }
                
                if (go.name != uniqueName) go.name = uniqueName;
                if (go.transform.parent != currentParent) go.transform.SetParent(currentParent, false);
                if (go.transform.GetSiblingIndex() != index) go.transform.SetSiblingIndex(index);
                child = go.transform;

                if (isNew)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Lgui Create Element");
                    }
#endif
                }
            }

            _activeElements.Add(child.gameObject);
            if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);

            T component = child.GetComponent<T>();
            if (component == null)
            {
                component = child.gameObject.AddComponent<T>();
            }

            return component;
        }

        private void CleanupUnused(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (!_activeElements.Contains(child.gameObject))
                {
                    if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
                }
                else
                {
                    CleanupUnused(child);
                }
            }
        }
    }
}
