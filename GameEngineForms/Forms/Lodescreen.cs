using System;
using System.Drawing;
using System.Windows.Forms;
using static GameEngineForms.Resources.StaticGameObjects;
using static GameEngineForms.Resources.GameEngineObjects;
using static GameEngineForms.Services.ControlServices;
using static GameEngineForms.Services.GameEngineServices;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using GameEngineForms.Resources;
using Timer = System.Windows.Forms.Timer;
using System.Collections.Generic;
using GameEngineForms.Services;

namespace GameEngineForms.Forms
{
    public partial class Lodescreen : Form
    {
        readonly PictureBox LoopContainer = new PictureBox();
        List<LoadTask> Tasks = new List<LoadTask>{

            new LoadTask{ Index = 3, Name = "Game asset's loading",     ToDo = new Task(InvokeAssetLoading) },
            new LoadTask{ Index = 2, Name = "Bitmap asset's loading",   ToDo = new Task(() => BitmapResources = new BitmapRepo().Resources)},
            new LoadTask{ Index = 1, Name = "Asset loading",            ToDo = new Task(() => SoundResources = new SoundRepo().Resources)},
            new LoadTask{ Index = 0, Name = "Initialization",           ToDo = new Task(() => Thread.Sleep(GameObject.MinimumLodeScreenTime))}
        };
        readonly Timer tikker = new Timer();

        public Lodescreen()
        {
            ClientSize = new Size(800, 450);
            Opacity = 0.7;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TransparencyKey = Color.Gray;
            BackColor = Color.Gray;
            LoopContainer.Dock = DockStyle.Fill;
            LoopContainer.Paint += Render;
            Controls.Add(LoopContainer);
            Tasks.ForEach(t => t.ToDo.Start());

            tikker.Tick += (object sender, EventArgs e) => LoopContainer.Refresh(); 
            tikker.Start();
            tikker.Interval = 10;
        }

        private void Render(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            #region // anemations ------------------------------

                int teller = 2;
                var path = RoundedRect(new Rectangle(200, 0, Width - 200, Height), 50);
                var strokePath = RoundedRect(new Rectangle(2, 20, Width, Height-40), 60);
            
                e.Graphics.FillPath(new SolidBrush(Color.White), path);
                e.Graphics.DrawPath(new Pen (new SolidBrush(Color.White),5), strokePath);

                for (int x = 0; x < 50; x++)
                {
                    var diamiter = new Random().Next(0, 20);
                    e.Graphics.DrawEllipse(new Pen(Color.Gray, 2), new Rectangle(
                        new Random().Next(200,Width),
                        new Random().Next(350, Height),
                        diamiter, diamiter));

               
                }              
                for (int b = 0; b < 30; b+= 5)
                {    
                    var bubbelBorder = new GraphicsPath();

                    for (int x = 0; x < 101; x++)
                        bubbelBorder.AddLine(
                            190+ (x * 10), 
                            (new Random().Next(300 + b, 302 + b)) + (b* teller) /3,
                            190 + (x * 10), 
                            (new Random().Next(300 + b, 302 + b)) + (b* teller) /3
                        );
                
                    e.Graphics.DrawPath(new Pen(Color.Gray, teller), bubbelBorder);
                    teller++;
                }
            #endregion

            // Text ------------------------------------
            e.Graphics.DrawString("Game", new Font("Arial", 50), Brushes.White, 0, 50);
            e.Graphics.DrawString("Engine", new Font("Arial", 50), Brushes.Gray, 200, 50);
            e.Graphics.DrawString("SandBox of fun", new Font("Arial", 15), Brushes.Red, 420, 92);
            e.Graphics.DrawString("Bart De Middelaer", new Font("Arial", 7), Brushes.Black, 212, 115);

            Tasks.ForEach(task => {

                int txtOffset = task.Index * 18;
                e.Graphics.DrawString(task.ToString(), new Font("Arial", 10), Brushes.Red, 210, Height - (240 + txtOffset));
            });

            if (Tasks.TrueForAll(t => t.ToDo.IsCompleted))
            {
                Hide();
                GameObject.GameToRun.ShowDialog();
            }
        }

        public class LoadTask
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public Task ToDo { get; set; }

            public override string ToString()
            {
                string status = ToDo.IsCompleted ? "Done!" : "Loading";
                return $"{status}      \t {Name}";
            }
        }
    }
}
