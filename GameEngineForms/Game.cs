using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.DrawServices;
using static GameEngineForms.Resources;




namespace GameEngineForms
{
    public partial class Game : Form
    {
        int z = 0;
        public Game()
        {
            ClientSize = new Size(450, 350);        
            BackColor = Color.BlanchedAlmond;
            DrawEvent += Draw;
            KeyDown += KeyDownLisener;
        }

        private void KeyDownLisener(object sender, KeyEventArgs e)
        {

    

        }

        private void Draw(object sender, PaintEventArgs e)
        {           
          


        }
    }
}
