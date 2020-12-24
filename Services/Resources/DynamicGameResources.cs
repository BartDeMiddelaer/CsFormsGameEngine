using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using GameEngineForms.Forms;
using GameEngineForms.Forms.CircelPacking;

namespace GameEngineForms.Resources
{
    public sealed class DynamicGameResources
    {
        // Singelton ---------------------------------------------------------------
        public static DynamicGameResources GameObjects
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DynamicGameResources();
                    }
                    return instance;
                }
            }
        }
        private static DynamicGameResources instance = null;
        private static readonly object padlock = new object();
        // -------------------------------------------------------------------------

        // Life Time Porpertys -----------------------------------------------------


        public DefaultFormParent FormToRun { get; set; } = new CircelPacking(); // <------------ Set game here
        public PictureBox LoopContainer { get; set; } = new PictureBox();
        public SmoothingMode RenderMode { get; set; } = SmoothingMode.HighSpeed;
        public int MinimumLodeScreenTime { get; set; } = 400;


        // -------------------------------------------------------------------------


        // GameCycle Porpertys -----------------------------------------------------
        public int ObjectCount { get; set; } = 0;
        // -------------------------------------------------------------------------
    }  
}
