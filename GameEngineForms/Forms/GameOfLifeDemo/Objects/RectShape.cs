using System;
using System.Collections.Generic;
using System.Text;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.MathServices;
using static GameEngineForms.Resources.DynamicResources;
using System.Drawing;



namespace GameEngineForms.Forms.GameOfLifeDemo.Objects
{
    public class RectShape
    {
        Size    dym;

        Point   leftUp_Corner, rightUp_Corner,
                leftDown_Corner, rightDown_Corner, center;





        int nobRadius;



        public RectShape()
        {
            dym = new Size(30, 30);
            leftUp_Corner = new Point
            {
                X = (GameObjects.LoopContainer.Width / 2) - (dym.Width/2),
                Y = (GameObjects.LoopContainer.Height / 2) - (dym.Height / 2)
            };

            GameCycle += Draw;
        }

        private void Draw(object sender, System.Windows.Forms.PaintEventArgs e)
        {





            e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(leftUp_Corner.X, leftUp_Corner.Y, dym.Width, dym.Height));

            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(leftUp_Corner.X - nobRadius, leftUp_Corner.Y - nobRadius, nobRadius * 2, nobRadius * 2));



        }
    }
}
