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

        int strakeSize = 1;
        const int mouseCircelDiameter = 220;
        static Random rand = new Random();
        Circel mouseCircel = new Circel(new Vector2(), mouseCircelDiameter);

        List<Circel> circels =
            new List<Circel> {new Circel(new Vector2(800/2,800/2),80)};
                    
        public override void GameAssets()
        {
            Height = 800;
            Width = 800;
            base.GameAssets();
            BackColor = Color.Black;
        }
        public override void GameLoop(object sender, PaintEventArgs e)
        {
            // Dispose
            circels.RemoveAll(c => c.LifeTime.Elapsed > TimeSpan.FromSeconds(5));
            // ------------------------------------------------------------------------------     

            // ------------------------------------------------------------------------------
            // New Circal Props
            // ------------------------------------------------------------------------------
            var haveNewCircal = false;
            var newDiameter = 0;
            Vector2 newLocation = new Vector2();

            
            while (!haveNewCircal)
            {
                newDiameter = rand.Next(10,500);
                newLocation = new Vector2().RandomVector2();
                haveNewCircal = circels.TrueForAll(c => Vector2.Subtract(newLocation, c.location).Length() > (newDiameter / 2 + c.Diameter / 2) + strakeSize && 
                                                        Vector2.Subtract(newLocation, mouseCircel.location).Length() > (mouseCircel.Diameter / 2 + c.Diameter / 2) + strakeSize);

            }   circels.Add(new Circel(newLocation, newDiameter));

            // ------------------------------------------------------------------------------
            // Draw MouseBall + update
            mouseCircel.location = GetMousePosition<Vector2>();

            e.Graphics.FillEllipse(Brushes.Gray, new Rectangle(
                new Point(
                    GetMousePosition<Point>().X - mouseCircelDiameter/2,
                    GetMousePosition<Point>().Y - mouseCircelDiameter/2), new Size(mouseCircelDiameter, mouseCircelDiameter)));

            // DrawCircels
            circels.ForEach(c => { e.Graphics.FillEllipse(
                  new SolidBrush(c.BorderColor),
                  c.location.X - (c.Diameter / 2),
                  c.location.Y - (c.Diameter / 2),
                  c.Diameter,
                  c.Diameter);
            });  

            // Colide
            circels.ForEach(c => {

                var travel = Vector2.Subtract(GetMousePosition<Vector2>(), c.location).Length();
                var combindeDiameters = (mouseCircelDiameter / 2 + c.Diameter / 2) + strakeSize;

                if (travel < combindeDiameters)
                {
                    // moet nog aangemast worden
                    Intersection_Circle(c.location, c.Diameter / 2, GetMousePosition<Vector2>(), c.location, out Vector2 intersection);
                    var angel = GetAngel(GetMousePosition<Vector2>(),c.location);

                    c.location = GetPointFromPoint(intersection, (c.Diameter / 2) + 5, angel);
                }

            });          
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
