using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;

namespace GS.Trade.TimeSeries.FortsTicks.Model
{
    public class Ticker
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(8)]
        public string Code { get; set; }
        [MaxLength(16)]
        public string Contract { get; set; }
       // public virtual ICollection<Tick> Ticks { get; set; }
        public int Decimals { get; set; }

        public Ticker()
        {
           // Ticks = new List<Tick>();
        }
    }

    public class Tick : ITickBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TradeID { get; set; }
        //public long ID { get; set; }
        public int TickerID { get; set; }
        public DateTime DT { get; set; }
        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }
        public int SrcFileID { get; set; }
        [ForeignKey("SrcFileID")]
        public virtual SrcFile SrcFile { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        //public long TradeID { get; set; }

    }
    public class Bar : IBarBase
    {
        [Key]
        [Column(Order = 1)]
        public int TickerID { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime DT { get; set; }

        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }

        //public int SrcFileID { get; set; }
        //[ForeignKey("SrcFileID")]
        //public virtual SrcFile File { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public double Volume { get; set; }

        public string Key
        {
            get { return string.Join("@", TickerID, DT.ToString("s")); }
        }
    }

    public class Stat // : ITimeSeriesStat
    {
        public Stat()
        {
            ModifiedDT = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }

        //[Key]
        //[Column(Order = 1)]
        public int TickerID { get; set; }

        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }

        //[Key]
        //[Column(Order = 2)]
        public TickBarTypeEnum Type { get; set; }

        //[Key]
        //[Column(Order = 3)]
        public TimeSeriesStatEnum Period { get; set; }

        //[Key]
        //[Column(Order = 4)]
        public int LastDate { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }

        public DateTime LastDT { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public DateTime? ModifiedDT { get; set; }
        public int Days
        {
            get { return 1 + (LastDT.Date - FirstDT.Date).Days; }
        }

        public override string ToString()
        {
            return
                $"ID: {ID}, TickerID: {TickerID}," +
                $" SeriesType: {Type}, TimeIntType: {Period}," +
                $" LastDateKey: {LastDate}, Count: {Count}, FirstDT: {FirstDT}, LastDT: {LastDT}," +
                $" MinValue: {MinValue}, MaxValue{MaxValue}, Days: {Days}, Modified: {ModifiedDT}";
        }
    }

    public class SrcFile
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }

        public DateTime LastDT { get; set; }

        public DateTime? ModifiedDT { get; set; }

        // public virtual ICollection<Tick> Ticks { get; set; }
       // public virtual ICollection<Bar> Bars { get; set; }
        public SrcFile()
        {
            ModifiedDT = DateTime.Now;
        }
    }
}
