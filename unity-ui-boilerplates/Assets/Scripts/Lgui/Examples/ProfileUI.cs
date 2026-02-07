using UnityEngine;
using Lgui.Core;
using Lgui.Components;

namespace Lgui.Examples
{
    public class ProfileUI : LguiBehaviour
    {
        public Sprite avatarSprite;
        public string userName = "SOLO LEVELING";
        public int level = 99;
        public float expProgress = 0.85f;

        public ProfileUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Center;
            settings.PanelSize = new Vector2(850, 1100);
            settings.PanelColor = new Color(0.08f, 0.08f, 0.12f, 0.98f); // Deep midnight blue
            settings.UseBackgroundBlocker = true;
            settings.BlockerColor = new Color(0, 0, 0, 0.85f);
            settings.DefaultPadding = new Lpadding(50, 50, 50, 50);
        }

        protected override void OnLgui()
        {
            // 1. Header with Close Button placeholder
            using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.UpperLeft))
            {
                using (Lgui.Core.Lgui.Vertical())
                {
                    Lgui.Core.Lgui.Text("PLAYER", style: Lstyle.Default.WithFontSize(24).WithColor(new Color(0.4f, 0.6f, 1f)));
                    Lgui.Core.Lgui.Text("PROFILE", style: Lstyle.Default.WithFontSize(52).WithColor(Color.white));
                }
                Lgui.Core.Lgui.FlexibleSpace();
                if (Lgui.Core.Lgui.Button("X", style: Lstyle.Default.WithSize(70, 70).WithColor(new Color(0.2f, 0.2f, 0.3f))))
                {
                    Debug.Log("Close Profile");
                }
            }

            Lgui.Core.Lgui.Space(60);

            // 2. Avatar & Main Info Section
            using (Lgui.Core.Lgui.Horizontal(spacing: 50, alignment: TextAnchor.MiddleLeft))
            {
                // Glowy Avatar Frame
                using (Lgui.Core.Lgui.Container(new Color(0.15f, 0.25f, 0.5f, 0.3f), padding: new RectOffset(6, 6, 6, 6)))
                {
                    using (Lgui.Core.Lgui.Container(new Color(0.05f, 0.05f, 0.08f, 1f), padding: new RectOffset(10, 10, 10, 10)))
                    {
                        Lgui.Core.Lgui.Image(avatarSprite, style: Lstyle.Default.WithSize(220, 220).WithColor(avatarSprite == null ? new Color(0.2f, 0.2f, 0.3f) : Color.white));
                    }
                }
                
                using (Lgui.Core.Lgui.Vertical(spacing: 20))
                {
                    using (Lgui.Core.Lgui.Container(new Color(0.4f, 0.6f, 1f, 0.15f), padding: new RectOffset(20, 20, 8, 8)))
                    {
                        Lgui.Core.Lgui.Text($"RANK: S-CLASS", style: Lstyle.Default.WithFontSize(22).WithColor(new Color(0.4f, 0.8f, 1f)));
                    }
                    Lgui.Core.Lgui.Text(userName, style: Lstyle.Default.WithFontSize(56).WithColor(Color.white).WithAlignment(TextAnchor.MiddleLeft));
                    
                    using (Lgui.Core.Lgui.Horizontal(spacing: 15))
                    {
                        Lgui.Core.Lgui.Text("LV.", style: Lstyle.Default.WithFontSize(28).WithColor(new Color(0.5f, 0.5f, 0.6f)));
                        Lgui.Core.Lgui.Text(level.ToString(), style: Lstyle.Default.WithFontSize(42).WithColor(new Color(1f, 0.8f, 0.2f)));
                    }
                }
            }

            Lgui.Core.Lgui.Space(60);

            // 3. Experience Section
            using (Lgui.Core.Lgui.Vertical(spacing: 20))
            {
                using (Lgui.Core.Lgui.Horizontal())
                {
                    Lgui.Core.Lgui.Text("EXPERIENCE", style: Lstyle.Default.WithFontSize(22).WithColor(new Color(0.5f, 0.5f, 0.6f)));
                    Lgui.Core.Lgui.FlexibleSpace();
                    Lgui.Core.Lgui.Text($"{Mathf.RoundToInt(expProgress * 100)}%", style: Lstyle.Default.WithFontSize(22).WithColor(Color.white));
                }
                // Custom-looking slider area
                Lgui.Core.Lgui.Slider(expProgress, 0, 1, style: Lstyle.Default.WithSize(750, 16).WithColor(new Color(0.3f, 0.5f, 1f)));
            }

            Lgui.Core.Lgui.Space(80);

            // 4. Stats Grid Section
            Lgui.Core.Lgui.Text("CORE STATISTICS", style: Lstyle.Default.WithFontSize(22).WithColor(new Color(0.5f, 0.5f, 0.6f)));
            Lgui.Core.Lgui.Space(25);
            
            using (Lgui.Core.Lgui.Vertical(spacing: 2, name: "StatsGrid"))
            {
                StatItem("STRENGTH", "2,842", new Color(1f, 0.3f, 0.3f));
                StatItem("AGILITY", "1,955", new Color(0.3f, 1f, 0.4f));
                StatItem("INTELLIGENCE", "3,412", new Color(0.3f, 0.6f, 1f));
                StatItem("VITALITY", "2,110", new Color(1f, 0.6f, 0.2f));
            }

            Lgui.Core.Lgui.FlexibleSpace();

            // 5. Action Footer
            using (Lgui.Core.Lgui.Horizontal(spacing: 30))
            {
                Lgui.Core.Lgui.Button("UPGRADE STATS", style: Lstyle.Default.WithSize(480, 100).WithColor(new Color(0.2f, 0.4f, 0.9f)).WithFontSize(28));
                Lgui.Core.Lgui.Button("LOGOUT", style: Lstyle.Default.WithSize(240, 100).WithColor(new Color(0.2f, 0.2f, 0.25f)).WithFontSize(24));
            }
        }

        private void StatItem(string label, string value, Color accentColor)
        {
            using (Lgui.Core.Lgui.Container(new Color(1,1,1, 0.02f), padding: new RectOffset(30, 30, 25, 25)))
            {
                using (Lgui.Core.Lgui.Horizontal())
                {
                    // Accent indicator
                    using (Lgui.Core.Lgui.Container(accentColor, padding: new RectOffset(4, 4, 15, 15))) { }
                    Lgui.Core.Lgui.Space(30);
                    
                    Lgui.Core.Lgui.Text(label, style: Lstyle.Default.WithFontSize(26).WithColor(new Color(0.7f, 0.7f, 0.8f)));
                    Lgui.Core.Lgui.FlexibleSpace();
                    Lgui.Core.Lgui.Text(value, style: Lstyle.Default.WithFontSize(32).WithColor(Color.white).WithAlignment(TextAnchor.MiddleRight));
                }
            }
        }
    }
    }
}
