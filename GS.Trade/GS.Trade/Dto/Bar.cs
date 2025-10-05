using System;

namespace GS.Trade.Dto
{
   // [Serializable]
    public class Bar : IBarSimple
    {
        public string Key {
            get { return SeriesId + "@" + DT.ToString("s"); }
        }
        public long SeriesId { get; set; }
        public DateTime DT { get;  set; }
        public double Open { get;  set; }
        public double High { get;  set; }
        public double Low { get;  set; }
        public double Close { get;  set; }
        public double Volume { get;  set; }
        public uint Ticks { get;  set; }

        public override string ToString()
        {
            return
                $"Type:{GetType()}; SeriesId:{SeriesId}; DT:{DT.ToString("s")}; Open:{Open}; High:{High}; Low:{Low}; Close:{Close}; Volume: {Volume}";
        }
    }
    [Serializable]
    public class BarDto : IBarBase, IBarBaseRW
    {
        public DateTime DT { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        public override string ToString()
        {
            return
                $"Type:{GetType()}; " +
                $"DT:{DT.ToString("G")}; " +
                $"Open:{Open}; High:{High}; Low:{Low}; Close:{Close}; Volume: {Volume}";
        }
        public string Key => DT.ToString("s");
    }
}
