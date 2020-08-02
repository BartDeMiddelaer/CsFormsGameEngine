using System;
using System.Collections.Generic;
using System.Text;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.MathServices;
using static GameEngineForms.Resources.DynamicResources;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngineForms.Forms.GameOfLifeDemo.Objects
{
    public class CircleShape
    {
        public Point Center { get; set; }
        public int Radius { get; set; }





        public CircleShape()
        {
            Center = new Point
            {
                X = (GameObjects.LoopContainer.Width / 2),
                Y = (GameObjects.LoopContainer.Height / 2)
            };

            Radius = 51;
            GameCycle += loopAlgo;

        }
        public CircleShape(int x, int y, int r)
        {
            Center = new Point
            {
                X = x,
                Y = y
            };

            Radius = r;
            GameCycle += loopAlgo;

        }



        private void loopAlgo(object sender, PaintEventArgs e)
        {
            
        }
    }
}
