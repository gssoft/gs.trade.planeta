using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Extension;
using GS.Moex.Interfaces;

namespace Moex
{
    [XmlRoot(ElementName = "row")]
    public class MoexTicker : IMoexTicker
    {
        [XmlIgnore]
        public MoexTickerTypeEnum TickerType { get; set; }
    
        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "BOARDID")]
        public string BoardId { get; set; }
        [XmlAttribute(AttributeName = "SHORTNAME")]
        public string ShortName { get; set; }
        [XmlAttribute(AttributeName = "SECNAME")]
        public string SecName { get; set; }
        // public double PrevSettlePrice { get; set; }
        // public string Status { get; set; }
        [XmlAttribute(AttributeName = "DECIMALS")]
        public int Decimals { get; set; }
        [XmlAttribute(AttributeName = "MINSTEP")]
        public double MinStep { get; set; }
        [XmlAttribute(AttributeName = "SECTYPE")]
        public string SecType { get; set; }
        [XmlAttribute(AttributeName = "LATNAME")]
        public string LatName { get; set; }
        [XmlAttribute(AttributeName = "ASSETCODE")]
        public string AssetCode { get; set; }
        [XmlAttribute(AttributeName = "PREVOPENPOSITION")]
        public string PrevOpenPositionStr { get; set; }
        [XmlIgnore]
        public double PrevOpenPosition { get; set; }
        [XmlAttribute(AttributeName = "LOTVOLUME")]
        public double LotVolume { get; set; }
        // public double PrevOpenPosition { get; set; }
        // public double LotVolume { get; set; }
        [XmlAttribute(AttributeName = "INITIALMARGIN")]
        public double InitialMargin { get; set; }
        [XmlAttribute(AttributeName = "HIGHLIMIT")]
        public double HighLimit { get; set; }
        [XmlAttribute(AttributeName = "LOWLIMIT")]
        public double LowLimit { get; set; }
        [XmlAttribute(AttributeName = "STEPPRICE")]
        public double StepPrice { get; set; }
        [XmlAttribute(AttributeName = "LASTSETTLEPRICE")]
        public double LastSettlePrice { get; set; }
        [XmlAttribute(AttributeName = "PREVSETTLEPRICE")]
        public double PrevSettlePrice { get; set; }
        [XmlAttribute(AttributeName = "PREVPRICE")]
        public string PrevPriceStr { get; set; }
        [XmlIgnore]
        public double PrevPrice { get; set; }
        [XmlAttribute(AttributeName = "FIRSTTRADEDATE")]
        public string FirstTradeDateStr { get; set; }
        [XmlIgnore]
        public DateTime FirstTradeDate { get; set; }
        [XmlAttribute(AttributeName = "LASTTRADEDATE")]
        public string LastTradeDateStr { get; set; }
        [XmlIgnore]
        public DateTime LastTradeDate { get; set; }
        public bool IsFutures => TickerType == MoexTickerTypeEnum.Futures;
        public bool IsOption => TickerType == MoexTickerTypeEnum.Option;
        //public bool IsFutures => BoardId.Equals("RFUD", StringComparison.InvariantCultureIgnoreCase);
        //public bool IsOption => BoardId.Equals("ROPD", StringComparison.InvariantCultureIgnoreCase);
        public double HighestPossiblePrice => LastSettlePrice + HighLimit;
        public double LowestPossiblePrice => LastSettlePrice - LowLimit;
        private void SetInstrumentType()
        {
            switch (BoardId)
            {
                case "RFUD": TickerType = MoexTickerTypeEnum.Futures; break;
                case "ROPD": TickerType = MoexTickerTypeEnum.Option; break;
                default: TickerType = MoexTickerTypeEnum.Unknown; break;
            }
        }
        private void ParseDateTimeFields()
        {
            try
            {
                FirstTradeDate = FirstTradeDateStr.HasValue() 
                    ? DateTime.Parse(FirstTradeDateStr) 
                    : DateTime.MinValue;

                LastTradeDate = LastTradeDateStr.HasValue() 
                    ? DateTime.Parse(LastTradeDateStr) 
                    : DateTime.MinValue;
            }
            catch (Exception e)
            {
                FirstTradeDate = DateTime.MaxValue;
                LastTradeDate = DateTime.MaxValue;
                ConsoleSync.WriteLineT(e.Message); 
            }
        }
        private void ParseSomeFields()
        {
            if(string.IsNullOrWhiteSpace(PrevPriceStr)) PrevPriceStr = "0";
            PrevPrice = double.Parse(PrevPriceStr.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(PrevOpenPositionStr)) PrevOpenPositionStr = "0";
            PrevOpenPosition = double.Parse(PrevOpenPositionStr.Trim().Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture);
        }
        public void Init()
        {
            SetInstrumentType();
            ParseDateTimeFields();
            ParseSomeFields();  
        }
        public string ShortInfo =>
            $"SecId:{SecId} SecType:{SecType} InstrType:{TickerType} ShortName:{ShortName} SecName:{SecName} " +
            $"Decimals:{Decimals} MinStep:{MinStep} StepPrice:{StepPrice} InitialMargin:{InitialMargin}";
        public override string ToString()
        {
            return $"SecId:{SecId} BoardId:{BoardId} ShortName:{ShortName} InstrType:{TickerType} SecName:{SecName} " +
                   $"Decimals:{Decimals} MinStep:{MinStep} SecType:{SecType} LatName:{LatName} " +
                   $"InitialMargin:{InitialMargin} StepPrice:{StepPrice} FirstDate:{FirstTradeDate.ToString("d")} LastDate:{LastTradeDate.ToString("d")}";
        }
    }
    [XmlRoot(ElementName = "row")]
    public class MoexFuture : IMoexTicker
    {
        [XmlIgnore]
        public MoexTickerTypeEnum TickerType { get; set; }

        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "BOARDID")]
        public string BoardId { get; set; }
        [XmlAttribute(AttributeName = "SHORTNAME")]
        public string ShortName { get; set; }
        [XmlAttribute(AttributeName = "SECNAME")]
        public string SecName { get; set; }
        // public double PrevSettlePrice { get; set; }
        // public string Status { get; set; }
        [XmlAttribute(AttributeName = "DECIMALS")]
        public int Decimals { get; set; }
        [XmlAttribute(AttributeName = "MINSTEP")]
        public double MinStep { get; set; }
        // public string LastTradeDate { get; set; }
        //public string LastDelDate { get; set; }
        [XmlAttribute(AttributeName = "SECTYPE")]
        public string SecType { get; set; }
        [XmlAttribute(AttributeName = "LATNAME")]
        public string LatName { get; set; }
        [XmlAttribute(AttributeName = "ASSETCODE")]
        public string AssetCode { get; set; }
        [XmlAttribute(AttributeName = "LOTVOLUME")]
        public double LotVolume { get; set; }
        // public double PrevOpenPosition { get; set; }
        // public double LotVolume { get; set; }
        [XmlAttribute(AttributeName = "INITIALMARGIN")]
        public double InitialMargin { get; set; }
        //[XmlAttribute(AttributeName = "HIGHLIMIT")]
        //public double HighLimit { get; set; }
        //[XmlAttribute(AttributeName = "LOWLIMIT")]
        //public double LowLimit { get; set; }
        [XmlAttribute(AttributeName = "STEPPRICE")]
        public double StepPrice { get; set; }
        //[XmlAttribute(AttributeName = "LASTSETTLEPRICE")]
        //public double LastSettlePrice { get; set; }
        //[XmlAttribute(AttributeName = "PREVSETTLEPRICE")]
        //public double PrevSettlePrice { get; set; }
        [XmlAttribute(AttributeName = "PREVPRICE")]
        public double PrevPrice { get; set; }
        [XmlAttribute(AttributeName = "FIRSTTRADEDATE")]
        public string FirstTradeDateStr { get; set; }
        public DateTime FirstTradeDate { get; set; }
        [XmlAttribute(AttributeName = "LASTTRADEDATE")]
        public string LastTradeDateStr { get; set; }
        public DateTime LastTradeDate { get; set; }
        public bool IsFutures => TickerType == MoexTickerTypeEnum.Futures;
        public bool IsOption => TickerType == MoexTickerTypeEnum.Option;
        //public bool IsFutures => BoardId.Equals("RFUD", StringComparison.InvariantCultureIgnoreCase);
        //public bool IsOption => BoardId.Equals("ROPD", StringComparison.InvariantCultureIgnoreCase);
        //public double HighestPossiblePrice => LastSettlePrice + HighLimit;
        //public double LowestPossiblePrice => LastSettlePrice - LowLimit;
        private void SetInstrumentType()
        {
            switch (BoardId)
            {
                case "RFUD": TickerType = MoexTickerTypeEnum.Futures; break;
                case "ROPD": TickerType = MoexTickerTypeEnum.Option; break;
                default: TickerType = MoexTickerTypeEnum.Unknown; break;
            }
        }
        private void ParseDateTimeFields()
        {
            try
            {
                FirstTradeDate = FirstTradeDateStr.HasValue()
                    ? DateTime.Parse(FirstTradeDateStr)
                    : DateTime.MinValue;

                LastTradeDate = LastTradeDateStr.HasValue()
                    ? DateTime.Parse(LastTradeDateStr)
                    : DateTime.MinValue;
            }
            catch (Exception e)
            {
                FirstTradeDate = DateTime.MaxValue;
                LastTradeDate = DateTime.MaxValue;
                ConsoleSync.WriteLineT(e.Message);
            }
        }
        public void Init()
        {
            SetInstrumentType();
            ParseDateTimeFields();
        }
        public string ShortInfo =>
            $"SecId:{SecId} SecType:{SecType} InstrType:{TickerType} ShortName:{ShortName} SecName:{SecName} " +
            $"Decimals:{Decimals} MinStep:{MinStep} StepPrice:{StepPrice} InitialMargin:{InitialMargin}";
        public override string ToString()
        {
            return $"SecId:{SecId} BoardId:{BoardId} ShortName:{ShortName} InstrType:{TickerType} SecName:{SecName} " +
                   $"Decimals:{Decimals} MinStep:{MinStep} SecType:{SecType} LatName:{LatName} " +
                   $"InitialMargin:{InitialMargin} StepPrice:{StepPrice} FirstDate:{FirstTradeDate.ToString("d")} LastDate:{LastTradeDate.ToString("d")}";
        }
    }
    [XmlRoot(ElementName = "row")]
    public class MoexOption : IMoexTicker
    {
        [XmlIgnore]
        public MoexTickerTypeEnum TickerType { get; set; }

        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "BOARDID")]
        public string BoardId { get; set; }
        [XmlAttribute(AttributeName = "SHORTNAME")]
        public string ShortName { get; set; }
        [XmlAttribute(AttributeName = "SECNAME")]
        public string SecName { get; set; }
        // public double PrevSettlePrice { get; set; }
        // public string Status { get; set; }
        [XmlAttribute(AttributeName = "DECIMALS")]
        public int Decimals { get; set; }
        [XmlAttribute(AttributeName = "MINSTEP")]
        public double MinStep { get; set; }
        // public string LastTradeDate { get; set; }
        //public string LastDelDate { get; set; }
        [XmlAttribute(AttributeName = "SECTYPE")]
        public string SecType { get; set; }
        [XmlAttribute(AttributeName = "LATNAME")]
        public string LatName { get; set; }
        [XmlAttribute(AttributeName = "ASSETCODE")]
        public string AssetCode { get; set; }
        [XmlAttribute(AttributeName = "LOTVOLUME")]
        public double LotVolume { get; set; }
        // public double PrevOpenPosition { get; set; }
        // public double LotVolume { get; set; }
        [XmlAttribute(AttributeName = "INITIALMARGIN")]
        public double InitialMargin { get; set; }
        [XmlAttribute(AttributeName = "HIGHLIMIT")]
        public double HighLimit { get; set; }
        [XmlAttribute(AttributeName = "LOWLIMIT")]
        public double LowLimit { get; set; }
        [XmlAttribute(AttributeName = "STEPPRICE")]
        public double StepPrice { get; set; }
        [XmlAttribute(AttributeName = "LASTSETTLEPRICE")]
        public double LastSettlePrice { get; set; }
        [XmlAttribute(AttributeName = "PREVSETTLEPRICE")]
        public double PrevSettlePrice { get; set; }
        [XmlAttribute(AttributeName = "PREVPRICE")]
        public double PrevPrice { get; set; }
        [XmlAttribute(AttributeName = "FIRSTTRADEDATE")]
        public string FirstTradeDateStr { get; set; }
        public DateTime FirstTradeDate { get; set; }
        [XmlAttribute(AttributeName = "LASTTRADEDATE")]
        public string LastTradeDateStr { get; set; }
        public DateTime LastTradeDate { get; set; }
        public bool IsFutures => TickerType == MoexTickerTypeEnum.Futures;
        public bool IsOption => TickerType == MoexTickerTypeEnum.Option;
        //public bool IsFutures => BoardId.Equals("RFUD", StringComparison.InvariantCultureIgnoreCase);
        //public bool IsOption => BoardId.Equals("ROPD", StringComparison.InvariantCultureIgnoreCase);
        public double HighestPossiblePrice => LastSettlePrice + HighLimit;
        public double LowesrPossiblePrice => LastSettlePrice - LowLimit;
        private void SetInstrumentType()
        {
            switch (BoardId)
            {
                case "RFUD": TickerType = MoexTickerTypeEnum.Futures; break;
                case "ROPD": TickerType = MoexTickerTypeEnum.Option; break;
                default: TickerType = MoexTickerTypeEnum.Unknown; break;
            }
        }
        private void ParseDateTimeFields()
        {
            try
            {
                FirstTradeDate = FirstTradeDateStr.HasValue()
                    ? DateTime.Parse(FirstTradeDateStr)
                    : DateTime.MinValue;

                LastTradeDate = LastTradeDateStr.HasValue()
                    ? DateTime.Parse(LastTradeDateStr)
                    : DateTime.MinValue;
            }
            catch (Exception e)
            {
                FirstTradeDate = DateTime.MaxValue;
                LastTradeDate = DateTime.MaxValue;
                ConsoleSync.WriteLineT(e.Message);
            }
        }
        public void Init()
        {
            SetInstrumentType();
            ParseDateTimeFields();
        }
        public string ShortInfo =>
            $"SecId:{SecId} SecType:{SecType} InstrType:{TickerType} ShortName:{ShortName} SecName:{SecName} " +
            $"Decimals:{Decimals} MinStep:{MinStep} StepPrice:{StepPrice} InitialMargin:{InitialMargin}";
        public override string ToString()
        {
            return $"SecId:{SecId} BoardId:{BoardId} ShortName:{ShortName} InstrType:{TickerType} SecName:{SecName} " +
                   $"Decimals:{Decimals} MinStep:{MinStep} SecType:{SecType} LatName:{LatName} " +
                   $"InitialMargin:{InitialMargin} StepPrice:{StepPrice} FirstDate:{FirstTradeDate.ToString("d")} LastDate:{LastTradeDate.ToString("d")}";
        }
    }
    [XmlRoot(ElementName = "row")]
    public class MoexMarketDataItem
    {
        [XmlIgnore]
        public MoexTickerTypeEnum TickerType;
        [XmlAttribute(AttributeName = "SECID")]
        public string SecId { get; set; }
        [XmlAttribute(AttributeName = "BOARDID")]
        public string BoardId { get; set; }
        [XmlAttribute(AttributeName = "BID")]
        public double Bid { get; set; }
        [XmlAttribute(AttributeName = "OFFER")]
        public double Offer { get; set; }
        [XmlAttribute(AttributeName = "SPREAD")]
        public double Spread { get; set; }
        [XmlAttribute(AttributeName = "OPEN")]
        public double Open { get; set; }
        [XmlAttribute(AttributeName = "HIGH")]
        public double High { get; set; }
        [XmlAttribute(AttributeName = "LOW")]
        public double Low { get; set; }
        [XmlAttribute(AttributeName = "LAST")]
        public double Last { get; set; }
        [XmlAttribute(AttributeName = "QUANTITY")]
        public double Quantity { get; set;}
        [XmlAttribute(AttributeName = "LASTCHANGE")]
        public double LastChange { get; set; }
        [XmlAttribute(AttributeName = "SETTLEPRICE")]
        public double SettlePrice { get; set; }
        [XmlAttribute(AttributeName = "SETTLETOPREVSETTLE")]
        public double SettleToPrevSettle { get; set; }
        [XmlAttribute(AttributeName = "OPENPOSITION")]
        public double OpenPositions { get; set; }
        [XmlAttribute(AttributeName = "NUMTRADES")]
        public double NumTrades { get; set; }
        [XmlAttribute(AttributeName = "VOLTODAY")]
        public double VolToday { get; set; }
        [XmlAttribute(AttributeName = "VALTODAY_USD")]
        public double ValTodayUsd {get; set;}
        [XmlAttribute(AttributeName = "UPDATETIME")]
        public string UpdateTimeStr { get; set; }
        [XmlAttribute(AttributeName = "LASTCHANGEPRCNT")]
        public string LastChangePrcnt { get; set; }
        [XmlAttribute(AttributeName = "BIDDEPTH")]
        public double BidDepth { get; set; }
        [XmlAttribute(AttributeName = "BIDDEPTHT")]
        public double BidDeptht { get; set; }
        [XmlAttribute(AttributeName = "NUMBIDS")]
        public double NumBids { get; set; }
        [XmlAttribute(AttributeName = "OFFERDEPTH")]
        public double OfferDepth { get; set; }
        [XmlAttribute(AttributeName = "OFFERDEPTHT")]
        public double OfferDeptht { get; set; }
        [XmlAttribute(AttributeName = "NUMOFFERS")]
        public double NumOffers { get; set; }
        [XmlAttribute(AttributeName = "TIME")]
        public string TimeStr { get; set; }
        [XmlAttribute(AttributeName = "SETTLETOPREVSETTLEPRC")]
        public double SettleToPrevSettlePrc { get; set; }
        [XmlAttribute(AttributeName = "SEQNUM")]
        public double SetqNum { get; set; }
        [XmlAttribute(AttributeName = "SYSTIME")]
        public string DateTimeStr { get; set; }
        [XmlAttribute(AttributeName = "TRADEDATE")]
        public string TradeDateStr { get; set; }
        [XmlAttribute(AttributeName = "LASTTOPREVPRICE")]
        public double LastToPrevPrc { get; set; }
        [XmlAttribute(AttributeName = "OICHANGE")]
        public double OiChange { get; set; }
    }
}
