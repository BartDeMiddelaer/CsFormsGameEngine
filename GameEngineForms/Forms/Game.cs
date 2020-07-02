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



namespace GameEngineForms.Forms
{

    public partial class Game : Form
    {
        float x = 0;

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
            DrawText("Rubbe is GAYyyy", new Font("Arial", 80), Color.Black, GetMousePosition(), x);

            x += 0.5f;
        }
    }
}
