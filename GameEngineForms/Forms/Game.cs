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
        List<Vector4> Vectors = new List<Vector4>();
        Random rand = new Random();

        public Game() {

            AutoScaleMode = AutoScaleMode.Inherit;
            ClientSize = new Size(800, 450);
            StartPosition = FormStartPosition.CenterScreen;
            GameCycle += DrawLoop;
            Initialize += InitializeElements;
            BackColor = Color.BurlyWood;

        }
              
        private void InitializeElements()
        {
            for (int i = 0; i < 5; i++)
                Vectors.Add(new Vector4(rand.Next(0, Width),rand.Next(0, Height),rand.Next(0, Width),rand.Next(0, Height)));     
            

        }

        private void DrawLoop(object sender, PaintEventArgs e)
        {             
            Vectors.ForEach(v => DrawLine(Color.Black, 1, new Vector2(v.X, v.Y), new Vector2(v.Z,v.W)));


        }
    }
}
