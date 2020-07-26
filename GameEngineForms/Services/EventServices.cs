using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources.DynamicResources;

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


        public delegate void btnAction();
        public delegate void colorPicker_Ok_Action();
        public delegate void CheckedChangedAction();

        public static void CreateButton(ref Button btn, string text, FlatStyle style, Rectangle location, btnAction action)
        {
            btn = new Button
            {
                Bounds = location,
                Text = text,
                FlatStyle = style
            };

            GameObjects.FormToRun.Controls.Add(btn);
            btn.Click += (object sender, EventArgs e) => action();
        }
        public static void CreateColorDialog(ref ColorDialog cdg, string text, FlatStyle style, Rectangle location, colorPicker_Ok_Action action)
        {
            ColorDialog tempCdg = cdg ?? new ColorDialog();

            Button btnColorPicker = new Button { 
                Text = text,
                FlatStyle = style,
                Bounds = location
            };
            GameObjects.FormToRun.Controls.Add(btnColorPicker);

            btnColorPicker.Click += (object sender, EventArgs e) =>
            {
                if (tempCdg.ShowDialog() == DialogResult.OK)
                {
                    action();
                }
            };

            cdg = tempCdg;
        }
        public static void CreateCheckBox(ref CheckBox cb,bool isChecked, string text, Appearance appearance, Rectangle location, CheckedChangedAction action)
        {
            cb = new CheckBox {
                Text = text,
                Appearance = appearance,
                Bounds = location,
                Checked = isChecked
            };

            GameObjects.FormToRun.Controls.Add(cb);
            cb.CheckedChanged += (object sender, EventArgs e) => action();
        }     
    }
}
