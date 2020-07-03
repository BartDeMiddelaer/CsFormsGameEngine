using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.MathServices;
using static GameEngineForms.Resources.ResourcesDeclaration;
using System.Numerics;

namespace GameEngineForms.Forms
{

    public partial class Game : Form
    {
        float x = 1;
        public Game()
        {
            ClientSize = new Size(800, 600);        
            BackColor = Color.BlanchedAlmond;
            GameCycle += DrawLoop;
            KeyDown += KeyDownLisener;
        }

        private void KeyDownLisener(object sender, KeyEventArgs e)
        {

            

        }


        private void DrawLoop(object sender, PaintEventArgs e)
        {
           x += x.MovePPS(100);


            DrawEllipse(Color.Black,null,2,new Vector2(100,100),100,110,x);

        }
    }
}
