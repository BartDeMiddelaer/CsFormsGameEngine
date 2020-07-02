using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.DrawServices;

namespace GameEngineForms
{
    public sealed class Resources
    {
        public static Resources GameObjects
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Resources();
                    }
                    return instance;
                }
            }
        }
        private static Resources instance = null;
        private static readonly object padlock = new object();
     


        public Form GameToRun { get; set; } = new Game();
        public PictureBox PictureBox { get; set; } = new PictureBox();


        // GameCycle Porpertys -----------------------------------------------------
        public List<Line> LineGeometry { get; set; } = new List<Line>();
        public List<Rect> RectGeometry { get; set; } = new List<Rect>();
        public List<Circle> CircleGeometry { get; set; } = new List<Circle>();
        public List<Ellipse> EllipseGeometry { get; set; } = new List<Ellipse>();
        public int ObjectCount { get; set; } = 0;

        // GameCycle Porpertys Reseting
        public Resources() => Destructor += () => {

            LineGeometry.Clear();
            RectGeometry.Clear();
            CircleGeometry.Clear();
            EllipseGeometry.Clear();
            ObjectCount = 0;
        };

        // -------------------------------------------------------------------------

    }
    public struct Line
    {
        public float? Angle { get; set; }
        public int? Length { get; set; }
        public Point StartPoint { get; set; }
        public Point? EndPoint { get; set; }
        public float Thickness { get; set; }
        public Color StrokeColor { get; set; }
    }
    public struct Rect
    {
        public float? Angle { get; set; }
        public Point StartPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? CornerRadius { get; set; }
    }
    public struct Circle
    {
        public Point CenterPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public float Radius { get; set; }
    }
    public struct Ellipse
    {
        public float? Angle { get; set; }
        public Point CenterPoint { get; set; }
        public float? StrokeThickness { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
