using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources.ResourcesDeclaration;


namespace GameEngineForms.Services
{
    public static class MathServices
    {
        public static Point GetMousePosition()
        {
            Rectangle screenRectangle = GameObjects.FormToRun.RectangleToScreen(GameObjects.FormToRun.ClientRectangle);

            return new Point
            {
                X = Control.MousePosition.X - screenRectangle.X,
                Y = Control.MousePosition.Y - screenRectangle.Y
            };
        }

    }
}
