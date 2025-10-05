using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.ICharts
{
    public class Level : ILevel
    {
        public double Value;
        public int Color;
        public int BackGroundColor = 0xffffff;
        public string Text;
        public int LineWidth = 1;
         
        public Func<double> GetValue;
        public Func<string> GetText;
        public Func<bool> IsValid;

        public bool Valid { get { return IsValid(); }}
        public double LevelValue { get { return GetValue == null ? Value : GetValue(); } }
        public string TextValue { get { return GetText == null ? Text : GetText(); } }

        public int LevelColor { get { return Color; } }
        public int LevelBackGroundColor { get { return BackGroundColor; } } 
        public string LevelText { get { return Text; } }
        public int LevelLineWidth {
            get { return LineWidth; }
        }
    }
}
