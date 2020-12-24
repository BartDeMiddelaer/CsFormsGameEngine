using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameEngineForms.Resources;
using System.Diagnostics;
using static GameEngineForms.Services.GameEngineServices;
using static GameEngineForms.Services.ExtensionMethods;


namespace GameEngineForms.Forms.CircelPackingDemo
{
    class CircelPacking : DefaultParentForm
    {

        static Random rand = new Random();
        int strakeSize = 1;

        List<Circel> circels =
            new List<Circel> {new Circel(new Vector2(rand.Next(0,800),rand.Next(0, 800)),rand.Next(0,80))};
                    
        public override void GameAssets()
        {
            Height = 800;
            Width = 800;
            base.GameAssets();
        }
        public override void GameLoop(object sender, PaintEventArgs e)
        {
            // ------------------------------------------------------------------------------
            // New Circal Props
            // ------------------------------------------------------------------------------
            bool haveNewCircal = false;
            int newDiameter = 0;
            Vector2 newLocation = new Vector2();

            while (!haveNewCircal)
            {
                newDiameter = rand.Next(0, 80);
                newLocation = new Vector2().RandomVector2();
                haveNewCircal = circels.TrueForAll(c => Vector2.Subtract(newLocation, c.location).Length() > (newDiameter / 2 + c.Diameter / 2) + strakeSize);

            }   circels.Add(new Circel(newLocation, newDiameter));

            // ------------------------------------------------------------------------------
            // Draw
            circels.ForEach(c => { e.Graphics.DrawEllipse(
                  new Pen(RandomColor(), strakeSize),
                  c.location.X - (c.Diameter / 2),
                  c.location.Y - (c.Diameter / 2),
                  c.Diameter,
                  c.Diameter);
            });

            // Colide
            circels.ForEach(c => {


                if (circels.TrueForAll(c => Vector2.Subtract(newLocation, c.location).Length() > (newDiameter / 2 + c.Diameter / 2) + strakeSize)) 
                    
                    c.location.Y += c.FallSpeed;

            


            });

            // Dispose
            circels.RemoveAll(c => 
            c.LifeTime.Elapsed > TimeSpan.FromSeconds(rand.Next(5,20)) || 
            c.Diameter < 10);
            // ------------------------------------------------------------------------------

            
        }
    }

    public class Circel
    {
        public Vector2 location;
        public int Diameter { get; set; }
        public int FallSpeed { get; set; }
        public Stopwatch LifeTime { get; set; } = new Stopwatch();

        public Circel(Vector2 loc, int dia) 
        {            
            location = loc;
            Diameter = dia;
            FallSpeed = new Random().Next(1, 3);
            LifeTime.Start();
        }     
    }
}
