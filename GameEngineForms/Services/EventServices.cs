using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GameEngineForms.Services
{
    public static class EventServices
    {
        internal delegate void DrawDelegate(object sender, PaintEventArgs e);
        internal static event DrawDelegate GameCycle;
        public static void InvokeDraw(object sender, PaintEventArgs e) => GameCycle?.Invoke(sender, e);


        internal delegate void DestructorDelegate();
        internal static event DestructorDelegate Destructor;
        public static void InvokeDestructor() => Destructor?.Invoke();
    }
}
