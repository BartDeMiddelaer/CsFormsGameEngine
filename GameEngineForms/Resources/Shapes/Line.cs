using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Line
    {
        public float? Angle { get; set; }
        public int? Length { get; set; }
        public Point StartPoint { get; set; }
        public Point? EndPoint { get; set; }
        public float Thickness { get; set; }
        public Color StrokeColor { get; set; }
    }
}
