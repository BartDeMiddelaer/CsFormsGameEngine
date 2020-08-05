using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameEngineForms.Resources.Shapes;
using GameEngineForms.Forms.GameOfLifeDemo;
using static GameEngineForms.Services.EventServices;

namespace GameEngineForms.Resources
{
    public sealed class DynamicResources
    {
        // Singelton ---------------------------------------------------------------
        public static DynamicResources GameObjects
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DynamicResources();
                    }
                    return instance;
                }
            }
        }
        private static DynamicResources instance = null;
        private static readonly object padlock = new object();
        // -------------------------------------------------------------------------

        // Life Time Porpertys -----------------------------------------------------

        public Form FormToRun { get; set; } = new GameOfLife(); // <------------ Set game here
        public PictureBox LoopContainer { get; set; } = new PictureBox();
        public SmoothingMode RenderMode { get; set; } = SmoothingMode.HighSpeed;
        public int MinimumLodeScreenTime { get; set; } = 10;


        // -------------------------------------------------------------------------


        // GameCycle Porpertys -----------------------------------------------------
        public bool UseDrawFunctions { get; set; } = false;
        public List<Line> LineGeometry { get; set; } = new List<Line>();
        public List<Rect> RectGeometry { get; set; } = new List<Rect>();
        public List<Circle> CircleGeometry { get; set; } = new List<Circle>();
        public List<Ellipse> EllipseGeometry { get; set; } = new List<Ellipse>();
        public List<Text> TextGeometry { get; set; } = new List<Text>();
        public int ObjectCount { get; set; } = 0;

        // GameCycle Porpertys Reseting
        public DynamicResources() {

            Destructor += () => {
                LineGeometry.Clear();
                RectGeometry.Clear();
                CircleGeometry.Clear();
                EllipseGeometry.Clear();
                TextGeometry.Clear();
                ObjectCount = 0;
            };           
        } 

        // -------------------------------------------------------------------------
    }  
}
