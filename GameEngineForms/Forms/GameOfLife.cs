using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.MathServices;
using static GameEngineForms.Resources.DynamicResources;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.ComponentModel.DataAnnotations;

namespace GameEngineForms.Forms
{
    public partial class GameOfLife : Form
    {
        /// <summary>
        /// https://www.youtube.com/watch?v=6avJHaC3C2U
        /// </summary>

        #region Vars decleration

        internal delegate void ControleDrawDelegate(object sender, PaintEventArgs e);
        internal static event ControleDrawDelegate ControleDraw;

        public enum Paterens {
            GosperGliderGun,
            SimkinGliderGun,
            PentaDecathlon,
            HWSS
        };

        Button btnPlayPauze, btnReset, btnSpawn;
        CheckBox cbQuadrantsUse, cbDrawQuadrants, cbDrawQuadrantsInfo;
        ComboBox cmbPaterens;

        ColorDialog 
            cdgCelColor = new ColorDialog { Color = Color.Red }, 
            cdgCelStrokeColor = new ColorDialog { Color = Color.DarkRed }, 
            cdgColorQuadrantBorder = new ColorDialog { Color = Color.FromArgb(125, 50, 125) }, 
            cdgColorSubQuadrantBorder = new ColorDialog { Color = Color.FromArgb(50, 0, 50) };

        readonly Random rand = new Random();
        readonly List<Rectangle> celDraw = new List<Rectangle>();
        readonly List<Rectangle> supQaudDraw = new List<Rectangle>();
        readonly List<Rectangle> quadDraw = new List<Rectangle>();
        int[,] oldCels, newCels, quadrants, subQuadrants;
        int celSize, maxCelsInX, maxCelsInY, quadrantsInX, quadrantsInY,
            widthControlPannal;

        bool running = false;

        #endregion
        public GameOfLife() => Initialize += () =>
        {
            maxCelsInX = 620; // moet deelbaar door 10 zijn
            maxCelsInY = 400; // moet deelbaar door 10 zijn
            quadrantsInX = maxCelsInX / 10;
            quadrantsInY = maxCelsInY / 10;
            widthControlPannal = 210;
            celSize = 2;

            ClientSize = new Size((celSize * maxCelsInX) + widthControlPannal + 1, (celSize * maxCelsInY) + 1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Linen;
            MaximizeBox = false;
            //TopMost = true;
            
            GameObjects.UseDrawFunctions = true;
            GameObjects.RenderMode = SmoothingMode.HighSpeed;
            GameObjects.LoopContainer.BackColor = Color.FromArgb(10,10,10);
            GameObjects.LoopContainer.Bounds =
                new Rectangle(widthControlPannal, 0, (celSize * maxCelsInX) + 1, (celSize * maxCelsInY) + 1);

            oldCels = new int[maxCelsInX, maxCelsInY];
            newCels = new int[maxCelsInX, maxCelsInY];
            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];

            DrawCelControles(5, 5);
            CelControles(5, 250);
            QuadrantsControles(5, 385);

            Paint += (object sender, PaintEventArgs e) => ControleDraw?.Invoke(sender, e);
            GameCycle += DrawLoop;
        };

        private void CelControles(int x, int y)
        {
            CreateButton(ref btnPlayPauze, "Play", FlatStyle.System, new Rectangle(x + 20, y + 25, 78, 25), new btnAction(() => {
                running = running == true ? false : true;
                btnPlayPauze.Text = running == true ? "Pauze" : "Play";
            }));
            CreateButton(ref btnReset, "Clear", FlatStyle.System, new Rectangle(x + 104, y + 25, 76, 25), new btnAction(() => {
                newCels = new int[maxCelsInX, maxCelsInY];
            }));

            CreateColorDialog(ref cdgCelColor, "Cel inner", FlatStyle.System, new Rectangle(x + 20, y + 55, 130, 25), new colorPicker_Ok_Action(() => Refresh()));         
            CreateColorDialog(ref cdgCelStrokeColor, "Cel stroke", FlatStyle.System, new Rectangle(x + 20, y + 85, 130, 25), new colorPicker_Ok_Action(() => Refresh()));
         
          
            ControleDraw += (object sender, PaintEventArgs e) => {
                e.Graphics.DrawString("Cel Properties", new Font("", 10), Brushes.Red, x, y);

                e.Graphics.FillRectangle(new SolidBrush(cdgCelColor.Color), new Rectangle(x + 155, y + 55, 23, 24));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 155, y + 55, 23, 24));

