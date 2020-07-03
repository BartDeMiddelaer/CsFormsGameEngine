using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace GameEngineForms.Resources.Shapes
{
    public struct Text
    {
        public float? Angle { get; set; }
        public string Context { get; set; }
        public Vector2 StartPoint { get; set; }
        public Font Font { get; set; }
        public Color FontColor { get; set; }
    }
}
