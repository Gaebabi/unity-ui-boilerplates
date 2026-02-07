using UnityEngine;
using Lgui.Core;
using Lgui.Components;

namespace Lgui.Examples
{
    public class DialogUI : LguiBehaviour
    {
        public string title = "Confirm Action";
        public string message = "Are you sure you want to delete this character? This action cannot be undone.";
        public bool isOpen = true;

        public DialogUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Center;
            settings.PanelSize = new Vector2(900, 500);
            settings.PanelColor = new Color(0.12f, 0.12f, 0.15f, 0.98f);
            settings.UseBackgroundBlocker = true;
            settings.BlockerColor = new Color(0, 0, 0, 0.85f);
        }

        protected override void OnLgui()
        {
            if (!isOpen)
            {
                return;
            }

            // Title with highlight
            using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.MiddleCenter))
            {
                Lgui.Core.Lgui.Text(title.ToUpper(), style: Lstyle.Default.WithFontSize(38).WithColor(Color.white));
            }

            Lgui.Core.Lgui.Space(30);

            // Message area
            using (Lgui.Core.Lgui.Container(new Color(1, 1, 1, 0.05f), padding: new RectOffset(40, 40, 30, 30)))
            {
                Lgui.Core.Lgui.Text(message, style: Lstyle.Default.WithFontSize(26).WithColor(new Color(0.9f, 0.9f, 0.9f)).WithAlignment(TextAnchor.MiddleCenter).WithSize(750, 150));
            }

            Lgui.Core.Lgui.FlexibleSpace();

            // Action Buttons
            using (Lgui.Core.Lgui.Horizontal(spacing: 40, alignment: TextAnchor.MiddleCenter))
            {
                if (Lgui.Core.Lgui.Button("CANCEL", style: Lstyle.Default.WithSize(380, 90).WithColor(new Color(0.25f, 0.25f, 0.3f))))
                {
                    isOpen = false;
                }
                if (Lgui.Core.Lgui.Button("CONFIRM", style: Lstyle.Default.WithSize(380, 90).WithColor(new Color(0.15f, 0.5f, 0.8f))))
                {
                    Debug.Log("Action Confirmed!");
                    isOpen = false;
                }
            }

            Lgui.Core.Lgui.Space(20);
        }
    }
}
