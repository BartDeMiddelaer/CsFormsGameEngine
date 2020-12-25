using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GameEngineForms.Resources.GameEngineObjects;


namespace GameEngineForms.Services
{
    public static class ExtensionMethods
    {
        public static void AddValuePerSec(ref this float value, float pps)
        {
            value = value + pps;
        }
        public static void GetPointFromPoint(ref this Vector2 value, double l_, double h_)
        {
            value = new Vector2(
                (float)(value.X + Math.Cos((Math.PI / 180.0) * h_) * l_),
                (float)(value.Y + Math.Sin((Math.PI / 180.0) * h_) * l_)
                );
        }
        public static Vector2 Intersection_LineToLine(this Vector2 value, Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;

            float delta = (float)Math.Round(a1 * b2 - a2 * b1);

            float x = float.IsInfinity((b2 * c1 - b1 * c2) / delta) ? -2100 : (b2 * c1 - b1 * c2) / delta;
            float y = float.IsInfinity((a1 * c2 - a2 * c1) / delta) ? -2100 : (a1 * c2 - a2 * c1) / delta;

            value =
                Math.Sign(delta) > 0
                ? new Vector2(-2100, -2100)
                : new Vector2(x, y);

            return value;
        }
        public static Vector2 RandomVector2(this Vector2 vector)
        {
            vector.X = new Random().Next(0, GameObject.LoopContainer.Width);
            vector.Y = new Random().Next(0, GameObject.LoopContainer.Height);
            return vector;
        }
      
    }
}
