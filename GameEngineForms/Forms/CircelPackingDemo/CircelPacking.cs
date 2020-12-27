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
        static Circel mouseCircel;
        List<Circel> circels;

        int mouseCircelDiameter = 220, 
            strakeSize = 1, 
            circelChap = 500, 
            maxCircelSize = 50;
                    
        public override void GameAssetsLoadIn()
        {
            Height = 800;
            Width = 1250;
            base.GameAssetsLoadIn();
            BackColor = Color.Black;
            circels = new List<Circel> { new Circel(new Vector2(), mouseCircelDiameter) };
            mouseCircel = circels[0];
        }
        public override void GameLoop(object sender, PaintEventArgs e)
        {
            // ------------------------------------------------------------------------------
            // New Circal Props
            // ------------------------------------------------------------------------------
            var haveNewCircal = false;
            var newDiameter = 0;
            Vector2 newLocation = new Vector2();
          
            if(circels.Count < circelChap ) while (!haveNewCircal)
            {
                newDiameter = rand.Next(10, maxCircelSize);
                newLocation = new Vector2().RandomVector2();
                haveNewCircal = circels.TrueForAll(c => Vector2.Subtract(newLocation, c.location).Length() > (newDiameter / 2 + c.Diameter / 2) + strakeSize && 
                                                        Vector2.Subtract(newLocation, mouseCircel.location).Length() > (mouseCircel.Diameter / 2 + c.Diameter / 2) + strakeSize);

            }   circels.Add(new Circel(newLocation, newDiameter));

            // ------------------------------------------------------------------------------
            // Colision
            // ------------------------------------------------------------------------------
            circels.ForEach(c => {

                var travel = Vector2.Subtract(GetMousePosition<Vector2>(), c.location).Length();
                var combindeDiameters = (mouseCircelDiameter / 2 + c.Diameter / 2) + strakeSize;

                if ((travel < combindeDiameters) && c != mouseCircel)
                {
                    // moet nog aangemast worden
                    Intersection_Circle(c.location, c.Diameter / 2, GetMousePosition<Vector2>(), c.location, out Vector2 intersection);
                    var angel = GetAngel(GetMousePosition<Vector2>(), c.location);

                    c.location = GetPointFromPoint(intersection, (c.Diameter / 2) + (combindeDiameters - travel), angel);
                }

            });

            // ------------------------------------------------------------------------------
            // DrawCircels
            // ------------------------------------------------------------------------------
            circels.ForEach(c => { e.Graphics.FillEllipse(
                  new SolidBrush(c.BorderColor),
                  c.location.X - (c.Diameter / 2),
                  c.location.Y - (c.Diameter / 2),
                  c.Diameter,
                  c.Diameter);
            });

            // ------------------------------------------------------------------------------
            // Dispose + Update mousBall
            // ------------------------------------------------------------------------------
            circels.RemoveAll(c => c != mouseCircel && (c.location.X <= 0 || c.location.X >= Width || c.location.Y <= 0 || c.location.Y >= Height));
            mouseCircel.location = GetMousePosition<Vector2>();

        }
    }

    public class Circel
    {
        public Vector2 location;
        public int Diameter { get; set; }
        public int FallSpeed { get; set; }
        public Stopwatch LifeTime { get; set; } = new Stopwatch();
        public Color BorderColor { get; set; } = RandomColor();

        public Circel(Vector2 loc, int dia) 
        {            
            location = loc;
            Diameter = dia;
            FallSpeed = new Random().Next(1, 3);
            LifeTime.Start();
        }     
    }
}
