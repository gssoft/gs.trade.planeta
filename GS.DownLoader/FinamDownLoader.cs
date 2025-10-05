using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.QuoteDownLoader;
//using SG.Trade.EventLog;


namespace GS.Trade.DownLoader
{
    public partial class FinamDownLoader
    {        
        public delegate void BarItemToSave( string ticker, string timeint, DateTime dt, 
                                            double open, double high, double low, double close,
                                            decimal volume);
        [XmlIgnore]
        private readonly BarItemToSave _cbBarItemSave;

        private readonly IEventLog _evl = new GS.EventLog.FileEventLog("DownLoad.log");

        // public  string ConnectionString;
        private const string ConnectionString 
        = @"http://195.128.78.52/quotes.txt?d=d&m=1&em=IdSymbol&p=IdBarInt&df=DayFrom&mf=MonthFrom&yf=YearFrom&dt=DayTo&mt=MonthTo&yt=YearTo&f=quote.&e=.txt&dtf=1&tmf=1&MSOR=1&cn=SymbolName&sep=1&sep2=1&datf=1";

        public string RemoteHostIP { get; private set; }
        public  string RemoteHostName { get; private set; }
        
        private  int _daysLoadPerPass = 360;
        
        private readonly Dictionary<string, Bar> _bars = new Dictionary<string, Bar>();
        public IEnumerable<Bar> Bars { get { return _bars.Values; } }

        public Dictionary<string, Ticker> TickerDictionary = new Dictionary<string, Ticker>();
        public Dictionary<string, TimeInt> TimeIntDictionary = new Dictionary<string, TimeInt>();

        public event BarItemToSave SaveBarItem;

        public FinamDownLoader()
        {
          RemoteHostIP = @"http://195.128.78.52";
          RemoteHostName = @"http://www.finam.ru";

          LoadTickersXml();
          LoadTimeIntsXml();
        }
        public FinamDownLoader( IEventLog evl, BarItemToSave cbBarItemSave)
        {
            if (cbBarItemSave == null || evl == null)
            {
                throw new NullReferenceException("Callback refference or EventLog is null");
            }
            _cbBarItemSave = cbBarItemSave;
            _evl = evl;

            _bars = new Dictionary<string, Bar>();

        }

