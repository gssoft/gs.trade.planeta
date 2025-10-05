using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.DownLoader
{
    public class Ticker
    {
        public int ID { get; set;}
        public string Code { get; set; }
        public string Name { get; set; }

        public Ticker()
        {
            // needs for serialization
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2}", ID, Code, Name);
        }
    }
    public class TimeInt
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int DaysPerPass { get; set; } 
    
        public TimeInt()
        {
            // needs for serialization
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", ID, Code, Name, DaysPerPass);
        }
    }
}

