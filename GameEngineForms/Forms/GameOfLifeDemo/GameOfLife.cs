using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using static GameEngineForms.Resources.GameEngineObjects;
using static GameEngineForms.Resources.StaticGameObjects;
using static GameEngineForms.Services.ExtensionMethods;
using static GameEngineForms.Services.GameEngineServices;
using static GameEngineForms.Services.ControlServices;

namespace GameEngineForms.Forms.GameOfLifeDemo
{
    public partial class GameOfLife : DefaultParentForm
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

        CheckBox cbQuadrantsUse, cbDrawQuadrants, cbSolidBrush;
        ComboBox cmbPaterens;
        TextBox txtbrushSize;
        Button btnPlayPauze;

        ColorDialog
            cdgCelColor = new ColorDialog { Color = Color.White },
            cdgColorQuadrantBorder = new ColorDialog { Color = Color.FromArgb(125, 50, 125) },
            cdgColorSubQuadrantBorder = new ColorDialog { Color = Color.FromArgb(50, 0, 50) };

        readonly ColorDialog
            cdgColorMousQuadrantBorder = new ColorDialog { Color = Color.FromArgb(100, 255, 255, 0) },
            cdgColorShapeQuadrantBorder = new ColorDialog { Color = Color.Green };

        readonly Random rand = new Random();
        readonly List<Rectangle> celDraw = new List<Rectangle>();
        readonly List<Rectangle> supQaudDraw = new List<Rectangle>();
        readonly List<Rectangle> quadDraw = new List<Rectangle>();
        readonly List<Rectangle> mousQuadDraw = new List<Rectangle>();
        readonly List<Rectangle> circleShapeQuadDraw = new List<Rectangle>();


        int[,] oldCels, newCels, mousQuadrants, quadrants, subQuadrants, circleShapeQuats;
        int celSize, maxCelsInX, maxCelsInY, quadrantsInX, quadrantsInY,
            widthControlPannal, maxBrushSize, lastBrushSize, StartBrushSize,
            brushAcc, celCanvasHeight, celCanvasWidth;

        bool running = true,
             drawing = false;

        static string 
            drawMesage = "Nothing spawnd",
            modeMesage = "Draw",
            brusType = "Rect";


        #endregion
 

