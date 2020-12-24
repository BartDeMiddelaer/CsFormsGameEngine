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
        public static Vector2 GetMousePosition()
        {
            Rectangle screenRectangle = GameObject.GameToRun.RectangleToScreen(GameObject.GameToRun.ClientRectangle);

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
    }
}
