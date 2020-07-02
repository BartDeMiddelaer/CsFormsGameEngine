using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources;

namespace GameEngineForms
{
    public static class DrawServices
    {
        internal delegate void DrawDelegate(object sender, PaintEventArgs e);
        internal static event DrawDelegate DrawEvent;


        public static void Draw(object sender, PaintEventArgs e) => DrawEvent?.Invoke(sender,e);
        public static Form GetForm() => GameObjects.GameToRun;
        public static void SetForm(Form form) {
            GameObjects.GameToRun = form;   
        }



        public static PictureBox GetPictureBox() => GameObjects.PictureBox;


        public static void DrawLine(Color color, int thickness, Point startPoint, Point endPoint)
        {
            GameObjects.LineGeometry.Add(
                new Line
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint,
                    StrokeColor = color,
                    Thickness = thickness,
                    Angle = null,
                    Length = null
                });
        }
        public static void DrawLine(Color color, int thickness, Point startPoint, int length, float angle)
        {

            var endPoint_x = startPoint.X + Math.Cos((Math.PI / 180.0) * angle) * length;
            var endPoint_y = startPoint.Y + Math.Sin((Math.PI / 180.0) * angle) * length;

            GameObjects.LineGeometry.Add(
                new Line
                {
                    StartPoint = startPoint,
                    EndPoint = new Point((int)endPoint_x, (int)endPoint_y),
                    StrokeColor = color,
                    Thickness = thickness,
                    Angle = angle,
                    Length = length
                });
        }

        public static void DrawRect(Color? strokeColor, Color? fillColor, float? strokeThickness, Point startPoint, int width, int height, int? cornerRadius, float? angle)
        {
            GameObjects.RectGeometry.Add(
                new Rect { 
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    StrokeThickness = strokeThickness,
                    StartPoint = startPoint,
                    Width = width,
                    Height = height,
                    CornerRadius = cornerRadius,
                    Angle = angle
                });      
        }
    }
}
