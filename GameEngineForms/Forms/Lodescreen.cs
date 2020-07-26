using System;
using System.Drawing;
using System.Windows.Forms;
using static GameEngineForms.Resources.StaticResources;
using static GameEngineForms.Resources.DynamicResources;
using static GameEngineForms.Services.EventServices;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using GameEngineForms.Resources;
using Timer = System.Windows.Forms.Timer;
using System.Collections.Generic;
using GameEngineForms.Services;

namespace GameEngineForms.Forms
{
    public partial class Lodescreen : Form
    {
        readonly PictureBox LoopContainer = new PictureBox();
        readonly Task gameFormLoading = new Task(InvokeInitialize);
        readonly Task showMinimumTime = new Task(() =>  Thread.Sleep(GameObjects.MinimumLodeScreenTime));
        readonly Task bitmapLoading = new Task(() => BitmapResources = new BitmapRepo().Resources);
        readonly Task soundLoading = new Task(() => SoundResources = new SoundRepo().Resources);
        readonly Timer tikker = new Timer();

        public Lodescreen()
        {
            ClientSize = new Size(800, 450);
            Opacity = 0.7;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TransparencyKey = Color.Gray;
            BackColor = Color.Gray;
            LoopContainer.Dock = DockStyle.Fill;
            LoopContainer.Paint += Render;
            Controls.Add(LoopContainer);

            tikker.Tick += (object sender, EventArgs e) => LoopContainer.Refresh(); 
            tikker.Start();
            tikker.Interval = 10;
            gameFormLoading.Start();
            showMinimumTime.Start();
            bitmapLoading.Start();
            soundLoading.Start();

        }

        private void Render(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            #region // anemations ------------------------------

                int teller = 2;
                var path = RoundedRect(new Rectangle(200, 0, Width - 200, Height), 50);
                var strokePath = RoundedRect(new Rectangle(2, 20, Width, Height-40), 60);
            
                e.Graphics.FillPath(new SolidBrush(Color.White), path);
                e.Graphics.DrawPath(new Pen (new SolidBrush(Color.White),5), strokePath);

                for (int x = 0; x < 50; x++)
                {
                    var diamiter = new Random().Next(0, 20);
                    e.Graphics.DrawEllipse(new Pen(Color.Gray, 2), new Rectangle(
                        new Random().Next(200,Width),
                        new Random().Next(350, Height),
                        diamiter, diamiter));

               
                }              
                for (int b = 0; b < 30; b+= 5)
                {    
                    var bubbelBorder = new GraphicsPath();

                    for (int x = 0; x < 101; x++)
                        bubbelBorder.AddLine(
                            190+ (x * 10), 
                            (new Random().Next(300 + b, 302 + b)) + (b* teller) /3,
                            190 + (x * 10), 
                            (new Random().Next(300 + b, 302 + b)) + (b* teller) /3
                        );
                
                    e.Graphics.DrawPath(new Pen(Color.Gray, teller), bubbelBorder);
                    teller++;
                }
            #endregion

            // Text ------------------------------------
            e.Graphics.DrawString("Zero -", new Font("Arial", 50), Brushes.White, 0, 50);
            e.Graphics.DrawString("Point", new Font("Arial", 50), Brushes.Gray, 200, 50);
            e.Graphics.DrawString("Made in winForms", new Font("Arial", 15), Brushes.Red, 380, 92);
            e.Graphics.DrawString("Bart De Middelaer", new Font("Arial", 7), Brushes.Black, 212, 115);

            e.Graphics.DrawString(              
                $"Bitmaps Loding: {bitmapLoading.Status} \n" +
                $"Sound Loading: {soundLoading.Status} \n" +
                $"Game instance Loading: {gameFormLoading.Status}  \n" +
                $"Initialize: { showMinimumTime.Status} \n", 
                new Font("Arial", 10), Brushes.Red, 210, Height - 240);

            if (bitmapLoading.IsCompleted && showMinimumTime.IsCompleted && soundLoading.IsCompleted && gameFormLoading.IsCompleted)
            {               
                Close(); Hide();
                GameObjects.FormToRun.ShowDialog();          
            }
        }

        static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
