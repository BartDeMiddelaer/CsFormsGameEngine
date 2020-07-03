using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static GameEngineForms.Resources.ResourcesDeclaration;


namespace GameEngineForms.Services
{
    public static class MathServices
    {
        static DateTime lastCheckTime = DateTime.Now;
        static long frameCount = 0;


        public static Point GetMousePosition()
        {
            Rectangle screenRectangle = GameObjects.FormToRun.RectangleToScreen(GameObjects.FormToRun.ClientRectangle);

            return new Point
            {
                X = Control.MousePosition.X - screenRectangle.X,
                Y = Control.MousePosition.Y - screenRectangle.Y
            };
        }
        public static float GetOnePixelPerSec()
        {
            double secondsElapsed = (DateTime.Now - lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref frameCount, 0);
            double fps = count / secondsElapsed;
            lastCheckTime = DateTime.Now;           
            double speed = 1d / fps;
            Interlocked.Increment(ref frameCount);
            return float.IsInfinity((float)speed) ? 0 : (float)speed;
        }




        // Extension Methods -------------------------------------------------------
        public static float MovePPS(this float thisF, int pps) => GetOnePixelPerSec() *pps;


        // -------------------------------------------------------------------------

    }
}
