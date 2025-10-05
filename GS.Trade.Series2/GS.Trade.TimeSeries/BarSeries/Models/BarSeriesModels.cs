using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
// using Math = GS.GSMath.Math;
using GS.Trade.TimeSeries.General;

namespace GS.Trade.TimeSeries.BarSeries.Models
{
    public abstract class DbElement1
    {
        public long ID { get; set; }
        [Required]
        [MaxLength(32)]
        public string Code { get; set; }
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string Description { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDT { get; set; }

        protected DbElement1()
        {
            ModifiedDT = DateTime.Now;
        }
    }
    [Table("TradeBoards")]
    public class TradeBoard : DbElement1
    {
        public string TradeBoardKey { get { return Code; } }
        public virtual ICollection<Ticker> Tickers { get; set; }
    }

    public class Ticker : DbElement1
    {
        [ForeignKey("TradeBoardID")]
        public virtual TradeBoard TradeBoard { get; set; }
        public long TradeBoardID { get; set; }
        public virtual ICollection<TimeSeries> TimeSeries { get; set; }
        [MaxLength(24)]
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public string TickerKey
        {
            get { return (TradeBoard != null ? TradeBoard.Code + "@" : "") + Code; }
        }
    }
    public class TimeInt : DbElement1
    {
        public int TimeInterval { get; set; }
        public int TimeShift { get; set; }

        public string TimeIntKey
        {
            get { return "TI:" + TimeInterval + "Sec" + (TimeShift != 0 
                    ? ";SH:" + TimeShift + "Sec" 
                    : ""); }
        }
    }
    public abstract class TimeSeries : DbElement1  // , ITimeSeriesBase3
    {
        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }
        public long TickerID { get; set; }

        [ForeignKey("TimeIntID")]
        public virtual TimeInt TimeInt { get; set; }
        public long TimeIntID { get; set; }

        [ForeignKey("QuoteProviderID")]
        public virtual QuoteProvider QuoteProvider { get; set; }
        public long QuoteProviderID { get; set; }
        public virtual ICollection<Stat> Stats{ get; set; }

        //public string Key {
        //    get { return TickerId.ToString() + ";" + TimeIntId + ";" + QuoteProviderId; }
        //}

        protected TimeSeries()
        {
            Stats = new List<Stat>();
        }

        public string Key
        {
            get
            {
                return Ticker.TickerKey + "@" +
                          TimeInt.TimeIntKey + "@" +
                          QuoteProvider.QuoteProviderKey;
            }
        }
    }
    // Stat for TimeSeries Ont to Zero or One relationships
    public class Stat // : ITimeSeriesStat
    {
        public Stat()
        {
            ModifiedDT = DateTime.Now;
            FirstDT = DateTime.MinValue.MinValueToSql();
            LastDT = DateTime.MinValue.MinValueToSql();
        }
        [Key]
        public long ID { get; set; }
        [ForeignKey("TimeSeries")]
        public long TimeSeriesID { get; set; }
        public virtual TimeSeries TimeSeries { get; set; }
        public TickBarTypeEnum Type { get; set; }

        public TimeSeriesStatEnum Period { get; set; }

        public int LastDate { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }

        public DateTime LastDT { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public int Days
        {
            get { return 1 + (LastDT.Date - FirstDT.Date).Days; }
        }

        public DateTime? ModifiedDT { get; set; }
        public override string ToString()
        {
            return String.Format("Type: {0}, ID: {1}, TimeSrsID: {2}, Type: {3}, TimeInterval: {4}, LastDate: {5}, " +
                                 "Count: {6}, FirstDT: {7}, LastDT: {8}, MinValue: {9}, MaxValue: {10}, Days: {11}",
                                 GetType(), ID, TimeSeriesID, Type, Period, LastDate, Count, FirstDT, LastDT, MinValue, MaxValue, Days);
        }
    }
    //public abstract class TimeSeriesItem
    //{
    //    // public long ID { get; set; }

    //    //[ForeignKey("TimeSeriesId")]
    //    //public virtual TimeSeries TimeSeries { get; set; }
    //    //public long TimeSeriesId { get; set; }

    //    // public DateTime DT { get; set; }
    //}
    //[Table("BarSeries")]
    public class BarSeries : TimeSeries
    {
        public virtual ICollection<Bar> Items { get; set; }
    }
    //[Table("TickSeries")]
    public class TickSeries : TimeSeries
    {
        public virtual ICollection<Tick> Items { get; set; }
    }


    //[Table("Bars")]
    public class Bar // : TimeSeriesItem //, IBarDb
    {
        [ForeignKey("BarSeriesID")]
        public virtual TimeSeries TimeSeries { get; set; }
        [Key, Column(Order = 1)]
        public long BarSeriesID { get; set; }
        [Key, Column(Order = 2)]
        public DateTime DT { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public double Volume { get; set; }

        public double TypicalPrice
        {
            get { return (Close + High + Low) / 3d; }
        }
        public double MedianPrice
        {
            get { return (High + Low) / 2d; }
        }
        public string Key {
            get { return BarSeriesID + DT.ToString("s"); }
        }
    }
    //[Table("Ticks")]
    public class Tick // : TimeSeriesItem
    {
        [Key]
        public long TradeID { get; set; }
        public DateTime DT { get; set; }

        //[ForeignKey("TickSeriesId")]
        //public virtual TickSeries TimeSeries { get; set; }
        //public long TimeSeriesId { get; set; }

        [ForeignKey("TickSeriesID")]
        public virtual TimeSeries TimeSeries { get; set; }
        public long TickSeriesID{ get; set; }

        public double Ammount { get; set; }
        public double Price { get; set; }
    }
    public class QuoteProvider : DbElement1
    {
        public string QuoteProviderKey
        {
            get { return Code; }
        }
    }
    
}
