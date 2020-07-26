using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Text;

namespace GameEngineForms.Resources
{

    // to call
    public static class StaticResources
    {
        public static List<BitmapInfo> BitmapResources { get; set; } = new List<BitmapInfo>() { };
        public static List<SoundInfo> SoundResources { get; set; } = new List<SoundInfo>() { };
    }


    // to fill in 
    public class BitmapRepo
    {     
        public List<BitmapInfo> Resources { get; set; } = new List<BitmapInfo>();
    }
    public class SoundRepo
    {
        public List<SoundInfo> Resources { get; set; } = new List<SoundInfo>();
    }


    // dataTypes
    public struct BitmapInfo
    {
        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
    }

    public struct SoundInfo
    {
        public string Name { get; set; }
        public SoundPlayer Sound { get; set; }

    }        
}
