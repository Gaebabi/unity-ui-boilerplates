using UnityEngine;
using Lgui.Core;

namespace Lgui.Components
{
    /// <summary>
    /// Lgui 시스템을 사용하는 모든 UI 컴포넌트의 베이스 클래스입니다.
    /// MonoBehaviour를 상속받아 유니티 컴포넌트로 동작합니다.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public abstract class LguiBehaviour : MonoBehaviour
    {
        public Lgui.Core.Lgui.GlobalSettings settings = new Lgui.Core.Lgui.GlobalSettings();
        private LguiContext _context;

        protected virtual void Awake()
        {
            _context = new LguiContext(this.transform);
        }

        protected virtual void OnValidate()
        {
            // 인스펙터 값이 변경될 때 Render를 예약합니다. 
            // 단, Update가 이미 매 프레임 Render를 호출하므로 여기서는 플래그만 설정하거나 생략 가능합니다.
            // 여기서는 수동 갱신을 위해 최소한의 보강만 남깁니다.
        }

        protected virtual void Update()
        {
            // [ExecuteAlways]에 의해 에디터와 플레이 모드 모두에서 실행됩니다.
            Render();
        }

        /// <summary>
        /// UI를 즉시 다시 그립니다.
        /// </summary>
        [ContextMenu("Refresh UI")]
        public void Render()
        {
            if (_context == null) _context = new LguiContext(this.transform);
            
            Lgui.Core.Lgui.Settings = settings;
            Lgui.Core.Lgui.Begin(_context);
            
            // Draw background blocker if enabled
            if (settings.UseBackgroundBlocker)
            {
                Lgui.Core.Lgui.FullScreenBlocker(settings.BlockerColor);
            }

            // Wrap OnLgui in the Main Panel structure
            using (Lgui.Core.Lgui.MainPanel(settings.Position, settings.PanelSize, settings.CustomAnchorPosition, settings.PanelColor))
            {
                OnLgui();
            }

            Lgui.Core.Lgui.End();
        }

        /// <summary>
        /// IMGUI 스타일의 UI 정의가 일어나는 추상 메서드입니다.
        /// </summary>
        protected abstract void OnLgui();
    }
}
