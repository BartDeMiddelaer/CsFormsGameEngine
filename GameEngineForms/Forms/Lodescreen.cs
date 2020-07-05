using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources.DynamicResources;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.DrawServices;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.Threading;

namespace GameEngineForms.Forms
{
    public partial class Lodescreen : Form
    {
        PictureBox DrawContainer = new PictureBox();

        public Lodescreen()
        {
            ClientSize = new Size(800, 450);
            Opacity = 0.7;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TransparencyKey = Color.Gray;
            BackColor = Color.Gray;
            HandleCreated += Lodescreen_HandleCreated;
            Initialize += () => { Application.Run(GameObjects.FormToRun);};

        }

        private void Lodescreen_HandleCreated(object sender, EventArgs e)
        {
            DrawContainer.Dock = DockStyle.Fill;
            DrawContainer.Paint += new PaintEventHandler((object sender, PaintEventArgs e) => Render(sender, e));

            Application.Idle += (object sender, EventArgs e) => {
                while (IdelTiming.IsApplicationIdle())
                {
                    DrawContainer.Refresh();
                    Controls.Add(DrawContainer);
                }
            };
        }

        private void Render(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            // Shapes ---------------------------------------------------------------------------------
            var path = RoundedRect(new Rectangle(200, 0, Width - 200, Height), 50);
            e.Graphics.FillPath(new SolidBrush(Color.White), path);

            // Text -----------------------------------------------------------------------------------
            e.Graphics.DrawString("Game", new Font("Arial", 50), Brushes.White, 0, 50);
            e.Graphics.DrawString("Engine", new Font("Arial", 50), Brushes.Gray, 200, 50);
            e.Graphics.DrawString("Forms", new Font("Arial", 20), Brushes.Gray, 430, 85);
            e.Graphics.DrawString("Bart De Middelaer", new Font("Arial", 7), Brushes.Black, 212, 115);


            // if loding is done close still need to make the tasks it just closis for now
            Close();
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
