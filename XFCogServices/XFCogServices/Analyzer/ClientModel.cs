using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFCogServices.Analyzer
{
    public class ClientModel
    {
        public FaceRectangle[] faceRectangels { get; set; }
        public string[] genders { get; set; }
        public int[] faceAges { get; set; }
        public string[] captions { get; set; }
        public string[] tags { get; set; }
        public string imageFormat { get; set; }
        public string imageDimensions { get; set; }
        public int clipArtType { get; set; }
        public int lineDrawingType { get; set; }
        public bool isBlackAndWhite { get; set; }
        public bool isAdultContent { get; set; }
        public double adultScore { get; set; }
        public bool isRacyContent { get; set; }
        public double racyScore { get; set; }
        public string[] categories { get; set; }
        public string dominantColorBackground { get; set; }
        public string dominantColorForeground { get; set; }
        public string[] dominantColors { get; set; }
        public string accentColor { get; set; }
    }
}
