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
        Random rand = new Random();
        List<Rectangle> drawBlocks = new List<Rectangle>();

        int[,] oldCels, newCels;
        int celDrawIntervals = 5, celOfset = 2, maxX, maxY;

        public GameOfLife() {

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
            GameObjects.RenderMode = SmoothingMode.None;

            maxX = (Width / celDrawIntervals);
            maxY = (Height / celDrawIntervals) +500;

            oldCels = new int[maxX, maxY];
            newCels = new int[maxX, maxY];

            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    newCels[x, y] = rand.Next(0, 5);
        }

        private void DrawLoop(object sender, PaintEventArgs e)
        {
            KillTheFuckers();
            drawBlocks.Clear();

            for (int x = 0; x < maxX; x++)           
                for (int y = 0; y < maxY; y++)             
                    if (newCels[x, y] == 1)
                        drawBlocks.Add(new Rectangle(x * celDrawIntervals, y * celDrawIntervals, celDrawIntervals - celOfset, celDrawIntervals - celOfset));             
                
            
            GameObjects.ObjectCount = drawBlocks.Count;
            e.Graphics.DrawRectangles(Pens.Red, drawBlocks.ToArray());
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
