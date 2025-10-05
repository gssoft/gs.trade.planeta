using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;

using Ticker = GS.Trade.TimeSeries.FortsTicks2.Model.Ticker;

namespace GS.Trade.TimeSeries.FortsTicks2.Model
{
    public class Ticker : DbElement<long>
    {
        [Required]
        [MaxLength(128)]
        public string Contract { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public short Decimals { get; set; }
        public short DaysToStore { get; set; }
        public string TickerKey => Contract;

        public Ticker()
        {
            ExpirationDateTime = (DateTime) SqlDateTime.MinValue;
            DaysToStore = 120;
        }

        public override string ToString()
        {
            return $"ID: {ID}, " +
                   $"Code: {Code}, Contract: {Contract}, Key: {Contract}, " +
                   $"Decimals: {Decimals}, DaysToStore: {DaysToStore}, " +
                   $"Name: {Name}, Description: {Description}, " +
                   $"Expiration: {ExpirationDateTime.ToString("g")}, " + 
                   $"Created: {Created}, Modified: {Modified}";
        }
    }

    public abstract class TimeSeries : DbElement<int>  // , ITimeSeriesBase3
    {
        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }
        public long TickerID { get; set; }

        [ForeignKey("FileID")]
        public virtual File File { get; set; }
        public int FileID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        // public int Date { get; set; }

        public int TimeInt { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }

        public DateTime LastDT { get; set; }

        // public TimeSeriesTypeEnum TimeSeriesType { get; set; }

        public string Key => "TickSeries" + (Ticker != null ? Ticker.TickerKey : "Ticker") + "@" +
                             TimeInt
                             ;
        public string DateStr => Date.ToString("d");
    }

    [Table("BarSeries")]
    public sealed class BarSeries : TimeSeries
    {
        public ICollection<Bar> Items { get; set; }

        public BarSeries()
        {
            Items = new List<Bar>();
        }
}
    [Table("TickSeries")]
    public sealed class TickSeries : TimeSeries
    {
        public ICollection<Tick> Items { get; set; }

        public TickSeries()
        {
            Items = new List<Tick>();
        }
    }

    public class Tick : ITickForts2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TradeID { get; set; }
        public int SeriesID { get; set; }
        public DateTime DT { get; set; }
        [ForeignKey("SeriesID")]
        public virtual TickSeries Series { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
    public class Bar : IBarBase
    {
        [Key]
        [Column(Order = 1)]
        public int SeriesID { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime DT { get; set; }

        [ForeignKey("SeriesID")]
        public virtual BarSeries Series { get; set; }

        //public int SrcFileID { get; set; }
        //[ForeignKey("SrcFileID")]
        //public virtual SrcFile File { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public double Volume { get; set; }

        public string Key => string.Join("@", SeriesID, DT.ToString("s"));
    }

    public class Stat : CreateModifyTracking// : ITimeSeriesStat
    {
        [Key]
        public int ID { get; set; }

        public TimeSeriesTypeEnum TimeSeriesType { get; set; }

        [ForeignKey("TickerID")]
        public virtual Ticker Ticker { get; set; }
        public long TickerID { get; set; }

        public StatTimeIntType Period { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastDate { get; set; }
        //public int LastDate { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }
        public DateTime LastDT { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Avg { get; set; }
        public double StDev { get; set; }

        public int Days => 1 + (LastDT.Date - FirstDT.Date).Days;

        public override string ToString()
        {
            return
                $"ID: {ID}," +
                $" SeriesType: {TimeSeriesType}, TimeIntType: {Period}," +
                $" LastDateKey: {LastDate}, Count: {Count}, FirstDT: {FirstDT}, LastDT: {LastDT}," +
                $" MinValue: {MinValue}, MaxValue{MaxValue}," +
                $" Avg: {Avg}, StDev: {StDev}," + 
                $" Days: {Days}," +
                $" Modified: {Modified}" +
                $" Created: {Created}"
                ;
        }
        public string LastDateStr => LastDate.ToString("d");
    }

    public class File : CreateModifyTracking
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        public long Count { get; set; }
        public DateTime FirstDT { get; set; }

        public DateTime LastDT { get; set; }

        // public virtual ICollection<Tick> Ticks { get; set; }
       // public virtual ICollection<Bar> Bars { get; set; }
    }
}
