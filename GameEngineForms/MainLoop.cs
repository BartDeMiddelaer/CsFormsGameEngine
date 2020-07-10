using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Resources.DynamicResources;
using System.Drawing.Drawing2D;
using System.Numerics;
using GameEngineForms.Forms;

namespace GameEngineForms
{
    static class MainLoop
    {
        static readonly Stopwatch fpsDisplyInterval = new Stopwatch();

        [STAThread] static void Main()
       {       
            fpsDisplyInterval.Start();
            if(GameObjects.FormToRun != null)
            GameObjects.FormToRun.HandleCreated += FormToRun_HandleCreated;
            new Lodescreen().ShowDialog();
        }

        public static void FormToRun_HandleCreated(object sender, EventArgs e)
        {                       
            GameObjects.DrawContainer.Paint += new PaintEventHandler((object sender, PaintEventArgs e) => Render(sender, e));
            Application.Idle += (object sender, EventArgs e) => {
                while (IdelTiming.IsApplicationIdle())
                {
                    Update();
                    CollisionDetection();

                    if (fpsDisplyInterval.ElapsedMilliseconds > 100)
                    {
                        GameObjects.FormToRun.Text =
                        $"ScreenSize: {GameObjects.DrawContainer.Width} x {GameObjects.DrawContainer.Height}" +
                        $"   Objects: {GameObjects.ObjectCount}" +
                        $"   FrameTiming: {IdelTiming.GetFps()} ";

                        fpsDisplyInterval.Restart();
                    }

                    InvokeDestructor();
                }
            };
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
        private static void Update()
        {
            GameObjects.DrawContainer.Refresh();
            GameObjects.FormToRun.Controls.Add(GameObjects.DrawContainer);
            Interlocked.Increment(ref IdelTiming.GetframeCount());
        }
        private static void Render(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = GameObjects.RenderMode;

            InvokeDraw(sender,e);

            GameObjects.LineGeometry.ForEach(l => {

                e.Graphics.DrawLine(new Pen(l.StrokeColor,l.Thickness),
                    new Point((int)Math.Round(l.StartPoint.X), (int)Math.Round(l.StartPoint.Y)),
                    new Point((int)Math.Round(l.EndPoint.X), (int)Math.Round(l.EndPoint.Y)));
                GameObjects.ObjectCount++;
            });

            GameObjects.RectGeometry.ForEach(r => {

                var path = RoundedRect(new Rectangle(
                    new Point(
                        (int)Math.Round(r.StartPoint.X), 
                        (int)Math.Round(r.StartPoint.Y)), 
                    new Size(r.Width, r.Height)), r.CornerRadius ?? 0);

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

                    GameObjects.ObjectCount++;
                }
                else
                {
                    if (r.FillColor != null)
                        e.Graphics.FillRectangle(new SolidBrush(r.FillColor ?? Color.Transparent), 
                            new Rectangle(
                                new Point(
                                    (int)Math.Round(r.StartPoint.X), 
                                    (int)Math.Round(r.StartPoint.Y)),
                                new Size(r.Width, r.Height)));

                    if (r.StrokeColor != null)
                        e.Graphics.DrawRectangle(new Pen(r.FillColor ?? Color.Transparent), 
                            new Rectangle(
                                new Point(
                                    (int)Math.Round(r.StartPoint.X), 
                                    (int)Math.Round(r.StartPoint.Y)), 
                                new Size(r.Width,r.Height)));

                    GameObjects.ObjectCount++;

                }

                e.Graphics.ResetTransform();

            });

            GameObjects.CircleGeometry.ForEach(c => {

                if (c.FillColor != null)
                    e.Graphics.FillEllipse(
                    new SolidBrush(c.FillColor ?? Color.Transparent),
                    new Rectangle(
                        new Point(
                            (int)Math.Round(c.CenterPoint.X - c.Radius), 
                            (int)Math.Round(c.CenterPoint.Y - c.Radius)),
                        new Size((int)Math.Round(c.Radius * 2), (int)Math.Round(c.Radius * 2))));

                if (c.StrokeColor != null || c.StrokeThickness != 0)
                    e.Graphics.DrawEllipse(
                    new Pen(c.StrokeColor ?? Color.Transparent, c.StrokeThickness ?? 0),
                    new Rectangle(
                        new Point(
                            (int)Math.Round(c.CenterPoint.X - c.Radius), 
                            (int)Math.Round(c.CenterPoint.Y - c.Radius)),
                        new Size((int)Math.Round(c.Radius * 2), (int)Math.Round(c.Radius * 2))));


                e.Graphics.ResetTransform();
                GameObjects.ObjectCount++;

            });

            GameObjects.EllipseGeometry.ForEach(el => {

                if (el.Angle != null || el.Angle != 0)
                {
                    float moveX = el.Width / 2f + (el.CenterPoint.X - (el.Width / 2));
                    float moveY = el.Height / 2f + (el.CenterPoint.Y - (el.Height / 2));

                    e.Graphics.TranslateTransform(moveX, moveY);
                    e.Graphics.RotateTransform(el.Angle ?? 0);
                    e.Graphics.TranslateTransform(-moveX, -moveY);
                }


                if (el.FillColor != null)
                    e.Graphics.FillEllipse(
                    new SolidBrush(el.FillColor ?? Color.Transparent),
                    new Rectangle(
                        new Point(
                            (int)Math.Round(el.CenterPoint.X - el.Width / 2), 
                            (int)Math.Round(el.CenterPoint.Y - el.Height / 2)),
                        new Size((int)Math.Round((float)el.Width), (int)Math.Round((float)el.Height))));

                if (el.StrokeColor != null || el.StrokeThickness != 0)
                    e.Graphics.DrawEllipse(
                    new Pen(el.StrokeColor ?? Color.Transparent, el.StrokeThickness ?? 0),
                    new Rectangle(
                        new Point(
                            (int)Math.Round(el.CenterPoint.X - el.Width / 2), 
                            (int)Math.Round(el.CenterPoint.Y - el.Height / 2)),
                        new Size((int)Math.Round((float)el.Width), (int)Math.Round((float)el.Height))));


                e.Graphics.ResetTransform();
                GameObjects.ObjectCount++;

             });
            
            GameObjects.TextGeometry.ForEach(t => {


                if (t.Angle != null || t.Angle != 0)
                {
                    var textDym = TextRenderer.MeasureText(t.Context, t.Font ?? new Font("Arial", 10));

                    float moveX = textDym.Width / 2f + t.StartPoint.X;
                    float moveY = textDym.Height / 2f + t.StartPoint.Y;

                    e.Graphics.TranslateTransform(moveX, moveY);
                    e.Graphics.RotateTransform(t.Angle ?? 0);
                    e.Graphics.TranslateTransform(-moveX, -moveY);
                }

                e.Graphics.DrawString(t.Context, t.Font ?? new Font("Arial", 10), new SolidBrush(t.FontColor), new Point((int)Math.Round(t.StartPoint.X), (int)Math.Round(t.StartPoint.Y)));

                e.Graphics.ResetTransform();
                GameObjects.ObjectCount++;

            });

        }
        private static void CollisionDetection()
        { 
        
        }
    }


    [StructLayout(LayoutKind.Sequential)] public struct NativeMessage
    {
        public IntPtr Handle;
        public uint Message;
        public IntPtr WParameter;
        public IntPtr LParameter;
        public uint Time;
        public Point Location;
    }
    public static class IdelTiming
    {

        static DateTime lastCheckTime = DateTime.Now;
        static long frameCount = 0;

        public static ref long GetframeCount() => ref frameCount;
        public static DateTime GetlastCheckTime() => lastCheckTime;

        [DllImport("user32.dll")]
        public static extern int PeekMessage(NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
        public static bool IsApplicationIdle() => PeekMessage(new NativeMessage(), IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
               
        public static double GetFps()
        {
            double secondsElapsed = (DateTime.Now - lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref frameCount, 0);
            double fps = count / secondsElapsed;
            lastCheckTime = DateTime.Now;
            return Math.Round(fps, 2);
        }
    }
}
