using UnityEngine;
using Lgui.Core;
using Lgui.Components;

namespace Lgui.Examples
{
    /// <summary>
    /// 모바일 게임의 상단 재화 표시 바 예시 구현체입니다.
    /// </summary>
    public class TopCurrencyUI : LguiBehaviour
    {
        [Header("Icons")]
        public Sprite coinIcon;
        public Sprite diamondIcon;
        public Sprite energyIcon;

        [Header("Data")]
        public int coins = 1000;
        public int diamonds = 50;
        public int energy = 10;
        
        public TopCurrencyUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Top;
            settings.PanelSize = new Vector2(1170, 120);
            settings.UseSafeArea = true;
            settings.UseBackgroundBlocker = false;
            settings.PanelColor = new Color(0, 0, 0, 0); // Transparent background for bar
        }

        protected override void Awake()
        {
            base.Awake();
            LoadIcons();
        }

        private void Reset()
        {
            LoadIcons();
        }

        private void LoadIcons()
        {
            if (coinIcon == null) coinIcon = Resources.Load<Sprite>("Icons/Gold");
            if (diamondIcon == null) diamondIcon = Resources.Load<Sprite>("Icons/GemRed");
            if (energyIcon == null) energyIcon = Resources.Load<Sprite>("Icons/GemBlue");
            
            if (coinIcon == null || diamondIcon == null || energyIcon == null)
            {
                // Try alternate path just in case
                if (coinIcon == null) Debug.LogWarning("LGUI: Failed to load Gold icon. Ensure it is in Resources/Icons/Gold and marked as Sprite.");
            }
        }

        protected override void OnLgui()
        {
            if (coinIcon == null) LoadIcons();

            // 상단 바 전체 영역
            using (Lgui.Core.Lgui.Horizontal(spacing: 25, padding: new RectOffset(20, 20, 10, 10), alignment: TextAnchor.UpperLeft, name: "TopBar"))
            {
                // 왼쪽 부분을 FlexibleSpace로 밀어서 우측 정렬 효과를 냅니다.
                Lgui.Core.Lgui.FlexibleSpace();

                CurrencyItem(energyIcon, energy.ToString(), Color.red, "Energy");
                CurrencyItem(diamondIcon, diamonds.ToString(), Color.cyan, "Diamonds");
                CurrencyItem(coinIcon, coins.ToString(), Color.yellow, "Coins");
            }
        }

        private void CurrencyItem(Sprite icon, string value, Color bgColor, string name)
        {
            // 각 재화 항목의 배경 컨테이너
            using (Lgui.Core.Lgui.Container(new Color(0, 0, 0, 0.6f), name: $"{name}_Bg"))
            {
                using (Lgui.Core.Lgui.Horizontal(spacing: 12, padding: new RectOffset(12, 12, 8, 8), alignment: TextAnchor.MiddleCenter, name: $"{name}_Layout"))
                {
                    Lgui.Core.Lgui.Image(icon, style: Lstyle.Default.WithSize(48, 48), name: "Icon");
                    Lgui.Core.Lgui.Text(value, style: Lstyle.Default.WithFontSize(28), name: "Value");
                    
                    if (Lgui.Core.Lgui.Button("+", name: "AddButton"))
                    {
                        Debug.Log($"{name} add clicked!");
                        if (name == "Coins") coins += 100;
                    }
                }
            }
        }
    }
}