        public override void GameAssetsLoadIn()
        {
            base.GameAssetsLoadIn();
            celSize = 2;
            celCanvasWidth = 1280;// moet deelbaar door 10 zijn
            celCanvasHeight = 800;// moet deelbaar door 10 zijn

            maxCelsInX = celCanvasWidth / celSize;
            maxCelsInY = celCanvasHeight / celSize;
            quadrantsInX = maxCelsInX / 10;
            quadrantsInY = maxCelsInY / 10;
            widthControlPannal = 210;
            maxBrushSize = 200;
            StartBrushSize = maxBrushSize / 4;
            lastBrushSize = StartBrushSize;
            brushAcc = 1;

            ClientSize = new Size((celSize * maxCelsInX) + widthControlPannal + 1, (celSize * maxCelsInY) + 1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Linen;
            MaximizeBox = false;
            //TopMost = true;
        
            GameObject.RenderMode = SmoothingMode.HighSpeed;
            GameObject.LoopContainer.BackColor = Color.FromArgb(10, 10, 10);
            GameObject.LoopContainer.Bounds =
                new Rectangle(widthControlPannal, 0, (celSize * maxCelsInX) + 1, (celSize * maxCelsInY) + 1);

            oldCels = new int[maxCelsInX, maxCelsInY];
            newCels = new int[maxCelsInX, maxCelsInY];
            quadrants = new int[quadrantsInX, quadrantsInY];
            subQuadrants = new int[quadrantsInX, quadrantsInY];
            mousQuadrants = new int[quadrantsInX, quadrantsInY];
            circleShapeQuats = new int[quadrantsInX, quadrantsInY];

            CelControles(5, 5);
            DrawCelControles(5, 155);
            QuadrantsControles(5, 570);

              
            MouseWheel += BrushResizer;
            GameObject.LoopContainer.MouseDown += (object sender, MouseEventArgs e) => drawing = true;
            GameObject.LoopContainer.MouseUp += (object sender, MouseEventArgs e) => drawing = false;
        }

        private void UpdateCels()
        {
            // cuda
            //https://www.youtube.com/watch?v=2EbHSCvGFM0
            //https://www.youtube.com/watch?v=kzXjRFL-gjo
            //https://www.youtube.com/watch?v=G5-iI1ogDW4
            //https://www.youtube.com/watch?v=nCM_H9nLZdA

            oldCels = newCels;
            newCels = new int[maxCelsInX, maxCelsInY];

            if (cbQuadrantsUse.Checked)
           
                Parallel.For(0, quadrantsInY, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount}, QwadYLine =>
                {
                    for (int quadOnX = 0; quadOnX < quadrantsInX; quadOnX++)
                    {
                        if (quadrants[quadOnX, QwadYLine] != 0 || subQuadrants[quadOnX, QwadYLine] != 0)
                        {
                            for (int celXindex = 0; celXindex < 10; celXindex++)
                            {
                                for (int celYindex = 0; celYindex < 10; celYindex++)
                                {
                                    int x = (quadOnX * 10) + celXindex;
                                    int y = (QwadYLine * 10) + celYindex;

                                    if (oldCels[x, y] == 1)
                                    {
                                        // make Cuda
                                        newCels[x, y] = Livingneighbors(x, y) == 2 || Livingneighbors(x, y) == 3 ? 1 :newCels[x, y];
                                        newCels[x, y] = Livingneighbors(x, y) > 4 || Livingneighbors(x, y) < 2 ? 0 : newCels[x, y];                                   
                                    }
                                    else
                                    {
                                        newCels[x, y] = Livingneighbors(x, y) == 3 ? 1 : newCels[x, y];
                                    }
                                }
                            }
                        }
                    }
                });

            else  UpdateCels_NOQuadrants();
            
          
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

            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x - 1) + maxCelsInX) % maxCelsInX, ((y + 1) + maxCelsInY) % maxCelsInY];
            tell += oldCels[((x + 1) + maxCelsInX) % maxCelsInX, ((y - 1) + maxCelsInY) % maxCelsInY];

            return tell;
        }

        private void MouseDraw()
        {                    

            int mouseX = (int)Math.Ceiling(GetMousePosition<Vector2>().X - widthControlPannal);
            int mouseY = (int)Math.Ceiling(GetMousePosition<Vector2>().Y);

            if (brusType == "Circle")
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


                                    if (Distance < lastBrushSize)
                                    newCels[x, y] = modeMesage == "Draw" ? 1 : 0;
                                }
                            }
                        }
                    }
                }
            }
            else
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

                                    if ((x * celSize) >= (mouseX - lastBrushSize) && (x * celSize) <= (mouseX + lastBrushSize) &&
                                        (y * celSize) >= (mouseY - lastBrushSize) && (y * celSize) <= (mouseY + lastBrushSize))

                                        newCels[x, y] = modeMesage == "Draw" ? 1 : 0;

                                }
                            }
                        }
                    }
                }
            }
        }

        public override void StaticPaint(object sender, PaintEventArgs e)
        {
            ControleDraw.Invoke(sender, e);
        }
        public override void GameLoop(object sender, PaintEventArgs e)
        {
            Clear();
            MakeCelsAndQuadrants();
            if (running) UpdateCels();
            DrawScene(e);
            if (drawing) MouseDraw();

            // ShapeDraw();


        }
        private void MakeCelsAndQuadrants()
        {          
            for (int x = 0; x < maxCelsInX; x++)
                for (int y = 0; y < maxCelsInY; y++)
                    if (newCels[x, y] == 1)
                    {                     
                        celDraw.Add(new Rectangle(x * celSize, y * celSize, celSize, celSize));
                        quadrants[x / 10, y / 10]++;
                    }

            for (int x = 0; x < quadrantsInX; x++)
                for (int y = 0; y < quadrantsInY; y++)
                {
                    if (quadrants[x, y] != 0) MarkSubQuadrants(x, y);
                }

            MakeMouseQuadrants();

            GameObject.ObjectCount = celDraw.Count;
        }
        private void MakeMouseQuadrants()
        {
            int mouseX = (int)Math.Ceiling(GetMousePosition<Vector2>().X - widthControlPannal);
            int mouseY = (int)Math.Ceiling(GetMousePosition<Vector2>().Y);

            if (brusType == "Circle")
            {
                for (int r = 0; r < lastBrushSize; r+=10)
                {
                    for (int i = 0; i < 360; i += brushAcc)
                    {

                        Vector2 cheakPoint = GetPointFromPoint(new Vector2(mouseX, mouseY), lastBrushSize-r, i);
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
        private void DrawScene(PaintEventArgs e)
        {
            CreateRectDrawQuadrants();
            Rectangle[] defaultRectArry = new Rectangle[] { Rectangle.Empty };
            
            if (cbDrawQuadrants.Checked)
            {    
                e.Graphics.FillRectangles(new SolidBrush(cdgColorSubQuadrantBorder.Color), supQaudDraw.Count != 0 && cbQuadrantsUse.Checked
                    ? supQaudDraw.ToArray()
                    : defaultRectArry);

                e.Graphics.FillRectangles(new SolidBrush(cdgColorQuadrantBorder.Color), quadDraw.Count != 0 && cbQuadrantsUse.Checked
                    ? quadDraw.ToArray()
                    : defaultRectArry);
           
                e.Graphics.FillRectangles(new SolidBrush(cdgColorMousQuadrantBorder.Color), mousQuadDraw.Count != 0 && cbQuadrantsUse.Checked
                    ? mousQuadDraw.ToArray()
                    : defaultRectArry);

                e.Graphics.FillRectangles(new SolidBrush(cdgColorShapeQuadrantBorder.Color), circleShapeQuadDraw.Count != 0 && cbQuadrantsUse.Checked
                    ? circleShapeQuadDraw.ToArray()
                    : defaultRectArry);
            }
            
            e.Graphics.FillRectangles(new SolidBrush(cdgCelColor.Color), celDraw.Count != 0
                    ? celDraw.ToArray()
                    : defaultRectArry);

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
        private void CreateRectDrawQuadrants()
        {                 
            for (int x = 0; x < quadrantsInX; x++)
                for (int y = 0; y < quadrantsInY; y++) 
                {
                    quadDraw.Add(quadrants[x, y] != 0
                        ? new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize))
                        : Rectangle.Empty);

                    supQaudDraw.Add(subQuadrants[x, y] != 0
                       ? new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize))
                       : Rectangle.Empty);

                    mousQuadDraw.Add(mousQuadrants[x, y] != 0
                       ? new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize))
                       : Rectangle.Empty);

                    circleShapeQuadDraw.Add(circleShapeQuats[x, y] != 0
                       ? new Rectangle(x * (10 * celSize), y * (10 * celSize), (10 * celSize), (10 * celSize))
                       : Rectangle.Empty);                  
                }
        }

        private void CelControles(int x, int y)
        {
            CreateButton(ref btnPlayPauze, "Pauze", FlatStyle.System, new Rectangle(x + 20, y + 25, 78, 25), new btnAction(() => {
                running = running == true ? false : true;
                btnPlayPauze.Text = running == true ? "Pauze" : "Play";
            }));
            CreateButton("Clear", FlatStyle.System, new Rectangle(x + 104, y + 25, 76, 25), new btnAction(() => {

                newCels = new int[maxCelsInX, maxCelsInY];
                Clear();
            }));
            CreateColorDialog(ref cdgCelColor, "Cel inner", FlatStyle.System, new Rectangle(x + 20, y + 55, 130, 25), new colorPicker_Ok_Action(() => Refresh()));

            CreateButton("1", FlatStyle.System, new Rectangle(x + 20, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 1;
                maxCelsInX = celCanvasWidth / celSize;
                maxCelsInY = celCanvasHeight / celSize;
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("2", FlatStyle.System, new Rectangle(x + 61, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 2;
                maxCelsInX = celCanvasWidth / celSize;
                maxCelsInY = celCanvasHeight / celSize;
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("4", FlatStyle.System, new Rectangle(x + 104, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 4;
                maxCelsInX = celCanvasWidth / celSize;
                maxCelsInY = celCanvasHeight / celSize;
                quadrantsInX = maxCelsInX / 10;
                quadrantsInY = maxCelsInY / 10;
                newCels = new int[maxCelsInX, maxCelsInY];
                Refresh();
            }));
            CreateButton("8", FlatStyle.System, new Rectangle(x + 145, y + 115, 30, 25), new btnAction(() => {

                Clear();
                celSize = 8;
                maxCelsInX = celCanvasWidth / celSize;
                maxCelsInY = celCanvasHeight / celSize;
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
            CreateCheckBox(ref cbQuadrantsUse, true, "Quadrant rendering", Appearance.Normal, new Rectangle(x + 20, y + 25, 155, 25), null);
            CreateCheckBox(ref cbDrawQuadrants, false, "Draw Quadrant", Appearance.Normal, new Rectangle(x + 20, y + 50, 155, 25), null);

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

            CreateComboBox(ref cmbPaterens, new Rectangle(x + 20, y + 25, 160, 25), Enum.GetValues(typeof(Paterens)));
            CreateButton("Spawn", FlatStyle.System, new Rectangle(x + 20, y + 60, 50, 25), new btnAction(() => {

                GlitterGun(rand.Next(0, maxCelsInX - 35), rand.Next(0, maxCelsInY - 23));
                drawMesage = cmbPaterens.SelectedItem.ToString();
                Refresh();
            }));
            CreateTextBox(ref txtbrushSize, new Point(x + 20, y + 120), 40, "" + StartBrushSize, 3, BorderStyle.Fixed3D, new TextChangedAction(() => Refresh()));
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



                Vector2 centerPoint = new Vector2().Intersection_LineToLine(new Vector2(x + 69, y + 150), new Vector2(x + 69, y + 249), new Vector2(x + 20, y + 199), new Vector2(x + 119, y + 199));

                if (brusType == "Circle")
                {
                    if (cbSolidBrush.Checked)
                    {
                        e.Graphics.FillEllipse(Brushes.Green,
                           centerPoint.X - (lastBrushSize / 4),
                           centerPoint.Y - (lastBrushSize / 4),
                           lastBrushSize / 2,
                           lastBrushSize / 2);
                    }
                    e.Graphics.DrawEllipse(Pens.Black,
                        centerPoint.X - (lastBrushSize / 4),
                        centerPoint.Y - (lastBrushSize / 4),
                        lastBrushSize / 2,
                        lastBrushSize / 2);

                }
                else
                {
                    if (cbSolidBrush.Checked)
                    {
                        e.Graphics.FillRectangle(Brushes.Green,
                           centerPoint.X - (lastBrushSize / 4),
                           centerPoint.Y - (lastBrushSize / 4),
                           lastBrushSize / 2,
                           lastBrushSize / 2);
                    }
                    e.Graphics.DrawRectangle(Pens.Black,
                        centerPoint.X - (lastBrushSize / 4),
                        centerPoint.Y - (lastBrushSize / 4),
                        lastBrushSize / 2,
                        lastBrushSize / 2);
                }

                e.Graphics.DrawLine(Pens.Black, new Point(x + 70, y + 150), new Point(x + 70, y + 249));
                e.Graphics.DrawLine(Pens.Black, new Point(x + 20, y + 200), new Point(x + 119, y + 200));
            };
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
