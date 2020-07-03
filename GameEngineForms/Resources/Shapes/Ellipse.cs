using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Ellipse
    {
        public float? Angle { get; set; }
        public Vector2 CenterPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
