using UnityEngine;
using Lgui.Core;
using Lgui.Components;

namespace Lgui.Examples
{
    public class SettingsUI : LguiBehaviour
    {
        [Header("Audio Settings")]
        public float masterVolume = 0.8f;
        public float musicVolume = 0.6f;
        public float sfxVolume = 0.9f;

        [Header("Preferences")]
        public bool notificationsEnabled = true;
        public bool cloudSync = false;
        public int graphicQuality = 2;

        public SettingsUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Center;
            settings.PanelSize = new Vector2(900, 1100);
            settings.PanelColor = new Color(0.1f, 0.1f, 0.12f, 0.98f);
            settings.UseBackgroundBlocker = true;
            settings.BlockerColor = new Color(0, 0, 0, 0.8f);
        }

        protected override void OnLgui()
        {
            // Header
            using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.MiddleCenter))
            {
                Lgui.Core.Lgui.Text("SYSTEM SETTINGS", style: Lstyle.Default.WithFontSize(42).WithColor(Color.white));
            }

            Lgui.Core.Lgui.Space(60);

            // Audio Section
            SectionHeader("AUDIO & VOLUME");
            using (Lgui.Core.Lgui.Container(new Color(1, 1, 1, 0.03f), padding: new RectOffset(40, 40, 40, 40)))
            {
                masterVolume = SliderItem("Master Volume", masterVolume);
                Lgui.Core.Lgui.Space(30);
                musicVolume = SliderItem("Music & Ambience", musicVolume);
                Lgui.Core.Lgui.Space(30);
                sfxVolume = SliderItem("Sound Effects", sfxVolume);
            }

            Lgui.Core.Lgui.Space(60);

            // Preferences Section
            SectionHeader("PREFERENCES");
            using (Lgui.Core.Lgui.Container(new Color(1, 1, 1, 0.03f), padding: new RectOffset(40, 40, 40, 40)))
            {
                notificationsEnabled = ToggleItem("Push Notifications", notificationsEnabled);
                Lgui.Core.Lgui.Space(30);
                cloudSync = ToggleItem("Cloud Data Sync", cloudSync);
            }

            Lgui.Core.Lgui.FlexibleSpace();

            // Footer Buttons
            using (Lgui.Core.Lgui.Horizontal(spacing: 40, alignment: TextAnchor.MiddleCenter))
            {
                Lgui.Core.Lgui.Button("BACK", style: Lstyle.Default.WithSize(380, 100).WithColor(new Color(0.2f, 0.2f, 0.25f)));
                Lgui.Core.Lgui.Button("APPLY", style: Lstyle.Default.WithSize(380, 100).WithColor(new Color(0.2f, 0.6f, 0.3f)));
            }
        }

        private void SectionHeader(string title)
        {
            using (Lgui.Core.Lgui.Horizontal())
            {
                Lgui.Core.Lgui.Text(title, style: Lstyle.Default.WithFontSize(22).WithColor(new Color(0.6f, 0.6f, 0.7f)).WithAlignment(TextAnchor.MiddleLeft));
            }
            Lgui.Core.Lgui.Space(20);
        }

        private float SliderItem(string label, float value)
        {
            using (Lgui.Core.Lgui.Vertical(spacing: 15))
            {
                using (Lgui.Core.Lgui.Horizontal())
                {
                    Lgui.Core.Lgui.Text(label, style: Lstyle.Default.WithFontSize(26).WithColor(Color.white).WithAlignment(TextAnchor.MiddleLeft));
                    Lgui.Core.Lgui.FlexibleSpace();
                    Lgui.Core.Lgui.Text($"{Mathf.RoundToInt(value * 100)}%", style: Lstyle.Default.WithFontSize(22).WithColor(Color.cyan));
                }
                return Lgui.Core.Lgui.Slider(value, 0, 1, style: Lstyle.Default.WithSize(700, 30), name: label);
            }
        }

        private bool ToggleItem(string label, bool value)
        {
            using (Lgui.Core.Lgui.Horizontal())
            {
                Lgui.Core.Lgui.Text(label, style: Lstyle.Default.WithFontSize(26).WithColor(Color.white).WithAlignment(TextAnchor.MiddleLeft));
                Lgui.Core.Lgui.FlexibleSpace();
                return Lgui.Core.Lgui.Toggle(value, style: Lstyle.Default.WithFontSize(22), name: label);
            }
        }

        private void ResetSettings()
        {
            masterVolume = 0.8f;
            musicVolume = 0.5f;
            sfxVolume = 0.5f;
            notificationsEnabled = true;
            cloudSync = false;
        }
    }
}
