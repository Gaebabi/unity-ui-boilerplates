namespace Lgui.Core
{
    /// <summary>
    /// 조립 가능한 UI 컴포넌트의 기본 인터페이스입니다.
    /// </summary>
    public interface ILguiComponent
    {
        /// <summary>
        /// 컴포넌트의 UI를 그립니다. Lgui.Begin()과 End() 사이에서 호출되어야 합니다.
        /// </summary>
        void Draw();
    }
}
