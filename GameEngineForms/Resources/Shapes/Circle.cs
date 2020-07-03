using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Circle
    {
        public Vector2 CenterPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public float Radius { get; set; }
    }
}
