using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Line
    {
        public float? Angle { get; set; }
        public int? Length { get; set; }
        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }
        public float Thickness { get; set; }
        public Color StrokeColor { get; set; }
    }
}
