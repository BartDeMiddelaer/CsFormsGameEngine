using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GameEngineForms.Resources;
using GameEngineForms.Resources.Shapes;
using static GameEngineForms.Resources.ResourcesDeclaration;

namespace GameEngineForms.Services
{
    public static class DrawServices
    {             
        public static void DrawLine(Color color, float thickness, Point startPoint, Point endPoint)
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
        public static void DrawLine(Color color, float thickness, Point startPoint, int length, float angle)
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
                new Rect
                { 
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
        public static void DrawCircle(Point centerPoint, float radius, Color? fillColor, Color? strokeColor, float strokeThickness)
        {
            GameObjects.CircleGeometry.Add( new Circle { 
                CenterPoint = centerPoint,
                Radius = radius,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            });
        }
        public static void DrawEllipse(Color? strokeColor, Color? fillColor, float? strokeThickness, Point centerPoint, int width, int height, float? angle)
        {
            GameObjects.EllipseGeometry.Add(
                new Ellipse
                {
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    StrokeThickness = strokeThickness,
                    CenterPoint = centerPoint,
                    Width = width,
                    Height = height,                  
                    Angle = angle
                });
        }
    }
}
