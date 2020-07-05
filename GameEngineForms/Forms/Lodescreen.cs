using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources.StaticResources;
using static GameEngineForms.Resources.DynamicResources;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.MainLoop;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using GameEngineForms.Resources;
using System.Diagnostics;

namespace GameEngineForms.Forms
{
    public partial class Lodescreen : Form
    {
        PictureBox DrawContainer = new PictureBox();
        Task BitmapLoading = new Task(new Action(() => {BitmapResources = new BitmapRepo().Resources;}));

        System.Windows.Forms.Timer tikker = new System.Windows.Forms.Timer();

        public Lodescreen()
        {
            ClientSize = new Size(800, 450);
            Opacity = 0.7;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TransparencyKey = Color.Gray;
            BackColor = Color.Gray;
            DrawContainer.Dock = DockStyle.Fill;
            DrawContainer.Paint += Render;
            Controls.Add(DrawContainer);

            tikker.Tick += Tikker_Tick;
            tikker.Start();
            tikker.Interval = 100;
            BitmapLoading.Start();
        }

        private void Tikker_Tick(object sender, EventArgs e)
        {
            DrawContainer.Refresh();
        }

        private void Render(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            // Shapes ---------------------------------------------------------------------------------
            var path = RoundedRect(new Rectangle(200, 0, Width - 200, Height), 50);
            e.Graphics.FillPath(new SolidBrush(Color.White), path);

            var strokePath = RoundedRect(new Rectangle(0, 20, Width, Height-40), 60);
            e.Graphics.DrawPath(new Pen (new SolidBrush(Color.White),1), strokePath);


            // Text -----------------------------------------------------------------------------------
            e.Graphics.DrawString("Game", new Font("Arial", 50), Brushes.White, 0, 50);
            e.Graphics.DrawString("Engine", new Font("Arial", 50), Brushes.Gray, 200, 50);
            e.Graphics.DrawString("Forms", new Font("Arial", 20), Brushes.Gray, 430, 85);
            e.Graphics.DrawString("Bart De Middelaer", new Font("Arial", 7), Brushes.Black, 212, 115);

            e.Graphics.DrawString($"BitmapsLoding: {BitmapLoading.Status}", new Font("Arial", 10), Brushes.Black, 210, Height - 220);


            if (BitmapLoading.IsCompleted)
            {
                Close(); Hide();
                if (!GameObjects.FormToRun.IsHandleCreated)GameObjects.FormToRun.ShowDialog();          
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
