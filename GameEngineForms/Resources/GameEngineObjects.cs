using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using GameEngineForms.Forms;
using GameEngineForms.Forms.CircelPackingDemo;
using GameEngineForms.Forms.GameOfLifeDemo;

namespace GameEngineForms.Resources
{
    public sealed class GameEngineObjects
    {
        // Singelton ---------------------------------------------------------------
        public static GameEngineObjects GameObject
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameEngineObjects();
                    }
                    return instance;
                }
            }
        }
        private static GameEngineObjects instance = null;
        private static readonly object padlock = new object();
        // -------------------------------------------------------------------------

        // Life Time Porpertys -----------------------------------------------------
        public DefaultParentForm GameToRun { get; set; } = new CircelPacking(); // <------------ Set game here
        public PictureBox LoopContainer { get; set; } = new PictureBox();
        public SmoothingMode RenderMode { get; set; } = SmoothingMode.HighSpeed;
        public int MinimumLodeScreenTime { get; set; } = 100;

        // -------------------------------------------------------------------------


        // GameCycle Porpertys -----------------------------------------------------
        public int ObjectCount { get; set; } = 0;
        // -------------------------------------------------------------------------
    }  
}
