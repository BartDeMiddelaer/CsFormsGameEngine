using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Resources.GameEngineObjects;

namespace GameEngineForms.Services
{
    public static class ControlServices
    {
        public static void InvokeGameCycle(object sender, PaintEventArgs e) => GameCycle?.Invoke(sender, e);
        internal delegate void DrawDelegate(object sender, PaintEventArgs e);
        internal static event DrawDelegate GameCycle;

        public static void InvokeAssetLoading() => AssetLoading?.Invoke();
        internal delegate void InitializeDelegate();
        internal static event InitializeDelegate AssetLoading;


        public delegate void btnAction();
        public delegate void colorPicker_Ok_Action();
        public delegate void CheckedChangedAction();
        public delegate void TextChangedAction();
        public delegate void SelectionChangedAction();


        public static void CreateButton(string text, FlatStyle style, Rectangle location, btnAction action)
        {
            Button btn = new Button
            {
                Bounds = location,
                Text = text,
                FlatStyle = style,
            };

            GameObject.GameToRun.Controls.Add(btn);
            btn.Click += (object sender, EventArgs e) => action();
        }
        public static void CreateButton(ref Button btn, string text, FlatStyle style, Rectangle location, btnAction action)
        {
            btn = new Button
            {
                Bounds = location,
                Text = text,
                FlatStyle = style,
            };

            GameObject.GameToRun.Controls.Add(btn);
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

            btnColorPicker.Click += (object sender, EventArgs e) => { if (tempCdg.ShowDialog() == DialogResult.OK) action(); };
            GameObject.GameToRun.Controls.Add(btnColorPicker);
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

            if (appearance == Appearance.Button) cb.BackColor = Color.FromArgb(240,240,240);

            if (action != null)
                cb.CheckedChanged += (object sender, EventArgs e) => action();

            GameObject.GameToRun.Controls.Add(cb);
        }
        public static void CreateTextBox(ref TextBox txt, Point location, int width, string startValue,int maxLength, BorderStyle style, TextChangedAction action)
        {
            txt = new TextBox {
                Text = startValue,
                Location = location,
                Width = width,
                BorderStyle = style,
                MaxLength = maxLength
            };
            if (action != null)
                txt.TextChanged += (object sender, EventArgs e) => action();  
            
            GameObject.GameToRun.Controls.Add(txt);          
        }
        public static void CreateComboBox(ref ComboBox cmb, Rectangle location, Array items)
        {
            cmb = new ComboBox { 
                Bounds = location
            };

            cmb.DataSource = items;

            GameObject.GameToRun.Controls.Add(cmb);
        }
    }
}
