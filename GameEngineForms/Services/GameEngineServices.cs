using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static GameEngineForms.Resources.GameEngineObjects;


namespace GameEngineForms.Services
{
    public static class GameEngineServices
    {
        public static T GetMousePosition<T>()
        {
            Rectangle screenRectangle = GameObject.GameToRun.RectangleToScreen(GameObject.GameToRun.ClientRectangle);

            var realX = Control.MousePosition.X - screenRectangle.X;
            var realY = Control.MousePosition.Y - screenRectangle.Y;

            if (typeof(T) == typeof(Point)) return (T)Convert.ChangeType(new Point(realX, realY), typeof(T));
            if (typeof(T) == typeof(Vector2)) return (T)Convert.ChangeType(new Vector2(realX, realY), typeof(T));

            return default(T);
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
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        public static Color RandomColor()
        {
            return Color.FromArgb(new Random().Next(0, 256), new Random().Next(0, 256), new Random().Next(0, 256));
        }

        public static Vector2 Intersection_Line(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
        {
            double a1 = e1.Y - s1.Y;
            double b1 = s1.X - e1.X;
            double c1 = a1 * s1.X + b1 * s1.Y;

            double a2 = e2.Y - s2.Y;
            double b2 = s2.X - e2.X;
            double c2 = a2 * s2.X + b2 * s2.Y;

            double delta = a1 * b2 - a2 * b1;

            return delta == 0 ? new Vector2()
            : new Vector2((float)((b2 * c1 - b1 * c2) / delta), (float)((a1 * c2 - a2 * c1) / delta));
        }
        public static int Intersection_Circle(float cx, float cy, float radius, Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
        {
            double dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) +
                (point1.Y - cy) * (point1.Y - cy) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 =
                    new Vector2((float)(point1.X + t * dx), (float)(point1.Y + t * dy));
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 =
                    new Vector2((float)(point1.X + t * dx), (float)(point1.Y + t * dy));
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 =
                    new Vector2((float)(point1.X + t * dx), (float)(point1.Y + t * dy));
                return 2;
            }
        }
        public static bool Intersection_PointInCircle(Vector2 pointToCheak_, Vector2 centerCircal_, int radius_)
        {
            return Math.Sqrt(Math.Pow((pointToCheak_.X - centerCircal_.X), 2) + Math.Pow((pointToCheak_.Y - centerCircal_.Y), 2)) < radius_;
        }



        public static bool Intersection_PointInCircle(Vector2 pointToCheak_, Vector2 centerCircal_, int radius_, out double output_)
        {
            output_ = Math.Sqrt(Math.Pow((pointToCheak_.X - centerCircal_.X), 2) + Math.Pow((pointToCheak_.Y - centerCircal_.Y), 2));
            return output_ < radius_;
        }

        public static void Intersection_Circle(Vector2 cCenter, float radius, Vector2 point1, Vector2 point2, out Vector2 intersection)
        {
            double dx, dy, A, B, C, det, t;
            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cCenter.X) + dy * (point1.Y - cCenter.Y));
            C = (point1.X - cCenter.X) * (point1.X - cCenter.X) +
                (point1.Y - cCenter.Y) * (point1.Y - cCenter.Y) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection = new Vector2(float.NaN, float.NaN);
            }
            else
            {
                t = (-B - Math.Sqrt(det)) / (2 * A);
                intersection = new Vector2((float)(point1.X + t * dx), (float)(point1.Y + t * dy));
            }
        }
    }
}
