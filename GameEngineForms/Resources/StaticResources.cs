using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngineForms.Resources
{

    // to call
    public static class StaticResources
    {
        public static List<BitmapInfo> BitmapResources { get; set; } = new List<BitmapInfo>() { };

    }


    // to fill in 
    public class BitmapRepo
    {
        public BitmapRepo()
        {

            for (long i = 0; i < 55000000; i++)
            {
                Resources.Add(new BitmapInfo());

            }

        }
        public List<BitmapInfo> Resources { get; set; } = new List<BitmapInfo>();
    }


    // dataTypes
    public struct BitmapInfo
    {
        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
    }

}
