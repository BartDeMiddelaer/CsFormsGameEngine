using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GameEngineForms.Services
{
    public static class EventServices
    {
        public static void InvokeDraw(object sender, PaintEventArgs e) => GameCycle?.Invoke(sender, e);
        internal delegate void DrawDelegate(object sender, PaintEventArgs e);
        internal static event DrawDelegate GameCycle;

        public static void InvokeInitialize() => Initialize?.Invoke();
        internal delegate void InitializeDelegate();
        internal static event InitializeDelegate Initialize;

        public static void InvokeDestructor() => Destructor?.Invoke();
        internal delegate void DestructorDelegate();
        internal static event DestructorDelegate Destructor;

        public static void DoOnIntervalPerSec(DoOnIntervalDelegate del, int perSecond)
        {
            del.Invoke();
        }
        public delegate void DoOnIntervalDelegate();
    }
}
