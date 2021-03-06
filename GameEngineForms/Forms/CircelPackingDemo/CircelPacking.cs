﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameEngineForms.Resources;
using System.Diagnostics;
using static GameEngineForms.Resources.GameEngineObjects;
using static GameEngineForms.Services.GameEngineServices;
using static GameEngineForms.Services.ExtensionMethods;
using System.Threading;

namespace GameEngineForms.Forms.CircelPackingDemo
{
    class CircelPacking : DefaultParentForm
    {
        
        static int threadCount = Environment.ProcessorCount;
        List<CirkelBatch> batchProcessing = new List<CirkelBatch>();
        static Random rand = new Random();
        static Circel mouseCircel;
        List<Circel> circels = new List<Circel>();
        List<Color> threadColor = new List<Color>();

        int circelMaxSupply = 3000,
            mouseCircelDiameter = 100,
            strakeSize = 1,
            minimumCircelSize = 10,
            maxCircelSize = 30;
             
        public override void GameAssetsLoadIn()
        {                    
            int supplyCount = 0;
            mouseCircel = new Circel(new Vector2(), mouseCircelDiameter,Color.Magenta);
            for (int i = 0; i < threadCount; i++) {

                if (i != threadCount-1)
                { 
                    batchProcessing.Add(new CirkelBatch(i, circelMaxSupply / threadCount));
                    supplyCount += circelMaxSupply / threadCount;
                }
                else
                {
                    int supplyLeft = circelMaxSupply - supplyCount;
                    batchProcessing.Add(new CirkelBatch(i, supplyLeft));
                }
            }

            Height = 600;
            Width = 800;
            base.GameAssetsLoadIn();
            BackColor = Color.Black;

            for (int i = 0; i < threadCount; i++) threadColor.Add(RandomColor());
            for (int i = 0; i < circelMaxSupply;)
            {
                FillBatchProcessing();
                batchProcessing.ForEach(b => { circels.AddRange(b.Batch); });

                i = circels.Count;
            }


        }

        public override void GameLoop(object sender, PaintEventArgs e)
        {
            // ------------------------------------------------------------------------------
            // Add circels to batche items + Update mousBall
            // ------------------------------------------------------------------------------                  
            FillBatchProcessing();

            // ------------------------------------------------------------------------------
            // Circal to circal colision
            // ------------------------------------------------------------------------------         
            batchProcessing.ForEach(b => { circels.AddRange(b.Batch); });
            mouseCircel.location = GetMousePosition<Vector2>();

            // ------------------------------------------------------------------------------
            // DrawCircels
            // ------------------------------------------------------------------------------
            circels.ForEach(c => { 
                e.Graphics.DrawEllipse( new Pen(c.BorderColor),c.location.X - (c.Radius), c.location.Y - (c.Radius),c.Diameter,c.Diameter);         
            });

            // ------------------------------------------------------------------------------
            // Mouse Colision
            // ------------------------------------------------------------------------------      
            
            Parallel.For(0, batchProcessing.Count, (onThread) =>
            {
                batchProcessing[onThread].Batch.ForEach(c =>
                {
                    if (c != mouseCircel) RepellBallfromBall(ref c.location, c.Radius, strakeSize, mouseCircel.location, mouseCircel.Radius, strakeSize);

                    c.location.Y += new Random().Next(-c.FallSpeed + 1, c.FallSpeed);
                    c.location.X += new Random().Next(-c.FallSpeed + 1, c.FallSpeed);
                });

                batchProcessing[onThread].Batch.RemoveAll(c => c != mouseCircel && (c.location.X <= -c.Radius || c.location.X >= (Width + c.Radius) || c.location.Y <= -c.Radius || c.location.Y >= (Height + c.Radius)));
            });

            // ------------------------------------------------------------------------------
            // Object count
            // ------------------------------------------------------------------------------
            GameObject.ObjectCount = circels.Count;
        }
      
        public void FillBatchProcessing()
        {
            // ------------------------------------------------------------------------------
            // Create
            // ------------------------------------------------------------------------------
            circels.Clear();
            circels.Add(mouseCircel);
            Parallel.For(0, batchProcessing.Count, (onThread) => {
               
                if (batchProcessing[onThread].Batch.Count < batchProcessing[onThread].MaxSupply)
                {                   
                    Vector2 newLocation = newLocation = new Vector2().RandomVector2();
                    var newDiameter = rand.Next(minimumCircelSize, maxCircelSize);
                    var newRadius = newDiameter / 2;
                    
                    var haveNewCircal  
                           = batchProcessing[onThread].Batch.TrueForAll(
                                c => Vector2.Subtract(newLocation, c.location).Length() > (newRadius + c.Radius) + strakeSize &&
                                     Vector2.Subtract(newLocation, mouseCircel.location).Length() > (mouseCircel.Radius + c.Radius) + strakeSize);


                    if (haveNewCircal) batchProcessing[onThread].Batch.Add(new Circel(newLocation, newDiameter, threadColor[onThread]));
                }       
            });             

        }
    } 
    public class Circel
    {
        public Vector2 location;
        public int Diameter { get; set; }
        public int Radius { get; set; }
        public int FallSpeed { get; set; }
        public Stopwatch LifeTime { get; set; } = new Stopwatch();
        public Color BorderColor { get; set; }

        public Circel(Vector2 loc, int dia, Color col) 
        {
            BorderColor = col;
            location = loc;
            Diameter = dia;
            Radius = dia / 2;
            FallSpeed = new Random().Next(3, 5);
            LifeTime.Start();
        }     
    }
    public class CirkelBatch
    {
        public CirkelBatch(int index, int maxSupply)
        {
            Index = index;
            MaxSupply = maxSupply;
        }
        public int Index { get; set; }
        public int MaxSupply { get; set; }
        public List<Circel> Batch { get; set; } = new List<Circel>();     
    }
}
