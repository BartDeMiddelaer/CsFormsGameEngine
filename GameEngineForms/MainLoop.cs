using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static GameEngineForms.Services.ControlServices;
using static GameEngineForms.Resources.GameEngineObjects;
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
            GameObject.LoopContainer.Paint += new PaintEventHandler((object sender, PaintEventArgs e) => 
            {
                e.Graphics.SmoothingMode = GameObject.RenderMode;
                InvokeGameCycle(sender, e);
            });
            
            Application.Idle += (object sender, EventArgs e) => 
            {
                while (IdelTiming.IsApplicationIdle())
                {
                    GameObject.LoopContainer.Refresh();
                    Interlocked.Increment(ref IdelTiming.GetframeCount());

                    if (fpsDisplyInterval.ElapsedMilliseconds > 100)
                    {
                        GameObject.GameToRun.Text =
                        $"ScreenSize: {GameObject.LoopContainer.Width} x {GameObject.LoopContainer.Height}" +
                        $"   Objects: {GameObject.ObjectCount}" +
                        $"   FrameTiming: {IdelTiming.GetFps()} ";

                        fpsDisplyInterval.Restart();
                    }
                }
            };
            
            fpsDisplyInterval.Start();
            new Lodescreen().ShowDialog();
        }       
    }
    
    public static class IdelTiming
    {

        static DateTime lastCheckTime = DateTime.Now;
        static long frameCount = 0;

        public static ref long GetframeCount() => ref frameCount;
        public static DateTime GetlastCheckTime() => lastCheckTime;

        [DllImport("user32.dll")]
        internal static extern int PeekMessage(NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
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
    [StructLayout(LayoutKind.Sequential)] public struct NativeMessage
    {
        public IntPtr Handle;
        public uint Message;
        public IntPtr WParameter;
        public IntPtr LParameter;
        public uint Time;
        public Point Location;
    }
}
