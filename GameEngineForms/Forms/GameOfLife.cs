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
using System.Threading.Tasks;

namespace GameEngineForms.Forms
{
    public partial class GameOfLife : Form
    {
        readonly Random rand = new Random();
        readonly List<Rectangle> drawBlocks = new List<Rectangle>();
        readonly Timer KillingTime = new Timer();

        readonly int celDrawIntervals = 6, celOfset = 2;
        int[,] oldCels, newCels;
        int maxX, maxY;

        public GameOfLife() {
            KillingTime.Start();
            KillingTime.Interval = 5;
            KillingTime.Tick += (object sender, EventArgs e) => KillTheFuckers();
            Initialize += GameOfLife_Initialize;
            GameCycle += DrawLoop;
            ClientSize = new Size(1280, 900);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            TopMost = true;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void GameOfLife_Initialize()
        {          
            GameObjects.RenderMode = SmoothingMode.HighSpeed;

            maxX = (Width / celDrawIntervals);
            maxY = (Height / celDrawIntervals);

            oldCels = new int[maxX, maxY];
            newCels = new int[maxX, maxY];

            if(false) // Random
            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    newCels[x, y] = rand.Next(0, 5);

            if (true) // glidergun
            {
                for (int x = 0; x < maxX; x++)
                    for (int y = 0; y < maxY; y++)
                        newCels[x, y] = 0;

                GlitterGun(0, 0);
                GlitterGun(181, 0);
                GlitterGun(-2, 133);
                GlitterGun(181, 133);
                GlitterGun(90, 72);         
            }
            
        }

        private void GlitterGun(int x, int y)
        {
            newCels[2+x, 2 + y] = 1;     newCels[3 + x, 2 + y] = 1;   newCels[2 + x, 3 + y] = 1;   newCels[3 + x, 3 + y] = 1;
            newCels[9 + x, 2 + y] = 1;   newCels[10 + x, 2 + y] = 1;  newCels[10 + x, 3 + y] = 1;  newCels[9 + x, 3 + y] = 1;
            newCels[6 + x, 6 + y] = 1;   newCels[6 + x, 5 + y] = 1;   newCels[7 + x, 5 + y] = 1;   newCels[7 + x, 6 + y] = 1;
            newCels[33 + x, 12 + y] = 1; newCels[33 + x, 13 + y] = 1; newCels[34 + x, 12 + y] = 1; newCels[34 + x, 13 + y] = 1;
            newCels[22 + x, 19 + y] = 1; newCels[22 + x, 20 + y] = 1; newCels[23 + x, 19 + y] = 1; newCels[23 + x, 21 + y] = 1;
            newCels[24 + x, 21 + y] = 1; newCels[25 + x, 21 + y] = 1; newCels[25 + x, 22 + y] = 1; newCels[24 + x, 11 + y] = 1;
            newCels[25 + x, 11 + y] = 1; newCels[27 + x, 11 + y] = 1; newCels[28 + x, 11 + y] = 1; newCels[23 + x, 12 + y] = 1;
            newCels[29 + x, 12 + y] = 1; newCels[23 + x, 13 + y] = 1; newCels[30 + x, 13 + y] = 1; newCels[23 + x, 14 + y] = 1;
            newCels[24 + x, 14 + y] = 1; newCels[25 + x, 14 + y] = 1; newCels[29 + x, 14 + y] = 1; newCels[28 + x, 15 + y] = 1;
        }

        private void DrawLoop(object sender, PaintEventArgs e)
        {
            drawBlocks.Clear();
         
            for (int x = 0; x < maxX; x++)           
                for (int y = 0; y < maxY; y++)             
                    if (newCels[x, y] == 1)
                        drawBlocks.Add(new Rectangle(x * celDrawIntervals, y * celDrawIntervals, celDrawIntervals - celOfset, celDrawIntervals - celOfset));             
                
           
            if(drawBlocks.Count > 1)
                e.Graphics.DrawRectangles(Pens.Red, drawBlocks.ToArray());

            GameObjects.ObjectCount = drawBlocks.Count;

        }

        private void KillTheFuckers()
        {            
            oldCels = newCels;
            newCels = new int[maxX, maxY];

            for (int x = 0; x < maxX; x++)          
                for (int y = 0; y < maxY; y++)
                {
                    if (oldCels[x, y] == 1 && (Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3)) newCels[x, y] = 1;                                                      
                    if (oldCels[x, y] == 1 && Livingneighbors(x,y) > 4 || oldCels[x, y] == 1 && Livingneighbors(x, y) < 2) newCels[x, y] = 0;
                    if (oldCels[x, y] == 0 && Livingneighbors(x, y) == 3) newCels[x, y] = 1;                    
                }            
        }

        private int Livingneighbors(int x, int y)
        {
            int tell = 0;         

            tell += oldCels[((x + 1) + maxX) % maxX, ((y) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxX) % maxX, ((y) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x) + maxX) % maxX, ((y + 1) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x) + maxX) % maxX, ((y - 1) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x + 1) + maxX) % maxX, ((y + 1) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxX) % maxX, ((y - 1) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxX) % maxX, ((y + 1) + maxY) % maxY] == 1 ? 1 : 0;
            tell += oldCels[((x + 1) + maxX) % maxX, ((y - 1) + maxY) % maxY] == 1 ? 1 : 0;

            return tell;
        }       
    }
}
