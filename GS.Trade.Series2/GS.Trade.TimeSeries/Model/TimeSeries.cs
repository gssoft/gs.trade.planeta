using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.TimeSeries.Model
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
    [Table("TradeBoards")]
    public class TradeBoard : DbElement1
    {
        public string TradeBoardKey { get { return Code; } }
        public virtual ICollection<Ticker> Tickers { get; set; }
    }

    public class Ticker : DbElement1
    {
        [ForeignKey("TradeBoardId")]
        public virtual TradeBoard TradeBoard { get; set; }

        public long TradeBoardId { get; set; }
        public virtual IList<TimeSeries> TimeSeries { get; set; }

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
        //public string TimeIntKey
        //{
        //    get { return Code.HasValue() + (TimeShift != 0 ? "@" + TimeShift : ""); }
        //}
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
    public abstract class TimeSeriesItem
    {
        public long Id { get; set; }

        //[ForeignKey("TimeSeriesId")]
        //public virtual TimeSeries TimeSeries { get; set; }
        //public long TimeSeriesId { get; set; }

        public DateTime DT { get; set; }
    }
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
    public class Bar : TimeSeriesItem, IBarDb
    {
        //public long Id { get; set; }

        //[ForeignKey("BarSeriesId")]
        //public virtual BarSeries TimeSeries { get; set; }
        //public long TimeSeriesId { get; set; }

        //[ForeignKey("BarSeriesId")]
        //public virtual BarSeries TimeSeries { get; set; }
        //public long BarSeriesId { get; set; }

        [ForeignKey("BarSeriesId")]
        public virtual TimeSeries TimeSeries { get; set; }
        public long BarSeriesId { get; set; }

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
            get { return BarSeriesId + DT.ToString("s"); }
        }
    }
    //[Table("Ticks")]
    public class Tick : TimeSeriesItem
    {
        //public long Id { get; set; }

        //[ForeignKey("TickSeriesId")]
        //public virtual TickSeries TimeSeries { get; set; }
        //public long TimeSeriesId { get; set; }

        [ForeignKey("TickSeriesId")]
        public virtual TimeSeries TimeSeries { get; set; }
        public long TickSeriesId { get; set; }

        public double Last { get; set; }
    }
    public class QuoteProvider : DbElement1
    {
        public string QuoteProviderKey
        {
            get { return Code; }
        }
        public virtual ICollection<TimeSeries> TimeSeries { get; set; }
    }
}