                e.Graphics.FillRectangle(new SolidBrush(cdgCelStrokeColor.Color), new Rectangle(x + 155, y + 85, 23, 24));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 155, y + 85, 23, 24));
            };
        }
        private void QuadrantsControles(int x, int y)
        {
            CreateCheckBox(ref cbQuadrantsUse, true, "Quadrant rendering", Appearance.Normal, new Rectangle(x +20, y + 25, 155, 25), null);           
            CreateCheckBox(ref cbDrawQuadrants, true, "Draw Quadrant", Appearance.Normal, new Rectangle(x + 20, y + 50, 155, 25), null);
            CreateCheckBox(ref cbDrawQuadrantsInfo, false, "Draw Quadrant info", Appearance.Normal, new Rectangle(x + 20, y + 75, 155, 25), null);

            CreateColorDialog(ref cdgColorQuadrantBorder, "QuadrantBorder", FlatStyle.System, new Rectangle(x + 20, y + 105, 130, 25), new colorPicker_Ok_Action(() => Refresh()));
            CreateColorDialog(ref cdgColorSubQuadrantBorder, "Sub quadrantBorder", FlatStyle.System, new Rectangle(x + 20, y + 135, 130, 25), new colorPicker_Ok_Action(() => Refresh()));

            ControleDraw += (object sender, PaintEventArgs e) => {

                e.Graphics.DrawString("Quadrant Properties", new Font("", 10), Brushes.Red, x, y);
           
                e.Graphics.FillRectangle(new SolidBrush(cdgColorQuadrantBorder.Color), new Rectangle(x + 155, y + 105, 23, 24));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 155, y + 105, 23, 24));

                e.Graphics.FillRectangle(new SolidBrush(cdgColorSubQuadrantBorder.Color), new Rectangle(x + 155, y + 135, 23, 24));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 155, y + 135, 23, 24));
            };
        }
        private void DrawCelControles(int x, int y)
        {

            CreateButton(ref btnSpawn, "Spawn", FlatStyle.System, new Rectangle(x + 20, y + 25, 50, 22), new btnAction(() => {
                GlitterGun(rand.Next(0, maxCelsInX - 35), rand.Next(0, maxCelsInY - 23));
            }));
            CreateComboBox(ref cmbPaterens, new Rectangle(x + 20, y + 55, 160, 25), Enum.GetValues(typeof(Paterens)), new SelectionChangedAction(()=> Refresh())); 

            ControleDraw += (object sender, PaintEventArgs e) => {
                e.Graphics.DrawString("Draw / Spawn Options ", new Font("", 10), Brushes.Red, x, y);
            };
        }
     


        private void DrawLoop(object sender, PaintEventArgs e)
        {
            MakeCels();
            if(cbQuadrantsUse.Checked) MakeQuadrants(cbDrawQuadrants.Checked, cbDrawQuadrantsInfo.Checked);                  
            if(running) UpdateCels(cbQuadrantsUse.Checked);          
            DrawScene(e);

        }


        private void MakeCels()
        {
            celDraw.Clear();

            for (int x = 0; x < maxCelsInX; x++)
                for (int y = 0; y < maxCelsInY; y++)
                    if (newCels[x, y] == 1)
                    {
                        celDraw.Add(new Rectangle(x * celSize, y * celSize, celSize, celSize));
                        quadrants[x / 10, y / 10]++;                  
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
                { 
                    for (int qwadY = 0; qwadY < quadrantsInY; qwadY++)
                    {
                        if (quadrants[qwadX, qwadY] != 0 || subQuadrants[qwadX, qwadY] != 0)
                        { 
                            for (int celXindex = 0; celXindex < 10; celXindex++)
                            {
                                for (int celYindex = 0; celYindex < 10; celYindex++)
                                {
                                    int x = (qwadX * 10) + celXindex;
                                    int y = (qwadY * 10) + celYindex;

                                    if (oldCels[x, y] == 1)
                                    {
                                        if (Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3) { newCels[x, y] = 1; }
                                        if (Livingneighbors(x, y) > 4 || Livingneighbors(x, y) < 2) { newCels[x, y] = 0; }
                                    }
                                    else
                                    {
                                        if (Livingneighbors(x, y) == 3) { newCels[x, y] = 1; }
                                    }
                                }
                            }
                        }
                    }
                }               
            }
            else { UpdateCels_NOQuadrants(); }
                      
        }
        private void UpdateCels_NOQuadrants()
        {            
            for (int x = 0; x < maxCelsInX; x++)
            {
                for (int y = 0; y < maxCelsInY; y++)
                {
                    if (oldCels[x, y] == 1 && (Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3)) { newCels[x, y] = 1; }
                    if (oldCels[x, y] == 1 && Livingneighbors(x, y) > 4 || oldCels[x, y] == 1 && Livingneighbors(x, y) < 2) { newCels[x, y] = 0; }
                    if (oldCels[x, y] == 0 && Livingneighbors(x, y) == 3) { newCels[x, y] = 1; }
                }
            }                                         
        }
        private void DrawScene(PaintEventArgs e)
        {
            if (cbQuadrantsUse.Checked)
                if (quadDraw.Count != 0)
                {
                    e.Graphics.DrawRectangles(new Pen(cdgColorSubQuadrantBorder.Color, 1), supQaudDraw.ToArray());
                    e.Graphics.DrawRectangles(new Pen(cdgColorQuadrantBorder.Color, 1), quadDraw.ToArray());
                }

            if (celDraw.Count != 0)
            {
                e.Graphics.FillRectangles(new SolidBrush(cdgCelColor.Color), celDraw.ToArray());
                e.Graphics.DrawRectangles(new Pen(cdgCelStrokeColor.Color, 1), celDraw.ToArray());
            }

            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];
        }


        private void MakeQuadrants(bool draw,bool info)
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
                                DrawText($"{quadrants[x, y]}", new Font("", celSize * 3), Color.White, new Vector2(x * (10 * celSize)+2, y * (10 * celSize)+2), null);

                            quadDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));
                            MarkSubQuadrants(x, y);
                        }
                    }

                for (int x = 0; x < quadrantsInX; x++)
                    for (int y = 0; y < quadrantsInY; y++)
                    {
                        if (subQuadrants[x, y] != 0)
                        {
                            if(info)
                                DrawText($"\n{subQuadrants[x, y]}", new Font("", celSize * 3), Color.White, new Vector2(x * (10 * celSize) + 2, y * (10 * celSize)+2), null);

                            supQaudDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));
                        }
                    }
            }
            else
            {
                for (int x = 0; x < quadrantsInX; x++)
                    for (int y = 0; y < quadrantsInY; y++)
                        if (quadrants[x, y] != 0)
                            MarkSubQuadrants(x, y);
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
        private void GlitterGun(int x, int y)
        {
            newCels[2 + x, 2 + y] = 1; 
            newCels[3 + x, 2 + y] = 1; 
            newCels[2 + x, 3 + y] = 1; 
            newCels[3 + x, 3 + y] = 1;
            newCels[9 + x, 2 + y] = 1; 
            newCels[10 + x, 2 + y] = 1;
            newCels[10 + x, 3 + y] = 1; 
            newCels[9 + x, 3 + y] = 1;
            newCels[6 + x, 6 + y] = 1; 
            newCels[6 + x, 5 + y] = 1; 
            newCels[7 + x, 5 + y] = 1; 
            newCels[7 + x, 6 + y] = 1;
            newCels[33 + x, 12 + y] = 1; 
            newCels[33 + x, 13 + y] = 1; 
            newCels[34 + x, 12 + y] = 1; 
            newCels[34 + x, 13 + y] = 1;
            newCels[22 + x, 19 + y] = 1; 
            newCels[22 + x, 20 + y] = 1; 
            newCels[23 + x, 19 + y] = 1; 
            newCels[23 + x, 21 + y] = 1;
            newCels[24 + x, 21 + y] = 1; 
            newCels[25 + x, 21 + y] = 1; 
            newCels[25 + x, 22 + y] = 1; 
            newCels[24 + x, 11 + y] = 1;
            newCels[25 + x, 11 + y] = 1; 
            newCels[27 + x, 11 + y] = 1; 
            newCels[28 + x, 11 + y] = 1; 
            newCels[23 + x, 12 + y] = 1;
            newCels[29 + x, 12 + y] = 1; 
            newCels[23 + x, 13 + y] = 1; 
            newCels[30 + x, 13 + y] = 1; 
            newCels[23 + x, 14 + y] = 1;
            newCels[24 + x, 14 + y] = 1; 
            newCels[25 + x, 14 + y] = 1; 
            newCels[29 + x, 14 + y] = 1; 
            newCels[28 + x, 15 + y] = 1;
        }

    }
}
