using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static GameEngineForms.DrawServices;
using static GameEngineForms.Resources;
using System.Drawing.Drawing2D;

namespace GameEngineForms
{
    static class MainLoop
    {
        static Stopwatch fpsDisplyInterval = new Stopwatch();

        [STAThread]
        static void Main()
        {
            fpsDisplyInterval.Start();
            GetPictureBox().Dock = DockStyle.Fill;
            GetPictureBox().Paint += new PaintEventHandler((object sender, PaintEventArgs e) => Render(sender,e));
            
            Application.Idle += (object sender, EventArgs e) => {
                while (IdelTiming.IsApplicationIdle())
                {                                   
                    Interlocked.Increment(ref IdelTiming.frameCount);
                    if (fpsDisplyInterval.ElapsedMilliseconds > 50)
                    {
                        GetForm().Text =                        
                        $"ScreenSize: {GetPictureBox().Width} x {GetPictureBox().Height}" +
                        $"   FrameTiming: {IdelTiming.GetFps()} ";

                        fpsDisplyInterval.Restart();
                    }

                    Update();
                    Reset();
                }
            };
            Application.Run(GetForm());
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
        private static void Render(object sender, PaintEventArgs e)
        {        
            Draw(sender,e);

            GameObjects.LineGeometry.ForEach(l => {
                e.Graphics.DrawLine(new Pen(l.StrokeColor,l.Thickness), l.StartPoint, l.EndPoint ?? new Point());                   
            });

            GameObjects.RectGeometry.ForEach(r => {

                var path = RoundedRect(new Rectangle(r.StartPoint, new Size(r.Width, r.Height)), r.CornerRadius ?? 0);

                if (r.Angle != null || r.Angle != 0)
                {
                    float moveX = r.Width / 2f + r.StartPoint.X;
                    float moveY = r.Height / 2f + r.StartPoint.Y;

                    e.Graphics.TranslateTransform(moveX, moveY);
                    e.Graphics.RotateTransform(r.Angle ?? 0);
                    e.Graphics.TranslateTransform(-moveX, -moveY);
                }

                if (r.CornerRadius != null || r.CornerRadius != 0)
                {
                    
                    if (r.FillColor != null)
                        e.Graphics.FillPath(new SolidBrush(r.FillColor ?? Color.Transparent), path);

                    if (r.StrokeColor != null)
                        e.Graphics.DrawPath(new Pen(new SolidBrush(r.StrokeColor ?? Color.Transparent), r.StrokeThickness ?? 0), path);
                }
                else
                {
                    if (r.FillColor != null)
                        e.Graphics.FillRectangle(new SolidBrush(r.FillColor ?? Color.Transparent), new Rectangle(r.StartPoint, new Size(r.Width, r.Height)));

                    if (r.StrokeColor != null)
                        e.Graphics.DrawRectangle(new Pen(r.FillColor ?? Color.Transparent), new Rectangle(r.StartPoint, new Size(r.Width,r.Height)));
                }
                
                e.Graphics.ResetTransform();

            });

            GameObjects.CircleGeometry.ForEach(c => { 

            
            });

        }
        private static void Update()
        {          
            GetPictureBox().Refresh();
            GetForm().Controls.Add(GetPictureBox());
        }
        private static void Reset()
        {
            GameObjects.LineGeometry.Clear();
            GameObjects.RectGeometry.Clear();
            GameObjects.CircleGeometry.Clear();
        }

    }


    public static class IdelTiming
    {
        static DateTime lastCheckTime = DateTime.Now;
        public static long frameCount = 0;

        public static bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

        public static string GetFps()
        {
            double secondsElapsed = (DateTime.Now - lastCheckTime).TotalSeconds;
            long count = System.Threading.Interlocked.Exchange(ref frameCount, 0);
            double fps = count / secondsElapsed;
            lastCheckTime = DateTime.Now;
            return Math.Round(fps, 2).ToString();
        }

    }
}
