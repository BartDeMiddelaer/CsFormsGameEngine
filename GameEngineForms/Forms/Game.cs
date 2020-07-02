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
using static GameEngineForms.Resources.ResourcesDeclaration;




namespace GameEngineForms.Forms
{
    public partial class Game : Form
    {
        int x = 0;
        public Game()
        {
            //components = new Container();
            //AutoScaleMode = AutoScaleMode.Dpi;

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
            DrawEllipse(null, Color.Red, 0, new Point(200, 200), 150, 180, x);
            x++;
        }
    }
}
