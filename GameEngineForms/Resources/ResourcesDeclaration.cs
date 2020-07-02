using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GameEngineForms.Forms;
using GameEngineForms.Resources.Shapes;
using static GameEngineForms.Services.EventServices;


namespace GameEngineForms.Resources
{
    public sealed class ResourcesDeclaration
    {
        // Singelton ---------------------------------------------------------------
        public static ResourcesDeclaration GameObjects
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ResourcesDeclaration();
                    }
                    return instance;
                }
            }
        }
        private static ResourcesDeclaration instance = null;
        private static readonly object padlock = new object();
        // -------------------------------------------------------------------------

        // Life Time Porpertys -----------------------------------------------------
        public Form FormToRun { get; set; } = new Game();
        public PictureBox PictureBox { get; set; } = new PictureBox();
        // -------------------------------------------------------------------------


        // GameCycle Porpertys -----------------------------------------------------
        public List<Line> LineGeometry { get; set; } = new List<Line>();
        public List<Rect> RectGeometry { get; set; } = new List<Rect>();
        public List<Circle> CircleGeometry { get; set; } = new List<Circle>();
        public List<Ellipse> EllipseGeometry { get; set; } = new List<Ellipse>();
        public int ObjectCount { get; set; } = 0;

        // GameCycle Porpertys Reseting
        public ResourcesDeclaration() => Destructor += () => {

            LineGeometry.Clear();
            RectGeometry.Clear();
            CircleGeometry.Clear();
            EllipseGeometry.Clear();
            ObjectCount = 0;
        };

        // -------------------------------------------------------------------------
    }  
}
