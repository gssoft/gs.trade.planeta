using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GS.Elements;
using GS.Interfaces;
using GS.Trade.Dto;
using GS.Moex.Interfaces;
using Moex;

namespace GS.Trade
{
   
    public interface ITickers3 : ITickers
    {
       
    }

    public delegate void NewTickEventHandler(DateTime dt, string tickerkey, double last, double bid, double ask);
    public interface ITickers : IElement1<string>
    {
        event NewTickEventHandler NewTickEvent;

        IEnumerable<ITicker> GetTickers { get; }
        ITicker  GetTicker(string key);

        //ITicker Register(string board, string code);
        ITicker Register(ITicker t);

        void PutDdeQuote3(string tick);
        void PutDdeQuote5(string tick);
        IEnumerable<ITicker> TickerCollection { get; }

        void PushDdeQuoteStr(string s);
        void DeQueueDdeQuoteStrProcess();

        void PushDdeQuoteStr1(string s);
        void PushDdeQuoteListStr(List<string> listOfstrings);
        void DeQueueDdeQuoteStrProcess1();

        void PushDdeQuoteStr2(string s);
        void DeQueueDdeQuoteStrProcess2();

        void UpdateTimeSeries(DateTime dt);
        void UpdateAsyncSeries(string tickerKey , IBarSimple b);

        void AddTicker(ITicker t);
        void Add(ITicker t);

        void ClearSomeData(int count);
        void Init(IEventLog evl);

        void LoadBarsFromArch();
        void Load();

        IEnumerable<IBarSimple> GetSeries(string queryString);
        IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt);
        IEnumerable<IBarSimple> GetSeries(string ticker, int timeInt, DateTime dt);

        TimeSeriesStat GetTimeSeriesStat(string ticker, int timeInt);

        ITicker CreateInstance(ITickerBase t);
        ITicker CreateInstanceFromMoex(IMoexTicker t);

        //IEnumerable<ITicker> TickerCollection { get; }
        void Start();
        void Stop();
        void QuoteStringsProcessing(IEnumerable<string> quotestrings);
    }

    public interface ITickers2 : ITickers
    {
        void DeQueueDdeQuoteStrProcess11();
    }

    //    <Ticker>
    // <ID>1</ID>
    //<Code>RIZ3</Code>
    //<ClassCode>SPBFUT</ClassCode>
    //<BaseContract>RTS</BaseContract>
    //<Name>RTSI September,2011</Name>
    //<Key>RTSI_1109</Key>
    //<Symbol>F_RTSI</Symbol>
    //<Decimals>0</Decimals>
    //<FormatF>F0</FormatF>
    //<Format>N0</Format>
    //<FormatAvg>N2</FormatAvg>
    //<FormatM>N2</FormatM>
    //<MinMove>10</MinMove>
    //<From>2011-03-15T10:00:00</From>
    //<To>2011-06-15T18:45:00</To>
    //<IsNeedLoadFromDataBase>false</IsNeedLoadFromDataBase>
    //<LoadMode>2</LoadMode>

    public interface ITickerBase : Containers5.IHaveKey<string>, IHaveId<int>
    {
        string Name { get; set; }
        string Code { get; set; }
        string Alias { get; set; }
        string TradeBoard { get; set; }
        string BaseContract { get; set; }
        float MinMove { get; set; }
        int Decimals { get; set; }
        decimal Margin { get; set; }
        float PriceLimit { get; set; }
        string FormatF { get; }
        string Format { get; }
        string FormatAvg { get; }
        string FormatM { get; }
        // DateTime FirstTradeDate { get; }
        // DateTime LastTradeDate { get; }
    }
    public interface ITicker : ITickerBase //, IElement1<string>
    {
        IElement1<string> Parent { get; set; }
        TickerTradeTypeEnum TickerTradeType { get; }
        string ClassCode { get; set; }
        string ShortName { get; set; }
        DateTime FirstTradeDate { get; }
        DateTime LastTradeDate { get; }
        double LastPrice { get; }
        double Bid { get; }
        double Ask { get; }
        double BestBid { get; }
        double BestAsk { get; }
        double Delta { get; }
        void SetLast(IBarSimple b);
        double MarketPriceBuy { get; }
        double MarketPriceSell { get; }
        double ToMinMove(double price, int direction);
        void RegisterBarSeries(string name, int timeInt, int shift);
        ITimeSeries RegisterTimeSeries(ITimeSeries ts);
        ITimeSeries RegisterAsyncSeries(ITimeSeries ts);
        IEventLog EventLog { get; set; }
        ITickers Tickers { get; set; }
        void ClearBarSeries();
        void ClearTimeSeries();
        void ClearSomeData( int count);

        // 15/10/03
        IEnumerable<ITimeSeries> BarSeries { get; }
        IEnumerable<ITimeSeries> TimeSeries { get; }

        IEnumerable<IBarSimple> GetSeries(int timeInt);
        IEnumerable<IBarSimple> GetSeries(int timeInt, DateTime dt);
        // 15.11.11
        ITimeSeries GetTimeSeries(string key);

        //15.10.13
        TimeSeriesStat GetTimeSeriesStat(int timeInt);

        // 15.10.08
        void UpdateAsyncSeries(IBarSimple b);
        void UpdateAsyncSeries2(IBarSimple b);
        void UpdateTimeSeries(DateTime syncDT);
    }
    public interface ITickerDb : ITickerBase
    {
    }

    public interface IOptionTicker : ITicker
    {
        OptionTypeEnum OptionType { get; }
        double Strike { get;}
        string BaseAssetCode { get; }
        double BaseAssetPrice { get; }
        DateTime ExpirationDate { get; }
        double Volatility { get; }
        // double Delta { get; }
        double Gamma { get; }
        double Theta { get; }
        double Vega { get; }
        double Rho { get; }
        // double Bid {};
        // double Ask {};
        double TheoryPrice { get; }
        long OpenInterest { get; }
        int TradesCount { get; }
    }
}
