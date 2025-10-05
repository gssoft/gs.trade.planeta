using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;

namespace GS.Trade.Data.Chart
{
    public class ChartText : IChartText
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string Header;
        public string Text;

        public int Color { get; set; }
        public int BackGroundColor { get; set; }
        
        public string FontName { get; set; }
        public double FontSize { get; set; }

        public Func<string> GetHeader;
        public Func<string> GetText;

        public Func<int> GetColor;
        public Func<int> GetBackGroundColor;
        
        public Func<bool> IsValid;

        public bool Valid { get { return IsValid(); } }
        public string HeaderValue { get { return GetHeader == null ? Header : GetHeader(); } }
        public string TextValue { get { return GetText == null ? Text : GetText(); } }

        public int TextColor { get { return GetColor == null ? Color : GetColor(); } }
        public int TextBackGroundColor { get { return GetBackGroundColor == null ? BackGroundColor : GetBackGroundColor(); } }

        public ChartText()
        {
            Color = 0x0;
            BackGroundColor = 0xffffff;

            FontName = "Arial Bold Italic";
            FontSize = 8d;
        }
    }
}