        public FinamDownLoader( string connstr, IEventLog evl, BarItemToSave cbBarItemSave)
        {
           // ConnectionString = connstr;

            if (cbBarItemSave == null || evl == null )
            {
                throw new NullReferenceException("Callback refference or EventLog is null");
            }
            _cbBarItemSave = cbBarItemSave;
            _evl = evl;

           _bars = new Dictionary<string, Bar>();
        }
        public int DownLoad(string tickerStr, string timeIntStr, int tickerid, int timeintid, DateTime dtmin, DateTime dtmax )
        {            
            var tickerID = tickerid;
            if( tickerID == 0) 
            {
                if (!string.IsNullOrWhiteSpace(tickerStr))
                {
                    Ticker ticker;
                    if ( (ticker = GetTicker(tickerStr)) == null)
                        throw new NullReferenceException("TickerID - not Found");

                    tickerID = ticker.ID;
                }
                else
                {
                    throw new NullReferenceException("Ticker or TickerID - should be define");    
                }
            }
     
            var timeIntID = timeintid;
            if (timeIntID == 0) 
            {
                if (!string.IsNullOrWhiteSpace(timeIntStr))
                {
                    TimeInt timeInt;
                    if (( timeInt = GetTimeInt(timeIntStr)) == null)
                        throw new NullReferenceException("TimeIntID - not Found");

                    timeIntID = timeInt.ID;  // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
                else
                {
                    throw new NullReferenceException("TimeInt or TimeIntID - should be define");
                }
            }
            
            _bars.Clear();

           // _evl.AddItem(EvlResult.SUCCESS, "Start DownLoad",
           //                        String.Format("Ticker={0} ID={1} TimeInt={2} ID={3} from {4} to {5}",
           //                                         ticker, tickerID, timeint, timeIntID,  dtmin, dtmax));

            var dtMin = dtmin;  // Date (and Time - in IntraDay) that we start
            var dtMax = dtmax;  // with Time

            var dt1 = dtMin; // with Time

            var dt2 = dt1.AddDays(_daysLoadPerPass - 1);
            if (dt2 > dtMax) dt2 = dtMax;

            while (dt1 <= dt2)
            {             
                var conString = GetConString( tickerStr, tickerID, timeIntID, dt1, dt2);

               // Console.WriteLine("{0} {1} {2} {3}", tickerID, timeIntID, dt1, dt2);
               // Console.WriteLine("{0}", conString);
                // Console.ReadLine();

                var cnt = DownLoad(conString, dt1);
                if ( cnt >= -1)
                {
                    if (cnt >= 0)
                        if(_evl != null)
                            _evl.AddItem(EvlResult.SUCCESS, "DownLoad",
                                   String.Format("Ticker={0} ID={1} TimeIntID={2} from {3} to {4} Count={5}",
                                                    tickerStr, tickerID, timeIntID, dt1.Date, dt2.Date, cnt));

                    dt1 = dt1.Date.AddDays(_daysLoadPerPass);
                    dt2 = dt1.Date.AddDays(_daysLoadPerPass - 1);
                    if (dt2 > dtMax.Date) dt2 = dtMax.Date;
                }
                else
                {
                    return cnt;
                }
            }

            foreach (var b in _bars.Values)
            {
               // _cbBarItemSave(b.Ticker, b.TimeInt, b.DT, b.Open, b.High, b.Low, b.Close, b.Volume);
                if( SaveBarItem != null)
                    SaveBarItem(b.Ticker, b.TimeInt, b.DT, b.Open, b.High, b.Low, b.Close, b.Volume);
            }

            return 0;
        }

        public int DownLoad(string tickerStr, string timeIntStr, DateTime dtmin, DateTime dtmax)
        {
            tickerStr = tickerStr.Trim().ToUpper();
            timeIntStr = timeIntStr.Trim().ToUpper();

            int tickerID;
            Ticker ticker;
            if (!string.IsNullOrWhiteSpace(tickerStr))
            {
                    
                    if ((ticker = GetTicker(tickerStr)) == null)
                        throw new NullReferenceException("TickerID - not Found");

                    tickerID = ticker.ID;
            }
            else
            {
                    throw new NullReferenceException("Ticker or TickerID - should be define");
            }

            int timeIntID;
            TimeInt timeInt;
            if (!string.IsNullOrWhiteSpace(timeIntStr))
            {
                    
                    if ((timeInt = GetTimeInt(timeIntStr)) == null)
                        throw new NullReferenceException("TimeIntID - not Found");

                    timeIntID = timeInt.ID;  // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            else
            {
                    throw new NullReferenceException("TimeInt or TimeIntID - should be define");
            }
            _daysLoadPerPass = timeInt.DaysPerPass;
  
            _bars.Clear();

            // _evl.AddItem(EvlResult.SUCCESS, "Start DownLoad",
            //                        String.Format("Ticker={0} ID={1} TimeInt={2} ID={3} from {4} to {5}",
            //                                         ticker, tickerID, timeint, timeIntID,  dtmin, dtmax));

            var dtMin = dtmin;  // Date (and Time - in IntraDay) that we start
            var dtMax = dtmax;  // with Time

            var dt1 = dtMin; // with Time

            var dt2 = dt1.AddDays(_daysLoadPerPass - 1);
            if (dt2 > dtMax) dt2 = dtMax;

            while (dt1 <= dt2)
            {
                var conString = GetConString(tickerStr, tickerID, timeIntID, dt1, dt2);

                // Console.WriteLine("{0} {1} {2} {3}", tickerID, timeIntID, dt1, dt2);
                // Console.WriteLine("{0}", conString);
                // Console.ReadLine();

                var cnt = DownLoad(conString, dt1);
                if (cnt >= -1)
                {
                    if (cnt >= 0)
                        if (_evl != null)
                            _evl.AddItem(EvlResult.SUCCESS, "DownLoad",
                                         String.Format("Ticker={0} ID={1} TimeInt={2} ID={3} from {4} to {5} Count={6}",
                                                       tickerStr, tickerID, timeIntStr, timeIntID, dt1.Date, dt2.Date, cnt));

                    dt1 = dt1.Date.AddDays(_daysLoadPerPass);
                    dt2 = dt1.Date.AddDays(_daysLoadPerPass - 1);
                    if (dt2 > dtMax.Date) dt2 = dtMax.Date;
                }
                else
                {
                    return cnt;
                }
            }

            foreach (var b in _bars.Values)
            {
                // _cbBarItemSave(b.Ticker, b.TimeInt, b.DT, b.Open, b.High, b.Low, b.Close, b.Volume);
                if (SaveBarItem != null)
                        SaveBarItem(b.Ticker, b.TimeInt, b.DT, b.Open, b.High, b.Low, b.Close, b.Volume);
            }

            return 0;
        }
        
        private static string GetConString(string ticker, int tickerID, int timeIntID, DateTime dt1, DateTime dt2)
        {

            var day1 = dt1.Day.ToString();
            var month1 = (dt1.Month - 1).ToString();
            var year1 = dt1.Year.ToString();

            var day2 = dt2.Day.ToString();
            var month2 = (dt2.Month - 1).ToString();
            var year2 = dt2.Year.ToString();

            //String SymbolID = tickerid.ToString();
            //String BarIntDS = r["idBarIntDS"].ToString();

            var conStr = ConnectionString;

            conStr = conStr.Replace("IdSymbol", tickerID.ToString());
            conStr = conStr.Replace("IdBarInt", timeIntID.ToString());
            conStr = conStr.Replace("SymbolName", ticker);

            conStr = conStr.Replace("DayFrom", day1);
            conStr = conStr.Replace("MonthFrom", month1);
            conStr = conStr.Replace("YearFrom", year1);

            conStr = conStr.Replace("DayTo", day2);
            conStr = conStr.Replace("MonthTo", month2);
            conStr = conStr.Replace("YearTo", year2);

            return conStr;
        }
        private Ticker GetTicker(string tickerstr)
        {
            //return (from p in TickerList where p.Key == ticker.ToUpper() select p.Value).FirstOrDefault();
            Ticker t;
            return (TickerDictionary.TryGetValue(tickerstr, out t)) ? t : null;
        }

        private TimeInt GetTimeInt(string timeIntStr)
        {
            //return (from p in TimeIntList where p.Key == timeInt.ToUpper() select p.Value).FirstOrDefault();
            TimeInt ti;
            return (TimeIntDictionary.TryGetValue(timeIntStr, out ti)) ? ti : null;
        }

        private int DownLoad(string connectionstring, DateTime dtMin)
        {
            var cnt = 0;
            try
            {
                var req = WebRequest.Create(connectionstring);
                var resp = req.GetResponse();
                if (resp != null)
                {
                    var reqStream = resp.GetResponseStream();
                    if (reqStream != null)
                    {
                        var sr = new StreamReader(reqStream);

                        var strLine = sr.ReadLine();

                        while (strLine != null)
                        {
                            cnt++;
                            ParseAndSendLine(strLine, dtMin);
                            strLine = sr.ReadLine();
                        }
                    }
                    else
                    {
                        {
                            if (_evl != null)
                                _evl.AddItem(EvlResult.FATAL, "GetResponseStream is Null", connectionstring);
                            return -1;
                        }
                    }
                }
                else
                {
                    if (_evl != null) _evl.AddItem(EvlResult.FATAL, "GetResponse is Null", connectionstring);
                    return -1;
                }
                return cnt;
            }
            catch (Exception e)
            {
                if (_evl != null)
                {
                    _evl.AddItem(EvlResult.FATAL, "Unable to connect to the remote server", connectionstring);
                    _evl.AddItem(EvlResult.FATAL,e.Message,"");
                }
                return -2;
            }
        }
        private void ParseAndSendLine(string strLine,  DateTime dtMin)
        {
            var EnglishCulture = new System.Globalization.CultureInfo("en-US");
           // System.Globalization.CultureInfo GermanCulture = new System.Globalization.CultureInfo("de-de");

            try
            {
                var split = strLine.Split(new Char[] {' ', ',', ':', ';'});

                var ticker = (split[0]).Trim();
                var timeInt = (split[1]).Trim();
                var date = (split[2]).Trim();
                var time = (split[3]).Trim();
                var open = (split[4]).Trim();
                var high = (split[5]).Trim();
                var low = (split[6]).Trim();
                var close = (split[7]).Trim();
                var volume = (split[8]).Trim();

                var pDate = date.Substring(0, 4) + '-' + date.Substring(4, 2) + '-' + date.Substring(6, 2);
                var pTime = time.Substring(0, 2) + ':' + time.Substring(2, 2) + ':' + time.Substring(4, 2);

                var dtBar = Convert.ToDateTime(pDate + ' ' + pTime);

                if (dtBar >= dtMin) // for IntraDay Data to reject Data that we already have
                {
                    //var pOpen = Convert.ToDouble(open);
                    //var pHigh = Convert.ToDouble(high);
                    //var pLow = Convert.ToDouble(low);
                    //var pClose = Convert.ToDouble(close);
                    //var pVolume = Convert.ToDecimal(volume);

                    double pOpen = 0, pHigh = 0, pLow = 0, pClose = 0;
                    decimal pVolume=0;
                    if (
                        !double.TryParse(open, System.Globalization.NumberStyles.Float, EnglishCulture, out pOpen) ||
                        !double.TryParse(high, System.Globalization.NumberStyles.Float, EnglishCulture, out pHigh) ||
                        !double.TryParse(low, System.Globalization.NumberStyles.Float, EnglishCulture, out pLow) ||
                        !double.TryParse(close, System.Globalization.NumberStyles.Float, EnglishCulture, out pClose) ||
                        !decimal.TryParse(volume, System.Globalization.NumberStyles.Any, EnglishCulture, out pVolume)
                        )
                    {
                         if (_evl != null) _evl.AddItem(EvlResult.FATAL, "Parse Error", strLine);
                         throw new FormatException("Parse Bar string line Failure: " + strLine);
                    }

                    //var pHigh = Convert.ToDouble(high);
                    //var pLow = Convert.ToDouble(low);
                    //var pClose = Convert.ToDouble(close);
                    //var pVolume = Convert.ToDecimal(volume);

                    var b = new Bar()
                                {
                                    Ticker = ticker,
                                    TimeInt = timeInt,
                                    DT = dtBar,
                                    Open = pOpen,
                                    High = pHigh,
                                    Low = pLow,
                                    Close = pClose,
                                    Volume = pVolume
                                };

                    // _cbBarItemSave(ticker, timeInt, dtBar, pOpen, pHigh, pLow, pClose, pVolume);

                    AddBar(b);
                }
            }
            catch ( Exception )
            {
                if (_evl != null) _evl.AddItem(EvlResult.FATAL, "Parse Error", strLine);
                throw new FormatException("Parse Bar string line Failure: " + strLine);
            }
        }
        private void AddBar( Bar newB )
        {
            Bar b;
            var key = newB.DT.ToString();
            if (!_bars.TryGetValue(key, out b))
            {
                _bars.Add(key, newB);
              //  Console.WriteLine(newB.ToString());
            }
            else
            {
                //Console.WriteLine("Duplicate: " + newB.ToString());

                if (b.High < newB.High) b.High = newB.High;
                if (b.Low > newB.Low) b.Low = newB.Low;

                b.Close = newB.Close;

                if (b.Volume < newB.Volume) b.Volume = newB.Volume;

                //Console.WriteLine("Result: " + newB.ToString());
            }
        }
    }
}
