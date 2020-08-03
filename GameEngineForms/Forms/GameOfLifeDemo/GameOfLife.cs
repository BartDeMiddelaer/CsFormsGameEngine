﻿using System;
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
using System.Diagnostics;
using GameEngineForms.Forms.GameOfLifeDemo.Objects;
using System.Threading.Tasks;
using System.Threading;

namespace GameEngineForms.Forms.GameOfLifeDemo
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

        CheckBox cbQuadrantsUse, cbDrawQuadrants, cbDrawQuadrantsInfo, cbSolidBrush;
        ComboBox cmbPaterens;
        TextBox txtbrushSize;
        Button btnPlayPauze;

        ColorDialog 
            cdgCelColor = new ColorDialog { Color = Color.White }, 
            cdgColorQuadrantBorder = new ColorDialog { Color = Color.FromArgb(125, 50, 125) },
            cdgColorSubQuadrantBorder = new ColorDialog { Color = Color.FromArgb(50, 0, 50) },
            cdgColorMousQuadrantBorder = new ColorDialog { Color = Color.FromArgb(100, 255, 255, 0) },
            cdgColorShapeQuadrantBorder = new ColorDialog { Color = Color.Green };

        readonly Random rand = new Random();
        readonly List<Rectangle> celDraw = new List<Rectangle>();
        readonly List<Rectangle> supQaudDraw = new List<Rectangle>();
        readonly List<Rectangle> quadDraw = new List<Rectangle>();
        readonly List<Rectangle> mousQuadDraw = new List<Rectangle>();
        readonly List<Rectangle> circleShapeQuadDraw = new List<Rectangle>();

        readonly List<CircleShape> staticCircleShapes = new List<CircleShape>();

        int[,] oldCels, newCels, mousQuadrants, quadrants, subQuadrants, circleShapeQuats;
        int celSize, maxCelsInX, maxCelsInY, quadrantsInX, quadrantsInY,
            widthControlPannal, maxBrushSize, lastBrushSize, StartBrushSize,
            brushAcc, maxParallelIterations;

        bool running = false,
             drawing = false;

        string drawMesage = "Nothing spawnd",
               modeMesage = "Draw",
               brusType = "Rect";

        float angelTikker = 0;

        #endregion
        public GameOfLife() => Initialize += () =>
        {
            celSize = 2;
            maxParallelIterations = 10;
            maxCelsInX = 1040 / celSize; // moet deelbaar door 10 zijn
            maxCelsInY = 800 / celSize; // moet deelbaar door 10 zijn
            quadrantsInX = maxCelsInX / 10;
            quadrantsInY = maxCelsInY / 10;
            widthControlPannal = 210;
            maxBrushSize = 50;
            StartBrushSize = maxBrushSize;
            lastBrushSize = StartBrushSize;
            brushAcc = 1;

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
            mousQuadrants = new int[quadrantsInX, quadrantsInY];
            circleShapeQuats = new int[quadrantsInX, quadrantsInY];


            CelControles(5,5);
            DrawCelControles(5, 155);
            SolidAndPulsersControles(5,465);
            QuadrantsControles(5, 570);

            Paint += (object sender, PaintEventArgs e) => ControleDraw?.Invoke(sender, e);
            GameCycle += DrawLoop;
            MouseWheel += BrushResizer;
            GameObjects.LoopContainer.MouseDown += (object sender, MouseEventArgs e) => drawing = true;
            GameObjects.LoopContainer.MouseUp += (object sender, MouseEventArgs e) => drawing = false;

        };

        private void UpdateCels()
        {
            oldCels = newCels;
            newCels = new int[maxCelsInX, maxCelsInY];

            int partSize = quadrantsInY / maxParallelIterations;

            if (cbQuadrantsUse.Checked)
            {
                Parallel.For(0, maxParallelIterations, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount}, thread =>
                {
                    for (int qwadX = 0; qwadX < quadrantsInX; qwadX++)
                    {
                        for (int qwadY = thread * partSize; qwadY < (thread+1) * partSize; qwadY++)
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

                });
            }
            else
            {
                UpdateCels_NOQuadrants();
            }

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

        private void MouseDraw()
        {
            int mouseX = (int)Math.Ceiling(GetMousePosition().X - widthControlPannal);
            int mouseY = (int)Math.Ceiling(GetMousePosition().Y);
            
            if (cbSolidBrush.Checked)
            {
                for (int qwadX = 0; qwadX < quadrantsInX; qwadX++)
                {
                    for (int qwadY = 0; qwadY < quadrantsInY; qwadY++)
                    {
                        if (mousQuadrants[qwadX, qwadY] != 0)
                        {
                            for (int celXindex = 0; celXindex < 10; celXindex++)
                            {
                                for (int celYindex = 0; celYindex < 10; celYindex++)
                                {
                                    int x = (qwadX * 10) + celXindex;
                                    int y = (qwadY * 10) + celYindex;

                                    Vector2 celVecter = new Vector2(x * celSize, y * celSize);
                                    Vector2 brushCenter = new Vector2(mouseX, mouseY);

                                    float Distance = Vector2.Distance(celVecter, brushCenter);

                                    if (brusType == "Circle")
                                    {
                                        if (Distance < lastBrushSize)
                                            if (modeMesage == "Draw") newCels[x, y] = 1;
                                            else newCels[x, y] = 0;
                                    }
                                    else
                                    {
                                        if ((x * celSize) >= (mouseX - lastBrushSize) && (x * celSize) <= (mouseX + lastBrushSize) && 
                                            (y * celSize) >= (mouseY - lastBrushSize) && (y * celSize) <= (mouseY+ lastBrushSize))

                                        if (modeMesage == "Draw") newCels[x, y] = 1;
                                        else newCels[x, y] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
              
            }
        }

        private void CelControles(int x, int y)
        {
            CreateButton(ref btnPlayPauze, "Play", FlatStyle.System, new Rectangle(x + 20, y + 25, 78, 25), new btnAction(() => {
                running = running == true ? false : true;
                btnPlayPauze.Text = running == true ? "Pauze" : "Play";
            }));
            CreateButton("Clear", FlatStyle.System, new Rectangle(x + 104, y + 25, 76, 25), new btnAction(() => {

                newCels = new int[maxCelsInX, maxCelsInY];
                Clear();
                staticCircleShapes.Clear();

            }));
            CreateColorDialog(ref cdgCelColor, "Cel inner", FlatStyle.System, new Rectangle(x + 20, y + 55, 130, 25), new colorPicker_Ok_Action(() => Refresh()));

            CreateButton("1", FlatStyle.System, new Rectangle(x + 20, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 1;
                maxCelsInX = 1040 / celSize; // moet deelbaar door 10 zijn
                maxCelsInY = 800 / celSize; // moet deelbaar door 10 zijn
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("2", FlatStyle.System, new Rectangle(x + 61, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 2;
                maxCelsInX = 1040 / celSize; // moet deelbaar door 10 zijn
                maxCelsInY = 800 / celSize; // moet deelbaar door 10 zijn
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("4", FlatStyle.System, new Rectangle(x + 104, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 4;
                maxCelsInX = 1040 / celSize; // moet deelbaar door 10 zijn
                maxCelsInY = 800 / celSize; // moet deelbaar door 10 zijn
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("8", FlatStyle.System, new Rectangle(x + 145, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 8;
                maxCelsInX = 1040 / celSize; // moet deelbaar door 10 zijn
                maxCelsInY = 800 / celSize; // moet deelbaar door 10 zijn
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));


            ControleDraw += (object sender, PaintEventArgs e) => {
                e.Graphics.DrawString("Cel Properties", new Font("", 10), Brushes.Red, x, y);

                e.Graphics.FillRectangle(new SolidBrush(cdgCelColor.Color), new Rectangle(x + 155, y + 55, 23, 24));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 155, y + 55, 23, 24));

                e.Graphics.DrawString($"Cel size: {celSize}", new Font("", 10), Brushes.Black, x + 20, y + 90);

            };
        }
        private void QuadrantsControles(int x, int y)
        {
            CreateCheckBox(ref cbQuadrantsUse, true, "Quadrant rendering", Appearance.Normal, new Rectangle(x +20, y + 25, 155, 25), null);           
            CreateCheckBox(ref cbDrawQuadrants, false, "Draw Quadrant", Appearance.Normal, new Rectangle(x + 20, y + 50, 155, 25), null);
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
            
            CreateComboBox(ref cmbPaterens, new Rectangle(x + 20, y + 25, 160, 25), Enum.GetValues(typeof(Paterens)), null);
            CreateButton("Spawn", FlatStyle.System, new Rectangle(x + 20, y + 60, 50, 25), new btnAction(() => {

                GlitterGun(rand.Next(0, maxCelsInX - 35), rand.Next(0, maxCelsInY - 23));
                drawMesage = cmbPaterens.SelectedItem.ToString();
                Refresh();
            }));
            CreateTextBox(ref txtbrushSize, new Point(x + 20, y + 120), 40, "" + StartBrushSize, 2, BorderStyle.Fixed3D, new TextChangedAction(() => Refresh()));
            CreateButton("Draw", FlatStyle.System, new Rectangle(x + 70, y + 120, 50, 23), new btnAction(() => {
                modeMesage = "Draw";
                Refresh();
            }));
            CreateButton("Erase", FlatStyle.System, new Rectangle(x + 130, y + 120, 50, 23), new btnAction(() => {
                modeMesage = "Erase";
                Refresh();
            CreateButton("Erase", FlatStyle.System, new Rectangle(x + 130, y + 120, 50, 23), new btnAction(() => {
                modeMesage = "Erase";
                Refresh();
            }));
            }));

            CreateButton("[]", FlatStyle.System, new Rectangle(x + 130, y + 150, 50, 47), new btnAction(() => {
                brusType = "Rect";
                Refresh();
            }));
            CreateButton("O", FlatStyle.System, new Rectangle(x + 130, y + 203, 50, 47), new btnAction(() => {
                brusType = "Circle";
                Refresh();
            }));


            CreateCheckBox(ref cbSolidBrush, true, "Solid brush", Appearance.Normal, new Rectangle(x + 20, y + 260, 155, 25), new CheckedChangedAction(() => Refresh()));

            ControleDraw += (object sender, PaintEventArgs e) => {
                e.Graphics.DrawString("Draw / Spawn Options ", new Font("", 10), Brushes.Red, x, y);
                e.Graphics.DrawString($"{drawMesage}", new Font("", 10), Brushes.Black, x + 80, y + 65);
                e.Graphics.DrawString($"Mode: {modeMesage} S: {txtbrushSize.Text} Sh: {brusType}", new Font("", 10), Brushes.Green, x + 20, y + 95);

                e.Graphics.FillRectangle(Brushes.White, new Rectangle(x + 20, y + 150, 99, 99));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x + 20, y + 150, 99, 99));               

               

                Vector2 centerPoint = new Vector2().Intersection_LineToLine(new Vector2(x + 69, y + 150),new Vector2(x + 69, y + 249),new Vector2(x + 20, y + 199),new Vector2(x + 119, y + 199));

                if (brusType == "Circle")
                {
                    if (cbSolidBrush.Checked)
                    {
                        e.Graphics.FillEllipse(Brushes.Green,
                           centerPoint.X - (lastBrushSize / 2),
                           centerPoint.Y - (lastBrushSize / 2),
                           lastBrushSize,
                           lastBrushSize);
                    }
                    e.Graphics.DrawEllipse(Pens.Black, 
                        centerPoint.X - (lastBrushSize / 2), 
                        centerPoint.Y - (lastBrushSize / 2), 
                        lastBrushSize, 
                        lastBrushSize);

                }
                else
                {
                    if (cbSolidBrush.Checked)
                    {
                        e.Graphics.FillRectangle(Brushes.Green,
                           centerPoint.X - (lastBrushSize / 2),
                           centerPoint.Y - (lastBrushSize / 2),
                           lastBrushSize,
                           lastBrushSize);
                    }
                    e.Graphics.DrawRectangle(Pens.Black,
                        centerPoint.X - (lastBrushSize / 2),
                        centerPoint.Y - (lastBrushSize / 2),
                        lastBrushSize,
                        lastBrushSize);
                }

                e.Graphics.DrawLine(Pens.Black, new Point(x + 70, y + 150), new Point(x + 70, y + 249));
                e.Graphics.DrawLine(Pens.Black, new Point(x + 20, y + 200), new Point(x + 119, y + 200));
            };
        }
        private void SolidAndPulsersControles(int x, int y)
        {
            ControleDraw += (object sender, PaintEventArgs e) =>  e.Graphics.DrawString("Shapes (WIP) ", new Font("", 10), Brushes.Red, x, y);

            CreateButton("Static circle", FlatStyle.System, new Rectangle(x + 20, y + 25, 78, 25), new btnAction(() => {
                
                staticCircleShapes.Add(new CircleShape(GetPointFromPoint(new Vector2((GameObjects.LoopContainer.Width / 2), (GameObjects.LoopContainer.Height / 2)),100, angelTikker), 25));
                staticCircleShapes.Add(new CircleShape(GetPointFromPoint(GetPointFromPoint(new Vector2((GameObjects.LoopContainer.Width / 2), (GameObjects.LoopContainer.Height / 2)),10,5), 100, angelTikker), 15));
                staticCircleShapes.Add(new CircleShape((GameObjects.LoopContainer.Width / 2), (GameObjects.LoopContainer.Height / 2), 51));
            }));


            CreateButton("Static circle", FlatStyle.System, new Rectangle(x + 104, y + 25, 76, 25), new btnAction(() => {}));
            CreateButton("Puls rect", FlatStyle.System, new Rectangle(x + 20, y + 55, 78, 25), new btnAction(() => {}));
            CreateButton("Puls circle", FlatStyle.System, new Rectangle(x + 104, y + 55, 76, 25), new btnAction(() => {}));
        }


        private void DrawLoop(object sender, PaintEventArgs e)
        {
            Clear();
            MakeCelsAndQuadrants();                          
            if(running) UpdateCels(); 

            if (drawing) MouseDraw();
            ShapeDraw();

            DrawScene(e);

            if (staticCircleShapes.Count != 0)
            {

                staticCircleShapes.ToArray()[0].Center = GetPointFromPoint(GetPointFromPoint(new Vector2((GameObjects.LoopContainer.Width / 2), (GameObjects.LoopContainer.Height / 2)), 200, -angelTikker * 3), 100, angelTikker);
                staticCircleShapes.ToArray()[1].Center = GetPointFromPoint(new Vector2((GameObjects.LoopContainer.Width / 2), (GameObjects.LoopContainer.Height / 2)), 200, angelTikker);
            
            }

           angelTikker.AddValuePerSec(200);
           //angelTikker += 0.9f;

        }
        private void ShapeDraw()
        {
            int mouseX = (int)Math.Ceiling(GetMousePosition().X - widthControlPannal);
            int mouseY = (int)Math.Ceiling(GetMousePosition().Y);

            Parallel.ForEach(staticCircleShapes, shape =>
            {

                for (int qwadX = 0; qwadX < quadrantsInX; qwadX++)
                {
                    for (int qwadY = 0; qwadY < quadrantsInY; qwadY++)
                    {
                        if (circleShapeQuats[qwadX, qwadY] != 0)
                        {
                            for (int celXindex = 0; celXindex < 10; celXindex++)
                            {
                                for (int celYindex = 0; celYindex < 10; celYindex++)
                                {
                                    int x = (qwadX * 10) + celXindex;
                                    int y = (qwadY * 10) + celYindex;

                                    Vector2 celVecter = new Vector2(x * celSize, y * celSize);
                                    Vector2 shapeCenter = new Vector2(shape.Center.X, shape.Center.Y);

                                    float Distance = Vector2.Distance(celVecter, shapeCenter);        
                                    
                                    if (Distance < shape.Radius)
                                        newCels[x, y] = 1;
                                        
                                }
                            }
                        }
                    }
                }
            });
        }
        private void MakeCelsAndQuadrants()
        {          
            for (int x = 0; x < maxCelsInX; x++)
                for (int y = 0; y < maxCelsInY; y++)
                    if (newCels[x, y] == 1)
                    {
                        if(celSize > 2)                         
                                celDraw.Add(new Rectangle(x * celSize, y * celSize, celSize - 1, celSize - 1));                      
                        else
                            celDraw.Add(new Rectangle(x * celSize, y * celSize, celSize, celSize));

                        quadrants[x / 10, y / 10]++;
                    }

            for (int x = 0; x < quadrantsInX; x++)
                for (int y = 0; y < quadrantsInY; y++)
                {
                    if (quadrants[x, y] != 0) MarkSubQuadrants(x, y);
                }

            MakeMouseQuadrants();
            MakeShapeQuadrants();


            GameObjects.ObjectCount = celDraw.Count;
        }
        private void MakeMouseQuadrants()
        {
           
            int mouseX = (int)Math.Ceiling(GetMousePosition().X - widthControlPannal);
            int mouseY = (int)Math.Ceiling(GetMousePosition().Y);

            if (brusType == "Circle")
            {
                for (int r = 0; r < lastBrushSize; r+=10)
                {
                    for (int i = 0; i < 360; i += brushAcc)
                    {
                        Vector2 cheakPoint = GetPointFromPoint(new Vector2(mouseX, mouseY), lastBrushSize, i);

                        cheakPoint = GetPointFromPoint(new Vector2(mouseX, mouseY), lastBrushSize-r, i);
                        int xIndex = (int)cheakPoint.X / (celSize * 10);
                        int yIndex = (int)cheakPoint.Y / (celSize * 10);

                        if (xIndex >= 0 && xIndex <= quadrantsInX - 1 && yIndex >= 0 && yIndex <= quadrantsInY - 1)
                            mousQuadrants[xIndex, yIndex] = 1;
                    }
                }
                
            }
            else
            {
                Point  leftTop = new Point((mouseX - lastBrushSize) / (celSize * 10), (mouseY - lastBrushSize) / (celSize * 10)),
                       reghtTop = new Point((mouseX + lastBrushSize) / (celSize * 10), (mouseY - lastBrushSize) / (celSize * 10)),
                       leftBottem = new Point((mouseX - lastBrushSize) / (celSize * 10), (mouseY + lastBrushSize) / (celSize * 10));

                for (int x = leftTop.X; x <= reghtTop.X; x++)
                {
                    for (int y = leftTop.Y; y <= leftBottem.Y; y++)
                    {
                        if (x >= 0 && x <= quadrantsInX - 1 && y >= 0 && y <= quadrantsInY - 1)
                            mousQuadrants[x, y] = 1;
                    }
                }
            }

        }
        private void MakeShapeQuadrants()
        {
            foreach (var shape in staticCircleShapes)
            {          
                for (int r = 0; r < shape.Radius; r += 10)
                {
                    for (int i = 0; i < 360; i += brushAcc)
                    {
                        Vector2 cheakPoint = GetPointFromPoint(new Vector2(shape.Center.X, shape.Center.Y), shape.Radius, i);

                        cheakPoint = GetPointFromPoint(new Vector2(shape.Center.X, shape.Center.Y), shape.Radius - r, i);
                        int xIndex = (int)cheakPoint.X / (celSize * 10);
                        int yIndex = (int)cheakPoint.Y / (celSize * 10);

                        if (xIndex >= 0 && xIndex <= quadrantsInX  && yIndex >= 0 && yIndex < quadrantsInY)
                            circleShapeQuats[xIndex, yIndex] = 1;
                    }
                }
            }


            // add rect Qwads


        }



        private void DrawScene(PaintEventArgs e)
        {
            if(cbDrawQuadrants.Checked)
            DrawQuadrants(e);

            
            if (cbQuadrantsUse.Checked)
            { 
                if (quadDraw.Count != 0)
                {
                    e.Graphics.DrawRectangles(new Pen(cdgColorSubQuadrantBorder.Color, 1), supQaudDraw.ToArray());
                    e.Graphics.DrawRectangles(new Pen(cdgColorQuadrantBorder.Color, 1), quadDraw.ToArray());
                }

                if (mousQuadDraw.Count != 0)
                    e.Graphics.FillRectangles(new SolidBrush(cdgColorMousQuadrantBorder.Color), mousQuadDraw.ToArray());

                if (circleShapeQuadDraw.Count != 0)
                    e.Graphics.DrawRectangles(new Pen(cdgColorShapeQuadrantBorder.Color, 1), circleShapeQuadDraw.ToArray());
            }

          
            if (celDraw.Count != 0 && cdgCelColor.Color != Color.Black)
                e.Graphics.FillRectangles(new SolidBrush(cdgCelColor.Color), celDraw.ToArray());              
            

            // brush sape on screen
            if (brusType == "Circle")
            {
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(100, 255, 0, 0), 2),
                new Rectangle(
                    new Point(
                        (int)GetMousePosition().X - (widthControlPannal + lastBrushSize),
                        (int)GetMousePosition().Y - lastBrushSize),
                    new Size(lastBrushSize * 2, lastBrushSize * 2)));
            }
            else
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(100, 255, 0, 0), 2),
                new Rectangle(
                    new Point(
                        (int)GetMousePosition().X - (widthControlPannal + lastBrushSize),
                        (int)GetMousePosition().Y - lastBrushSize),
                    new Size(lastBrushSize * 2, lastBrushSize * 2)));
            }
        }


        public void BrushResizer(object sender, MouseEventArgs e)
        {
            if (int.TryParse(txtbrushSize.Text.ToString(), out int value))
                if (e.Delta > 0)
                {
                    value++;
                    lastBrushSize = value;
                    txtbrushSize.Text = "" + value;
                }
                else
                {
                    value--;
                    lastBrushSize = value;
                    txtbrushSize.Text = "" + value;
                }

            if (value < 1) txtbrushSize.Text = "1";
            if (value > maxBrushSize) txtbrushSize.Text = "" + maxBrushSize;

        }
        private void DrawQuadrants(PaintEventArgs e)
        {                 
            for (int x = 0; x < quadrantsInX; x++)
                for (int y = 0; y < quadrantsInY; y++)
                {
                    if (quadrants[x, y] != 0)
                    { 
                        quadDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));
                        if (cbDrawQuadrantsInfo.Checked) e.Graphics.DrawString($"{quadrants[x, y]}", new Font("", celSize * 3), Brushes.White, x * (10 * celSize) + 2, y * (10 * celSize) + 2);
                    }

                    if (subQuadrants[x, y] != 0)
                    { 
                        supQaudDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));
                        if (cbDrawQuadrantsInfo.Checked) e.Graphics.DrawString($"\n{subQuadrants[x, y]}", new Font("", celSize * 3), Brushes.White, x * (10 * celSize) + 2, y * (10 * celSize)+2);
                    }

                    if (mousQuadrants[x, y] != 0)
                        mousQuadDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));

                    if (circleShapeQuats[x, y] != 0)
                        circleShapeQuadDraw.Add(new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize)));
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

        private void Clear()
        {
            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];
            mousQuadrants = new int[quadrantsInX, quadrantsInY];
            circleShapeQuats = new int[quadrantsInX, quadrantsInY];

            quadDraw.Clear();
            supQaudDraw.Clear();
            mousQuadDraw.Clear();
            celDraw.Clear();
            circleShapeQuadDraw.Clear();
        }
    }
}
