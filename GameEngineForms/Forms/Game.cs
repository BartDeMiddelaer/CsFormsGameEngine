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
using static GameEngineForms.Resources.DynamicResources;
using System.Numerics;
using System.Drawing.Drawing2D;

namespace GameEngineForms.Forms
{

    public partial class Game : Form
    {
        public Game() => Initialize += () => {
           
            GameCycle += DrawLoop;

            ClientSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            TopMost = true;

            BackColor = Color.BurlyWood;
            StartPosition = FormStartPosition.CenterScreen;
            GameObjects.RenderMode = SmoothingMode.HighSpeed;
            
        };

        public void DrawLoop(object sender, PaintEventArgs e)
        {      
           
        }      
    }
}
