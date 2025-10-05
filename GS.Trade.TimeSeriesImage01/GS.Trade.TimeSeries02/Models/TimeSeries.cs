using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

namespace GS.Trade.TimeSeries02.Models
{
    public abstract class DbElement1
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string Name { get; set; }
        //public string Alias { get; set; }
        public string Description { get; set; }
        //public string Tags { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDT { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDT { get; set; }

        protected DbElement1()
        {
            //var dt = DateTime.Now;
            //CreatedDT = dt;
            //ModifiedDT = dt;

            CreatedDT = ModifiedDT = DateTime.Now;
        }
    }
    public class Ticker : DbElement1
    {
        //[ForeignKey("TradeBoardId")]
        //public virtual TradeBoard TradeBoard { get; set; }
        //public long TradeBoardId { get; set; }
        public string TradeBoard { get; set; }
        public virtual IList<TimeSeries> TimeSeries { get; set; }
        // public string TickerKey => (TradeBoard != null ? TradeBoard.Code + "@" : "") + Code;
    }
    public class TimeInt : DbElement1
    {
        public int TimeInterval { get; set; }
        public int TimeShift { get; set; }

        public string TimeIntKey => "TI:" + TimeInterval + "Sec" + (TimeShift != 0
            ? ";SH:" + TimeShift + "Sec"
            : "");

        //public string TimeIntKey
        //{
        //    get { return Code.HasValue() + (TimeShift != 0 ? "@" + TimeShift : ""); }
        //}
    }

    public class QuoteProvider : DbElement1
    {
        public string QuoteProviderKey => Code;
        public virtual ICollection<TimeSeries> TimeSeries { get; set; }
    }
    public abstract class TimeSeries : DbElement1, ITimeSeriesBase2
    {
        [ForeignKey("TickerId")]
        public virtual Ticker Ticker { get; set; }
        public long TickerId { get; set; }

        [ForeignKey("TimeIntId")]
        public virtual TimeInt TimeInt { get; set; }
        public long TimeIntId { get; set; }

        [ForeignKey("QuoteProviderId")]
        public virtual QuoteProvider QuoteProvider { get; set; }
        public long QuoteProviderId { get; set; }

        //public string Key {
        //    get { return TickerId.ToString() + ";" + TimeIntId + ";" + QuoteProviderId; }
        //}
        //public string Key => Ticker.TickerKey + "@" +
        //                     TimeInt.TimeIntKey + "@" +
        //                     QuoteProvider.QuoteProviderKey;
        public string Key => Id.ToString();
    }  
    //[Table("BarSeries")]
    public class BarSeries : TimeSeries
    {
        public BarSeries()
        {
            Items = new List<Bar>();
        }
        public ICollection<Bar> Items { get; set; }
    }
    //[Table("TickSeries")]
    public class TickSeries : TimeSeries
    {
        public TickSeries()
        {
            Items = new List<Tick>();
        }
        public  ICollection<Tick> Items { get; set; }
    }
#region BytesSeries
    public class BytesBarTimeSeries : TimeSeries
    {
        public ICollection<BytesBarTimeSeriesItem> Items;
        public BytesBarTimeSeries()
        {
            Items = new List<BytesBarTimeSeriesItem>();
        }
    }
    public class BytesTickTimeSeries : TimeSeries
    {
        public ICollection<BytesTickTimeSeriesItem> Items;
        public BytesTickTimeSeries()
        {
            Items = new List<BytesTickTimeSeriesItem>();
        }
    }
    public class BytesOrderLogTimeSeries : TimeSeries
    {
        public ICollection<BytesOrderLogTimeSeriesItem> Items;
        public BytesOrderLogTimeSeries()
        {
            Items = new List<BytesOrderLogTimeSeriesItem>();
        }
    }
    public class BytesTickerBarSeries : TimeSeries
    {
        public ICollection<BytesTickerBarSeriesItem> Items;
        public BytesTickerBarSeries()
        {
            Items = new List<BytesTickerBarSeriesItem>();
        }
    }
    public class BytesTickerTickSeries : TimeSeries
    {
        public ICollection<BytesTickerTickSeriesItem> Items;
        public BytesTickerTickSeries()
        {
            Items = new List<BytesTickerTickSeriesItem>();
        }
    }
    public class BytesTickerOrderLogSeries : TimeSeries
    {
        public ICollection<BytesTickerOrderLogSeriesItem> Items;
        public BytesTickerOrderLogSeries()
        {
            Items = new List<BytesTickerOrderLogSeriesItem>();
        }
    }
    #endregion

    #region TimeSeriesItems Series

    public abstract class TimeSeriesItem
    {
        [ForeignKey("TimeSeriesId")]
        public virtual TimeSeries TimeSeries { get; set; }
        public long TimeSeriesId { get; set; }
        public long Id { get; set; }
        [Required]
        public DateTime DT { get; set; }
    }

    public abstract class TimeSeriesItemBase : TimeSeriesItem
    {
    }

    [Table("BarItems")]
    public class Bar : TimeSeriesItemBase // TimeSeriesItem //, IBarDb
    {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        // public string Key => TimeSeriesId + DT.ToString("s");

        public override string ToString()
        {
            return $"DT:{DT.ToString("g")} O:{Open} H:{High} L:{Low} C:{Close}";
        }
    }
    [Table("TickItems")]
    public class Tick : TimeSeriesItemBase // TimeSeriesItem
    {
        public double Last { get; set; }
    }
    public abstract class BytesTimeSeriesItemsBase : TimeSeriesItem
    {
        public TimeIntervalEnum TimeInterval { get; set; }
        public string Format { get; set; }
        public long Count { get; set; }
        public string CheckSum { get; set; }
        public byte[] Bytes { get; set; }
        public DateTime FirstDateTime { get; set; }
        public DateTime LastDateTime { get; set; }
    }
    public enum TimeIntervalEnum { All, Daily, Weekly, Monthly, Quartely, Annualy };

    [Table("BytesBarItems")]
    public class BytesBarTimeSeriesItem : BytesTimeSeriesItemsBase
    {}
    [Table("BytesTickItems")]
    public class BytesTickTimeSeriesItem : BytesTimeSeriesItemsBase
    {}
    [Table("BytesOrderLogItems")]
    public class BytesOrderLogTimeSeriesItem : BytesTimeSeriesItemsBase
    {}
    [Table("BytesTickerBarItems")]
    public class BytesTickerBarSeriesItem : BytesTimeSeriesItemsBase
    {}
    [Table("BytesTickerTickItems")]
    public class BytesTickerTickSeriesItem : BytesTimeSeriesItemsBase
    {}
    [Table("BytesTickerOrderLogItems")]
    public class BytesTickerOrderLogSeriesItem : BytesTimeSeriesItemsBase
    {}
    #endregion
}
