using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.DownLoader
{
    public class Bar : IBarFromFinam
    {
        public string Ticker { get; set; }
        public string TimeInt { get; set; }
        public DateTime DT { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public decimal Volume { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                          Ticker, TimeInt, DT, Open, High, Low, Close, Volume);
        }
    
    }
    

}
