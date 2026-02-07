using System;
using UnityEngine;

namespace Lgui.Core
{
    [Serializable]
    public struct Lpadding
    {
        public int Left, Right, Top, Bottom;
        public Lpadding(int l, int r, int t, int b) { Left = l; Right = r; Top = t; Bottom = b; }
        public RectOffset ToRectOffset() => new RectOffset(Left, Right, Top, Bottom);
    }

    public struct Lstyle
    {
        public Color? Color;
        public int? FontSize;
        public Vector2? Size;
        public RectOffset Padding;
        public float? Spacing;
        public TextAnchor? Alignment;

        public static Lstyle Default => new Lstyle();

        public Lstyle WithColor(Color color) { this.Color = color; return this; }
        public Lstyle WithFontSize(int size) { this.FontSize = size; return this; }
        public Lstyle WithSize(float x, float y) { this.Size = new Vector2(x, y); return this; }
        public Lstyle WithPadding(int left, int right, int top, int bottom) { this.Padding = new RectOffset(left, right, top, bottom); return this; }
        public Lstyle WithSpacing(float spacing) { this.Spacing = spacing; return this; }
        public Lstyle WithAlignment(TextAnchor alignment) { this.Alignment = alignment; return this; }
    }
}
