using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Services.DrawServices;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Services.MathServices;
using static GameEngineForms.Resources.DynamicResources;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace GameEngineForms.Forms
{

    public partial class Game : Form
    {
        Random rand = new Random();
        int celInterval = 4;

        // x,y == cords // Z == living yes or no 1 = yes 0 = no
        List<Vector3?> celPop = new List<Vector3?>();

        public Game() => Initialize += () => {

            GameCycle += DrawLoop;
            ClientSize = new Size(804, 601);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            //TopMost = true;
            BackColor = Color.BurlyWood;
            StartPosition = FormStartPosition.CenterScreen;
            GameObjects.RenderMode = SmoothingMode.HighSpeed;

            for (int x = 0; x < Width; x+= celInterval)
            {
                for (int y = 0; y < Height; y+= celInterval)
                {
                    int state = rand.Next(0, 2);
                    celPop.Add(new Vector3(x, y, state));
                }
            }
        };

        private void DrawLoop(object sender, PaintEventArgs e)
        {

            celPop.ForEach(cel => {
                if (cel?.Z == 1) {

                    Vector2 cords = new Vector2 { 
                        X = cel?.X ?? 0,
                        Y = cel?.Y ?? 0                   
                    };

                    DrawRect(null, Color.BlueViolet, null, cords, celInterval-1, celInterval-1, null, null);                                            
                
                }
            });

            ThinOutPopelation();
        }

        void ThinOutPopelation()
        {
          
            List<Vector3?> celsToLive = new List<Vector3?>();

             celPop.ForEach(cel => {
             
                 if(cel?.Z == 1)
                 celsToLive.Add(cel);

                
            });

            celPop.Clear();
            celPop = celsToLive;
        }

        private List<Vector3?> Livingneighbors(float? xIndex, float? yIndex)
        {
            Vector3? chacker = celPop.Find(v => v?.X == xIndex && v?.Y == yIndex);

            List<Vector3?> returnList = new List<Vector3?> {
                celPop.Find(v => v?.X == chacker?.X && v?.Y == (chacker?.Y - celInterval) && v?.Z == 1),//celUp    
                celPop.Find(v => v?.X == chacker?.X && v?.Y == (chacker?.Y + celInterval) && v?.Z == 1),//celDown     
                celPop.Find(v => (v?.X - celInterval) == chacker?.X && v?.Y == chacker?.Y && v?.Z == 1),//celLeft     
                celPop.Find(v => (v?.X + celInterval) == chacker?.X && v?.Y == chacker?.Y && v?.Z == 1),//celRecht   
                celPop.Find(v => v?.X == (chacker?.X - celInterval) && v?.Y == (chacker?.Y - celInterval) && v?.Z == 1),//celUpLeft  
                celPop.Find(v => v?.X == (chacker?.X + celInterval) && v?.Y == (chacker?.Y - celInterval) && v?.Z == 1),//celUpRecht 
                celPop.Find(v => v?.X == (chacker?.X - celInterval) && v?.Y == (chacker?.Y + celInterval) && v?.Z == 1), //celDownLeft
                celPop.Find(v => v?.X == (chacker?.X + celInterval) && v?.Y == (chacker?.Y + celInterval) && v?.Z == 1)//celDownRecht 
            };

            return returnList.FindAll(v => v != null);
        }
    }
}
