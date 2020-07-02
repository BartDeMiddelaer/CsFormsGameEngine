using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Rect
    {
        public float? Angle { get; set; }
        public Point StartPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? CornerRadius { get; set; }
    }
}
