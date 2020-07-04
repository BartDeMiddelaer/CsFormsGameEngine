using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GameEngineForms.Resources.Shapes;
using static GameEngineForms.Resources.ResourcesDeclaration;


namespace GameEngineForms.Services
{
    public static class MathServices
    {
        static DateTime lastCheckTime = DateTime.Now;
        static long frameCount = 0;


        public static Vector2 GetMousePosition()
        {
            Rectangle screenRectangle = GameObjects.FormToRun.RectangleToScreen(GameObjects.FormToRun.ClientRectangle);

            return new Vector2
            {
                X = Control.MousePosition.X - screenRectangle.X,
                Y = Control.MousePosition.Y - screenRectangle.Y
            };
        }
        public static Vector2 GetPointFromPoint(Vector2 from_, double l_, double h_)
        {
            return new Vector2(
                (float)(from_.X + Math.Cos((Math.PI / 180.0) * h_) * l_),
                (float)(from_.Y + Math.Sin((Math.PI / 180.0) * h_) * l_)
                );
        }
        public static float GetAngel(Vector2 one_, Vector2 tho_)
        {
            float xDiff = one_.X - tho_.X;
            float yDiff = one_.Y - tho_.Y;
            return (float)(Math.Atan2(-yDiff, -xDiff) * (180 / Math.PI));
        }


        // Extension Methods -------------------------------------------------------


        // float Extensions -----------
        public static void GetAngel(ref this float value, Vector2 one_, Vector2 tho_)
        {
            float xDiff = one_.X - tho_.X;
            float yDiff = one_.Y - tho_.Y;
            value = (float)(Math.Atan2(-yDiff, -xDiff) * (180 / Math.PI));
        }
        public static void AddValuePerSec(ref this float value, float pps)
        {
            double secondsElapsed = (DateTime.Now - lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref frameCount, 0);
            double fps = count / secondsElapsed;
            lastCheckTime = DateTime.Now;
            double speed = pps / fps;
            Interlocked.Increment(ref frameCount);
            value = float.IsInfinity((float)speed) ? 0 : value + (float)speed;
        }

        // Vector2 Extensions ------------


        public static void GetPointFromPoint(ref this Vector2 value, Vector2 from_, double l_, double h_)
        {
            value = new Vector2(
                (float)(from_.X + Math.Cos((Math.PI / 180.0) * h_) * l_),
                (float)(from_.Y + Math.Sin((Math.PI / 180.0) * h_) * l_)
                );
        }
        public static void Intersection_LineToLine(ref this Vector2 value, Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
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
                ? new Vector2(-2100,-2100)
                : new Vector2(x, y);

        }
        public static void IntersectionNearest_LineToLine(ref this Vector2 value, Vector2 s1, Vector2 e1, List<Line> lines)
        { 

        
        }

        public static void RandomVector2(ref this Vector2 vector)
        { 
            vector = new Vector2(
                new Random().Next(0, GameObjects.DrawContainer.Width),
                new Random().Next(0, GameObjects.DrawContainer.Height));       
        }

        // Divers Extensions ----------
        public static void RandomColor(ref this Color col)
        {
            col = Color.FromArgb(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256));        
        }





        // -------------------------------------------------------------------------

    }
}
