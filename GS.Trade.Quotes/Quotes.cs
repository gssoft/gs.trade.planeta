using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Process;


namespace GS.Trade.Quotes
{

   // *************************** Quotes **********************
    public partial class Quotes
    {
        [XmlIgnore]
        private IEventLog _eventLog;

        private string _name = "";
        public string Name
        {
            get { return _name = ""; }
            set { _name = value; }
        }

        private string _ticker = "";
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value; }
        }

        public DateTime DT { get; set;}
        public double Close { get; set; }

        public string QuoteString { get; set; }

        public int Put_Polling_Delay_Seconds=5;
        public DateTime Put_Next_Request_DT;

        public int Get_Polling_Delay_Seconds=5;
        public DateTime Get_Next_Request_DT;

       // [XmlIgnore]
        public  List<Quote> QuoteCollection;
        public Dictionary<string, Quote> QuoteDictionary;

        private object _lockPutQuote;

        [XmlIgnore]
        private readonly Queue<string> qQuoteStr;

        public Quotes()
        { 
            qQuoteStr = new Queue<string>();
            QuoteCollection = new List<Quote>();
            QuoteDictionary = new Dictionary<string, Quote>();
            QuoteObserveCollection = new ObservableCollection<Quote>();

            _lockPutQuote = new object();
            _observeQuoteProcess = new SimpleProcess("Quote Observe Process", 5, 1, ExecuteObserveProcess);
        }
        public void Init(IEventLog evl)
        {
            _eventLog = evl;
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING, "Quotes", "Quotes", "Initialization", "", "");
        }
        
        public void PutDdeQuote( string _ddestr )
        {
            var dt = DateTime.Now;
            if (dt < Put_Next_Request_DT && Put_Polling_Delay_Seconds != 0) return;

            lock (_lockPutQuote)
            {
                Put_Next_Request_DT = dt.AddSeconds(Put_Polling_Delay_Seconds);
                qQuoteStr.Enqueue(_ddestr);
            }
            //  _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "QuoteDdeChannel.OnPoked", String.Format("New Quote {0}", _ddestr)));
        }
        public void PutDdeQuote2( string ddeStr )
        {
            lock (_lockPutQuote)
            {
                qQuoteStr.Enqueue(ddeStr);
            }
   
        }
        public void PutDdeQuote3( string ddeStr )
        {
            // _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Dde", "Put DdeString", ddeStr, "");

            lock (_lockPutQuote)
            {
                var q = ParseQuoteStr(ddeStr);
                UpdateQuoteOrNew3(q);
            }
        }
        public void PutDdeQuoteTest()
        {
            for( var i=0; i<100; i++ )
                PutDdeQuote("RIZ0; 01.01.2010; 10:00:00; 173740; ");
        }

        public void Get_Dde_Quote()
        {
            var dt = DateTime.Now;
            if (dt < Get_Next_Request_DT) return;

            Get_Next_Request_DT = dt.AddSeconds(Get_Polling_Delay_Seconds);

            string s = null; int cnt = 0; int cnt2 = 0;

            lock (_lockPutQuote)
            {
                cnt = qQuoteStr.Count;
                cnt2 = cnt;
                while (cnt-- > 0)
                {

                    s = qQuoteStr.Dequeue();
                }
            }
            if (s == null) return;
            QuoteString = s;
            if ( Parse_Quote_Str(s) )
            {
                // _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "Get_DDE_Quotes from Queue.",  
                //    String.Format("{0} {1} {2}; {3}; Queue={4}", Ticker, DT, Close, s, cnt2 )));
            }
        }
        public bool Parse_Quote_Str(string s)
        {
            string sError = null; bool _r = true;;
            try
            {
                string[] split = s.Split(new Char[] { ';' });
                sError = "Split_Ticker";
                string sTicker = (split[0]).Trim();
                sError = "Split_Date";
                string sD = (split[1]).Trim();
                sError = "Split_Time";
                string sT = (split[2]).Trim();
                sError = "Split_Close";
                string sClose = (split[3]).Trim();

                DateTime _d; TimeSpan _t;  double _close;

                sError = "Parse_Close";
                _close = double.Parse(sClose);
                sError = "Parse_Date";
                _d = DateTime.Parse(sD);
                sError = "Parse_Time";
                //_t = DateTime.Parse(sT);
                _t = TimeSpan.Parse(sT);

                Name = sTicker; 
                Ticker = sTicker; Close = _close; DT = _d.Date + _t;
            }
            catch
            {
                //if (FatalErrorCount > 0)

                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Quotes", "Quotes", "QuoteDdeChannel.OnPoked",
                            String.Format("Quote DdeDataStr Parse Error {0} {1}", sError, s),"");
                _r = false;
            }
            return _r;
        }
        public void GetDdeQuote()
        {
            DateTime dt = DateTime.Now;
            if (dt >= Get_Next_Request_DT)
            {
                Get_Next_Request_DT = dt.AddSeconds(Get_Polling_Delay_Seconds);

                string s = null;
                int cnt = 0;
                Quote q;

                lock (this)
                {
                    cnt = qQuoteStr.Count;
                }
                while (cnt-- > 0)
                {
                    lock(this)
                    {
                        s = qQuoteStr.Dequeue();
                    }
                    if (s == null) continue;
                   
                    //QuoteString = s;
                    q = ParseQuoteStr(s);

                    UpdateQuoteOrNew(q);

                    // _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "Get_DDE_Quotes from Queue.",  
                    //    String.Format("{0} {1} {2}; {3}; Queue={4}", Ticker, DT, Close, s, cnt2 )));
                }
            }
        }
        public void GetDdeQuote2()
        {
            var dt = DateTime.Now;
            if (dt < Get_Next_Request_DT) return;

            Get_Next_Request_DT = dt.AddSeconds(Get_Polling_Delay_Seconds);

            string s;
            Quote q;
            lock (_lockPutQuote)
            {
                var cnt = qQuoteStr.Count;
                while (cnt-- > 0)
                {
                    s = qQuoteStr.Dequeue();

                    if (s == null) continue;

                    q = ParseQuoteStr(s);

                    UpdateQuoteOrNew3(q);

                    /*
                    Quote quote;
                    if (QuoteDictionary.TryGetValue(q.Ticker, out quote))
                    {
                        quote.Close = q.Close;
                        quote.DT = q.DT;
                    }
                    else
                    {
                        QuoteDictionary.Add(q.Ticker, q);
                        _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "New Ticker",
                                          quote.ToString(), "");
                    }
                     */
                }
            }
        }
        public Quote ParseQuoteStr(string s)
        {
            string sError = null; Quote r = null;
            try
            {
                string[] split = s.Split(new Char[] { ';' });
                sError = "Split_Ticker";
                string sTicker = (split[0]).Trim();
                sError = "Split_Date";
                string sD = (split[1]).Trim();
                sError = "Split_Time";
                string sT = (split[2]).Trim();
                sError = "Split_Close";
                string sClose = (split[3]).Trim();

                sError = "Parse_Close";
                var close = double.Parse(sClose);
                sError = "Parse_Date";
                var dt = DateTime.Parse(sD);
                sError = "Parse_Time";
                //_t = DateTime.Parse(sT);
                var t = TimeSpan.Parse(sT);

                //Name = sTicker;
                //Ticker = sTicker; Close = _close; DT = _dt.Date + _t;
                r = new Quote( sTicker, dt + t, close);
            }
            catch
            {
                //if (FatalErrorCount > 0)

                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "QuoteDdeChannel.OnPoked", "QuoteDdeChannel.OnPoked", "Parse",
                                            String.Format("Error in {0} {1}", sError, s),"");
                r = null;
            }
            return r;
            
        }
        public void UpdateQuoteOrNew(Quote quote)
        {
            var q = QuoteCollection.Where(qq => qq.Ticker == quote.Ticker).FirstOrDefault();
            if (q != null)
            {
                q.DT = quote.DT;
                q.Close = quote.Close;
                
                // _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "QuoteUpdate",
                //            String.Format("Quote Update {0} {1} {2}", quote.Ticker, quote.DT, quote.Close)));
            }
            else
            {
                lock (this)
                {
                QuoteCollection.Add(quote);
                }
                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "Quotes", "New",
                            quote.ToString(),"");
            }
        }
        private void UpdateQuoteOrNew2(Quote quote)
        {
 
 //           QuoteDictionary[quote.Ticker] = quote;

            var q = QuoteCollection.Where(qq => qq.Ticker == quote.Ticker).FirstOrDefault();
            if (q != null)
            {
                q.DT = quote.DT;
                q.Close = quote.Close;

                // _EventLog.Add(new EventLogItem(EvlResult.SUCCESS, "QuoteUpdate",
                //            String.Format("Quote Update {0} {1} {2}", quote.Ticker, quote.DT, quote.Close)));
            }
            else
            {
                lock (this)
                {
                    QuoteCollection.Add(quote);
                }
                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "New", "New",
                            quote.ToString(), "");
            }
        }
        private void UpdateQuoteOrNew3(Quote q)
        {
            Quote quote;
            if (QuoteDictionary.TryGetValue(q.Ticker, out quote))
            {
                quote.Close = q.Close;
                quote.DT = q.DT;
            }
            else
            {
                QuoteDictionary.Add(q.Ticker, q);
                _eventLog.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Quotes", "Quotes", "New Ticker",
                                  q.Ticker, q.ToString());
            }
        }
        public Quote GetQuote(string ticker)
        {
            return  QuoteCollection.Where(q => q.Ticker == ticker).FirstOrDefault();         
        }

// ************* Quote *****************************************
        public class Quote
        {
            public string Name { get; set; }
            public string Ticker { get; set; }
            private DateTime _dt;
            public DateTime DT
            {
                get { return _dt; }
                set { _dt = value; }
            }
            public double Close { get; set; }
            public double Last { get { return Close; } }
            public string DateTimeString { get { return _dt.ToString("G"); }}

            public Quote()
            {
            }
            public Quote(string ticker, DateTime dt, double close)
            {
                Name = ticker;
                Ticker = ticker;
                DT = dt;
                Close = close;
            }
            public override string ToString()
            {
                return String.Format("Ticker={0}, DateTime={1}, Last={2}", Ticker, DT, Close);
            }
        }

    }
}

