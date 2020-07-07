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
        readonly List<Rectangle> celDraw = new List<Rectangle>();
        readonly List<Rectangle> supQaudDraw = new List<Rectangle>();
        readonly List<Rectangle> quadDraw = new List<Rectangle>();
        int[,] oldCels, newCels, quadrants, subQuadrants;

        readonly static int
            celDymSqwd = 4,
            // moet deelbaar door 10 zijn
            maxCelsInX = 420,
            maxCelsInY = 260,
            quadrantsInX = maxCelsInX / 10,
            quadrantsInY = maxCelsInY / 10;



        public GameOfLife() {
            Initialize += GameOfLife_Initialize;
            GameCycle += DrawLoop;
            ClientSize = new Size(celDymSqwd * maxCelsInX, celDymSqwd * maxCelsInY);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            TopMost = true;
            BackColor = Color.Black;
            TransparencyKey = Color.Magenta;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void GameOfLife_Initialize()
        {
            GameObjects.RenderMode = SmoothingMode.HighSpeed;

            oldCels = new int[maxCelsInX, maxCelsInY];
            newCels = new int[maxCelsInX, maxCelsInY];
            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];


            if (true) // Random
            for (int x = 0; x < maxCelsInX; x++)
                for (int y = 0; y < maxCelsInY; y++)
                    newCels[x, y] = rand.Next(0, 2);

            if (true) // glidergun
            {
                for (int x = 0; x < maxCelsInX; x++)
                    for (int y = 0; y < maxCelsInY; y++)
                        newCels[x, y] = 0;

                GlitterGun(0, 0);
                GlitterGun(100, 100);
                GlitterGun(200, 200);

                GlitterGun(100, 0);
                GlitterGun(200, 100);
                GlitterGun(300, 200);


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
            DrawCels(e);
          
            MakeQuadrants(e, true,false);

            UpdateCels(true);     
        }


        private void DrawCels(PaintEventArgs e)
        {
            celDraw.Clear();

            for (int x = 0; x < maxCelsInX; x++)
                for (int y = 0; y < maxCelsInY; y++)
                    if (newCels[x, y] == 1)
                    {
                        celDraw.Add(new Rectangle(x * celDymSqwd, y * celDymSqwd, celDymSqwd, celDymSqwd));
                        quadrants[x / 10, y / 10]++;
                    }

            if (celDraw.Count != 0)
            {
                e.Graphics.FillRectangles(Brushes.Green, celDraw.ToArray());
                e.Graphics.DrawRectangles(Pens.DarkRed, celDraw.ToArray());
            }

            GameObjects.ObjectCount = celDraw.Count;
        }
        private void UpdateCels(bool Quadrants)
        {
            oldCels = newCels;
            newCels = new int[maxCelsInX, maxCelsInY];

            if (Quadrants)
            {
                for (int qwadX = 0; qwadX < quadrantsInX; qwadX++)
                    for (int qwadY = 0; qwadY < quadrantsInY; qwadY++)
                    {
                        if (quadrants[qwadX, qwadY] != 0 || subQuadrants[qwadX, qwadY] != 0)
                            for (int celXindex = 0; celXindex < 10; celXindex++)
                            {
                                for (int celYindex = 0; celYindex < 10; celYindex++)
                                {
                                    int x = (qwadX * 10) + celXindex;
                                    int y = (qwadY * 10) + celYindex;

                                    if (oldCels[x, y] == 1 && (Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3)) { quadrants[x / 10, y / 10]++; newCels[x, y] = 1; }
                                    if (oldCels[x, y] == 1 && Livingneighbors(x, y) > 4 || oldCels[x, y] == 1 && Livingneighbors(x, y) < 2) { quadrants[x / 10, y / 10]++; newCels[x, y] = 0; }
                                    if (oldCels[x, y] == 0 && Livingneighbors(x, y) == 3) { newCels[x, y] = 1; }
                                }
                            }
                    }
            }
            else { UpdateCels_NOQuadrants(); }
           
            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];
        }
        private void UpdateCels_NOQuadrants()
        {
            oldCels = newCels;
            newCels = new int[maxCelsInX, maxCelsInY];
            
            for (int x = 0; x < maxCelsInX; x++)
            {
                for (int y = 0; y < maxCelsInY; y++)
                {
                    if (oldCels[x, y] == 1 && (Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3)) { quadrants[x / 10, y / 10]++; newCels[x, y] = 1; }
                    if (oldCels[x, y] == 1 && Livingneighbors(x, y) > 4 || oldCels[x, y] == 1 && Livingneighbors(x, y) < 2) { quadrants[x / 10, y / 10]++; newCels[x, y] = 0; }
                    if (oldCels[x, y] == 0 && Livingneighbors(x, y) == 3) { newCels[x, y] = 1; }
                }
            }                
                

            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];
        }



        private void MakeQuadrants(PaintEventArgs e,bool draw,bool info)
        {
            quadDraw.Clear();
            supQaudDraw.Clear();

            if (draw)
            {
                for (int x = 0; x < quadrantsInX; x++)
                    for (int y = 0; y < quadrantsInY; y++)
                    {
                        if (quadrants[x, y] != 0)
                        {
                            if(info)
                                DrawText($"{quadrants[x, y]}", new Font("", celDymSqwd * 2), Color.White, new Vector2(x * (10 * celDymSqwd), y * (10 * celDymSqwd)), null);

                            quadDraw.Add(new Rectangle(x * (10 * celDymSqwd), y * (10 * celDymSqwd), (10 * celDymSqwd), (10 * celDymSqwd)));
                            MarkSubQuadrants(x, y);
                        }
                    }

                for (int x = 0; x < quadrantsInX; x++)
                    for (int y = 0; y < quadrantsInY; y++)
                    {
                        if (subQuadrants[x, y] != 0)
                        {
                            if(info)
                                DrawText($"{subQuadrants[x, y]}", new Font("", celDymSqwd * 2), Color.White, new Vector2(x * (10 * celDymSqwd), y * (10 * celDymSqwd)), null);

                            supQaudDraw.Add(new Rectangle(x * (10 * celDymSqwd), y * (10 * celDymSqwd), (10 * celDymSqwd), (10 * celDymSqwd)));
                        }
                    }

               

                if (quadDraw.Count != 0)
                e.Graphics.DrawRectangles(new Pen(Color.FromArgb(20, 20, 20), 0.2f), supQaudDraw.ToArray());
                    e.Graphics.DrawRectangles(Pens.Gray, quadDraw.ToArray());

            }
            else
            {
                for (int x = 0; x < quadrantsInX; x++)
                    for (int y = 0; y < quadrantsInY; y++)
                        if (quadrants[x, y] != 0)
                            MarkSubQuadrants(x, y);
            }
          
        }
        private void DrawSubQuadrants(PaintEventArgs e)
        {
            

            for (int x = 0; x < quadrantsInX; x++)
                for (int y = 0; y < quadrantsInY; y++)
                {
                    if (subQuadrants[x, y] != 0)
                    {
                        
                    }
                }



        }
        private void MarkSubQuadrants(int x, int y)
        {            
            
                subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] = quadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] = quadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] = quadrants[((x) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] = quadrants[((x) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] = quadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] += 1
                    : subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] = quadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] = quadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] != 0
                    ? subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY]
                    : subQuadrants[((x - 1) + quadrantsInX) % quadrantsInX, ((y + 1) + quadrantsInY) % quadrantsInY] += 1;

                subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] = quadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] != 0
                        ? subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY]
                        : subQuadrants[((x + 1) + quadrantsInX) % quadrantsInX, ((y - 1) + quadrantsInY) % quadrantsInY] += 1;
                           
        }



        private int Livingneighbors(int x, int y)
        {
            int tell = 0;

            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY] == 1 ? 1 : 0;
            
            return tell;
        }

    }
}
