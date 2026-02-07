using UnityEngine;
using Lgui.Core;
using Lgui.Components;

namespace Lgui.Examples
{
    public class InventoryUI : LguiBehaviour
    {
        public int columnCount = 4;
        public int itemCount = 12;

        public InventoryUI()
        {
            settings.Position = Lgui.Core.Lgui.PanelPosition.Center;
            settings.PanelSize = new Vector2(1000, 1200);
            settings.PanelColor = new Color(0.08f, 0.08f, 0.1f, 0.98f);
            settings.UseBackgroundBlocker = true;
            settings.BlockerColor = new Color(0, 0, 0, 0.85f);
        }

        protected override void OnLgui()
        {
            // Title Header with Search-like bar placeholder
            using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.MiddleCenter, name: "Header"))
            {
                Lgui.Core.Lgui.Text("INVENTORY", style: Lstyle.Default.WithFontSize(42).WithColor(Color.white));
                Lgui.Core.Lgui.FlexibleSpace();
                using (Lgui.Core.Lgui.Container(new Color(1, 1, 1, 0.1f), padding: new RectOffset(20, 20, 10, 10)))
                {
                    Lgui.Core.Lgui.Text("ALL ITEMS \u25BC", style: Lstyle.Default.WithFontSize(18).WithColor(Color.gray));
                }
            }

            Lgui.Core.Lgui.Space(40);

            // Grid Layout
            int rows = Mathf.CeilToInt((float)itemCount / columnCount);
            float slotSize = 200f;
            float gridSpacing = 20f;

            using (Lgui.Core.Lgui.Vertical(spacing: gridSpacing, name: "GridBody"))
            {
                for (int r = 0; r < rows; r++)
                {
                    using (Lgui.Core.Lgui.Horizontal(spacing: gridSpacing, name: $"Row_{r}"))
                    {
                        for (int c = 0; c < columnCount; c++)
                        {
                            int index = r * columnCount + c;
                            if (index < itemCount)
                            {
                                ItemSlot(index, slotSize);
                            }
                            else
                            {
                                Lgui.Core.Lgui.Space(slotSize); 
                            }
                        }
                    }
                }
            }

            Lgui.Core.Lgui.FlexibleSpace();
            
            // Stats summary
            using (Lgui.Core.Lgui.Horizontal(padding: new RectOffset(20, 20, 20, 20)))
            {
                Lgui.Core.Lgui.Text($"CAPACITY: {itemCount}/100", style: Lstyle.Default.WithFontSize(20).WithColor(Color.gray));
                Lgui.Core.Lgui.FlexibleSpace();
                Lgui.Core.Lgui.Button("SELL ALL", style: Lstyle.Default.WithSize(180, 60).WithColor(new Color(0.4f, 0.2f, 0.2f)));
            }

            Lgui.Core.Lgui.Space(20);

            // Main Buttons
            using (Lgui.Core.Lgui.Horizontal(spacing: 30, alignment: TextAnchor.MiddleCenter, name: "MainActions"))
            {
                Lgui.Core.Lgui.Button("SORT", style: Lstyle.Default.WithSize(450, 90).WithColor(new Color(0.2f, 0.3f, 0.5f)));
                Lgui.Core.Lgui.Button("USE", style: Lstyle.Default.WithSize(450, 90).WithColor(new Color(0.2f, 0.5f, 0.3f)));
            }
        }

        private void ItemSlot(int index, float size)
        {
            using (Lgui.Core.Lgui.Container(new Color(1, 1, 1, 0.05f), padding: new RectOffset(5, 5, 5, 5), name: $"Slot_{index}"))
            {
                // Item Frame
                using (Lgui.Core.Lgui.Vertical(alignment: TextAnchor.MiddleCenter, name: "Content"))
                {
                    Lgui.Core.Lgui.Image(null, style: Lstyle.Default.WithSize(size * 0.75f, size * 0.75f).WithColor(new Color(1, 1, 1, 0.05f)), name: "Icon");
                    
                    // Quantity overlay
                    using (Lgui.Core.Lgui.Horizontal(alignment: TextAnchor.LowerRight))
                    {
                        Lgui.Core.Lgui.FlexibleSpace();
                        Lgui.Core.Lgui.Text($"x{index + 1}", style: Lstyle.Default.WithFontSize(22).WithColor(Color.white), name: "Qty");
                    }
                }
            }
        }
    }
}
