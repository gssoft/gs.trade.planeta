using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Configurations;
using GS.Corteg;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.Interfaces;
// 
using GS.Trade.Trades;
using GS.Xml;
using WebClients;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.Strategies
{
    public partial class Strategies : Element1<string>, IStrategies
    {
        //private TradeTerminals.TradeTerminals TradeTerminalForLoading;
        [XmlElement]
        public string StrategiesInitXmlFile { get; set; }
        [XmlElement]
        public string StrategiesXmlElementPath { get; set; }

        private readonly object _locker;

        private IEventLog _evl;
        [XmlIgnore]
        public ITradeContext TradeContext { get; set; }

        //public event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //public virtual void OnExceptionEvent(IEventArgs e)
        //{
        //    EventHandler<IEventArgs> handler = ExceptionEvent;
        //    if (handler != null) handler(this, e);
        //}

        public event EventHandler<Events.EventArgs> StrategyTradeEntityChangedEvent;
        public void OnStrategyTradeEntityChangedEvent(EventArgs e)
        {
            EventHandler<EventArgs> handler = StrategyTradeEntityChangedEvent;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<Events.EventArgs> StrategiesEvent;
        public void OnStrategiesEvent(EventArgs e)
        {
            EventHandler<EventArgs> handler = StrategiesEvent;
            if (handler != null) handler(this, e);
        }

        //private IEnumerable<IStrategy> Items {
        //    get {
        //        lock (_locker)
        //        {
        //            return StrategyCollection.ToList();
        //        }
        //    }
        //}

        public IEnumerable<IPosition2> GetPositionCurrents()
        {
            var ss = StrategyCollection; // .ToList();
            return StrategyCollection.Select(s => s.Position).ToList();
        }
        public IEnumerable<IPosition2> GetPositionTotals()
        {
            var ss = StrategyCollection; //.ToList();
            return StrategyCollection.Select(s => s.PositionTotal).ToList();
        }
        public IEnumerable<IPosition2> GetDeals()
        {
            var lst = new List<IPosition2>();
            var ss = StrategyCollection; //.ToList();
            foreach (var s in ss)
            {
                lst.AddRange(s.GetDeals());
            }
            return lst;
        }

        //public string Name { get; private set; }
        //public string Code { get; private set; }

        public override string Key {
            get { return Code; }
        }

        private readonly Dictionary<string, IStrategy> _strategyDictionary = new Dictionary<string, IStrategy>();
        private XmlWebClient _xmlWebClient;

        public IEnumerable<IStrategy> StrategyCollection
        {
            get
            {
                lock(_locker)
                {
                    return _strategyDictionary.Values.ToList();
                }
            }
        }
        public int Count => _strategyDictionary.Count;

        public IStrategy this[string index] => Count > 0 ? _strategyDictionary[index] : null;

        public IStrategy this[int index] => Count > 0 && index < Count ? _strategyDictionary.Values.ToList()[index] : null;

        public Strategies()
        {
            IsEvlEnabled = true;
            _locker = new object();
        }
        public Strategies(string name, string code, ITradeContext tx)
        {
            IsEvlEnabled = true;
            _locker = new object();
            Name = name;
            Code = code;
            if (tx == null) throw new NullReferenceException("ITradeContext is Null");
            TradeContext = tx;
            Parent = tx as IElement1<string>;

        }
        public override void Init()
        {
            IsEvlEnabled = true;
            try
            {
             base.Init(TradeContext.EventLog);
             
            _evl = TradeContext.EventLog;
            //_evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies",
            //    "Strategies","Init Start", Name + " " + Code, "Count: " + _strategyDictionary.Count());

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies",
                "Strategies", "Init Start", Name + " " + Code, "Count: " + _strategyDictionary.Count());

             //TradeContext.Orders.StopOrderFilledEvent += StopOrderFilledEventHandler;
            //TradeContext.Orders.LimitOrderFilledEvent += LimitOrderFilledEventHandler;

            //DeSerializationCollection();
            // DeSerializationCollection3();

            // 230928 Delete *********************************************************************** 
            // Main method from the Web Configuration Version
             DeSerializationCollection32();
                //*******************************************************************************
                //var badInit = new List<string>();
                //foreach (var s in StrategyCollection)
                //{
                //    ((Strategy)s).SetParent(this);
                //    ((Strategy)s).SetTradeContext(TradeContext);
                //    s.Parent = this;
                //    ((Strategy)s).Init();

                //    if (!s.IsWrong) continue;
                //    badInit.Add(s.Key);
                //    _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, s.Name, s.Name, "Initialization Failure", s.ToString(), "");
                //}
                //foreach (var k in badInit) _strategyDictionary.Remove(k);

                //_evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "Strategies",
                //    "Strategies", "Init Finish", Name + " " + Code, "Count: " + _strategyDictionary.Count());

                // This without Cofigurations
                // var strategies =
                //    XDocExtensions.DeSerialize<IStrategy>(StrategiesInitXmlFile, StrategiesXmlElementPath,
                //        "GS.Trade.Strategies", "GS.Trade.Strategies");

                //foreach (var s in strategies)
                //{
                //    s.SetParent(this);
                //    s.SetTradeContext(TradeContext);
                //    s.Parent = this;
                //    s.Strategies = this;
                //    ((Strategy)s).Init();
                //    Register(s);
                //}

            }
            catch (Exception e)
            {
                // throw new NullReferenceException("Strategies.Init Failure:" + e.Message);
                SendException(e);
            }
        }
        public IStrategy Register(Strategy s)
        {
            IStrategy ss;
            lock (_locker)
            {
                if (_strategyDictionary.TryGetValue(s.Key, out ss))
                    return ss;
            }
            AddStrategy(s);
            return s;
        }
        public IStrategy Register(IStrategy s)
        {
            IStrategy ss;
            lock (_locker)
            {
                if (_strategyDictionary.TryGetValue(s.Key, out ss))
                    return ss;
            }
            AddStrategy(s);
            return s;
        }

        public void RegisterStrs(IEnumerable<IStrategy> strs)
        {
            foreach (var s in strs)
            {
                if (s != null)
                {
                    s.SetParent(this);
                    s.SetTradeContext(TradeContext);
                    s.Parent = this;
                    s.Init(TradeContext.EventLog);
                    Register(s);
                }
            }
        }
        public void SetEventLog(IEventLog evl)
        {
            if (evl == null) throw new NullReferenceException("EventLog == Null");
            _evl = evl;
        }

        private void AddStrategy(IStrategy s)
        {
            lock (_locker)
            {
                _strategyDictionary.Add(s.Key, s);
            }
            // if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Register New",
            //                 s.ToString(), s.Key);
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", s.StrategyTickerString, "Register New", s.ToString(), s.Key);
        }
        public void StopOrderFilledEventHandler(string orderTradeKey)
        {
            var stra = (from Strategy s in StrategyCollection where s.TradeKey == orderTradeKey select s).FirstOrDefault();
            if (stra == null) return;
            
            stra.SetStopOrderFilledStatus(true);

            _evl?.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Orders", "Orders", "StopOrderFilledEvent", orderTradeKey, "");
        }
        public void StopOrderFilledEventHandler(Order o)
        {
            var stra = (from Strategy s in StrategyCollection where s.TradeKey == o.TradeKey select s).FirstOrDefault();
            if (stra == null) return;

            stra.SetStopOrderFilledStatus(true);
            stra.StopOrderFilledEventHandler(o);

            if (_evl == null) return;
            _evl.AddItem(EvlResult.WARNING, EvlSubject.TRADING, "Orders", "Orders", "StopOrderFilledEvent", o.ToString(), "");
        }
        public void LimitOrderFilledEventHandler(Order o)
        {
            var stra = (from Strategy s in StrategyCollection where s.TradeKey == o.TradeKey select s).FirstOrDefault();
            if (stra == null) return;

            stra.LimitOrderFilledEventHandler(o);

            if (_evl == null) return;
            TradeContext.Evlm(EvlResult.WARNING, EvlSubject.TRADING, o.StratTicker ,"Order", "LimitOrderFilledEvent",o.ShortOrderInfo, o.ToString());
        }

        //public IStrategy RegisterDefaultStrategy(
        //    string name, string code, 
        //        string accountKey,
        //        string tickerBoard, string tickerKey,  uint timeInt,
        //            string terminalType, string terminalKey)
        public IStrategy RegisterDefaultStrategy( string name, string code, 
                IAccount a , ITicker t,
                uint timeInt, string terminalType, string terminalKey)
        {
            var methodname = MethodBase.GetCurrentMethod()?.Name + "()";

            var s = new DefaultStrategy
            {
                Parent = this,
                Name = name,
                Code = code,
                TradeAccountKey = a.Key,
                TickerBoard = t.ClassCode,
                TickerKey = t.Code,
                TradeTerminalType = terminalType,
                TradeTerminalKey = terminalKey,
                TimeInt = (int)timeInt,
                TimePlanKey = "Forts.Standard",

                Ticker = t,
                TradeAccount = a 

            } as Strategy;
            try
            {
                s.SetParent(this);
                s.SetTradeContext(TradeContext);
                s.Init();
                // Check if Already Exists
                var sf = GetByKey(s.Key);
                if (sf != null)
                {
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, sf.StrategyTimeIntTickerString,
                        sf.StrategyTimeIntTickerString, methodname, "Strategy Already Registered", sf.ToString());
                    return sf;
                }
                //s.SetParent(this);
                //s.SetTradeContext(TradeContext);
                //s.Init();
                //if (s.IsWrong)
                //    return null;

                s = (Strategy)Register(s);
                sf = GetByKey(s.Key);
                if (sf != null)
                {
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, sf.StrategyTimeIntTickerString,
                        sf.StrategyTimeIntTickerString, methodname, "Register New Strategy", sf.ToString());
                    return sf;
                }
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, s.StrategyTimeIntTickerString,
                    s.StrategyTimeIntTickerString, methodname, "Failure In Register Strategy", s.ToString());
                return null;
                /*
            OnStrategyTradeEntityChangedEvent(
                new Events.EventArgs
                {
                    Category = "UI.Positions",
                    Entity = "Current",
                    Operation = "ADD.TOEND",
                    Object = s.Position
                });
            OnStrategyTradeEntityChangedEvent(
                new Events.EventArgs
                {
                    Category = "UI.Positions",
                    Entity = "Total",
                    Operation = "ADD.TOEND",
                    Object = s.PositionTotal
                });
             */
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DefaultStrategy", "RegisterDefaultStrategy()", 
                                                        s==null?"DefaultStrategy":s.Key,e);
               // throw;
            }
            return null;
        }
        public IStrategy RegisterDefaultStrategy(string name, string code, 
                    string account, string tickerboard, string tickercode,
                    uint timeInt, string terminalType, string terminalKey)
        {
            var methodname = MethodBase.GetCurrentMethod()?.Name + "()";

            var s = new DefaultStrategy
            {
                Parent = this,
                Name = name,
                Code = code,
                TradeAccountKey = account,
                TickerBoard = tickerboard,
                TickerKey = tickercode,
                TradeTerminalType = terminalType,
                TradeTerminalKey = terminalKey,
                TimeInt = (int)timeInt,
                TimePlanKey = "Forts.Standard"
            } as Strategy;

            try
            {
                s.SetParent(this);
                s.SetTradeContext(TradeContext);
                s.Init();
                // Check if Already Exists
                var sf = GetByKey(s.Key);
                if (sf != null)
                {
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, sf.StrategyTimeIntTickerString,
                        sf.StrategyTimeIntTickerString, methodname, "Strategy Already Registered", sf.ToString());
                    return sf;
                }
                //s.SetParent(this);
                //s.SetTradeContext(TradeContext);
                //s.Init();
                //if (s.IsWrong)
                //    return null;

                s = (Strategy)Register(s);
                sf = GetByKey(s.Key);
                if (sf != null)
                {
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, sf.StrategyTimeIntTickerString,
                        sf.StrategyTimeIntTickerString, methodname, "Register New Strategy", sf.ToString());
                    return sf;
                }
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, s.StrategyTimeIntTickerString,
                    s.StrategyTimeIntTickerString, methodname, "Failure In Register Strategy", s.ToString());
                return null;
                /*
            OnStrategyTradeEntityChangedEvent(
                new Events.EventArgs
                {
                    Category = "UI.Positions",
                    Entity = "Current",
                    Operation = "ADD.TOEND",
                    Object = s.Position
                });
            OnStrategyTradeEntityChangedEvent(
                new Events.EventArgs
                {
                    Category = "UI.Positions",
                    Entity = "Total",
                    Operation = "ADD.TOEND",
                    Object = s.PositionTotal
                });
             */
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "DefaultStrategy", "RegisterDefaultStrategy()",
                                                        s == null ? "DefaultStrategy" : s.Key, e);
                // throw;
            }
            return null;
        }
        public void SetWorkingStatus(bool status)
        {
            foreach (var s in StrategyCollection)
                s.SetWorkingStatus(status,"");
        }

        public IStrategy GetStrategy(string code, string accountcode,
            string tickerboard, string tickercode, int timeint)
        {
            return StrategyCollection.FirstOrDefault(s =>
                s.Code == code && s.TradeAccountCode == accountcode &&
                s.TickerTradeBoard == tickerboard && s.TickerCode == tickercode &&
                s.TimeInt == timeint);
        }

        public IStrategy GetDefaultStrategy(IOrder3 order)
        {
            return StrategyCollection.FirstOrDefault(
                        s => s.Code=="Default" && 
                        s.TimeInt == 60 && s.TradeAccountKey == order.AccountCode &&
                        s.TickerKey == order.TickerCode && s.TickerBoard == order.TickerBoard);
        }

        public void SetStrategiesLongShortEnabled(bool longenabled, bool shortenabled)
        {
            foreach(var s in StrategyCollection)
                s.SetLongShortEnabled(longenabled, shortenabled);
        }

        public void UpdateLastPrice()
        {
            foreach (var s in StrategyCollection)
            {
                s.UpdateFromLastTick();
            }
        }

        //public bool SerializeCollection()
        //{
        //    // It does not work/ A can't to serialize the derived class X001

        //    var xmlfname = "StrategiesCollection.xml";
        //    TextWriter tr = null;
        //    try
        //    {
        //        //  var xDoc = XDocument.Load(@"D:\Mts\SpsInit.xml");
        //        //  var xe = xDoc.Descendants("Tickers_XmlFileName").First();
        //        //  xmlfname = xe.Value;

        //        var tl = _strategyDictionary.Values.ToList();

        //        tr = new StreamWriter(xmlfname);
        //        //var sw = new StringWriter();
        //        //var sr = new XmlSerializer(typeof(List<X001>));

        //        foreach (var t in tl)
        //        {
        //            var sr = new XmlSerializer(typeof(X001));
        //            sr.Serialize(tr, t);
        //        }

        //        //sr.Serialize(tr, tl);
        //        tr.Close();

        //        if (_evl != null)
        //            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "Serialization",
        //            String.Format("FileName={0}", xmlfname), "Count=" + _strategyDictionary.Count);

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        if (_evl != null)
        //            _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "Serialization",
        //            String.Format("FileName={0}", xmlfname), e.ToString());

        //        if (tr != null) tr.Close();
        //        return false;
        //    }
        //}
        public bool DeSerializationCollection()
        {

            //if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");
            IStrategy stra = null;
            try
            {

            var xDoc1 = XDocument.Load(@"SpsInit.xml");
            var xe = xDoc1.Descendants("Strategies_XmlFileName").First();
            var xmlfname = xe.Value;

            //var stratFiles = Builder.Build<List<string>>(@"Strategies\StratFiles.xml", "ArrayOfString");
            var stratFiles = Builder.Build<List<string>>(xmlfname, "ArrayOfString");
            foreach(var strafile in stratFiles)
            {
                //var xDoc = XDocument.Load(xmlfname);
                var xDoc = XDocument.Load(strafile);
                var ss = xDoc.Descendants("Strategies").FirstOrDefault();
                var xx = ss.Elements();
                foreach (var x in xx)
                {
                    // var s = StrategyFactory(x);
                    var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                    var t = Type.GetType(typeName, false, true);
                    var s = Serialization.Do.DeSerialize<Strategy>(t, x, ErrMessage);
                    stra = s;
                    if (s != null)
                    {
                        s.SetParent(this);
                        s.SetTradeContext(TradeContext);
                        s.Parent = this;
                        s.Init();
                        Register(s);
                    }
                    else
                    {
                        throw new SerializationException("DeSerialize Strategy Failure: " + typeName);
                    }
                }
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            //if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch(Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "Strategy Deserialization", stra==null?"Strategy":stra.ToString(),e);
                throw;
                //throw new SerializationException("DeSerialize Strategy Failure: " + e.Message);
                //if( _evl == null) throw new SerializationException("Serialization error");
                //_evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }
        public bool DeSerializationCollection2()
        {

            //if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");
            IStrategy stra = null;
            try
            {
                var xDoc1 = XDocument.Load(@"SpsInit.xml");
                var xe = xDoc1.Descendants("Strategies_XmlFileName").First();
                var xmlfname = xe.Value;

                //var stratFiles = Builder.Build<List<string>>(@"Strategies\StratFiles.xml", "ArrayOfString");
                var stratFiles = Builder.Build<List<string>>(xmlfname, "ArrayOfString");
                foreach (var strafile in stratFiles)
                {
                    XDocument xDoc = null;
                    if (File.Exists(strafile))
                    {
                        xDoc = XDocument.Load(strafile);
                        //if(xDoc == null)
                        //    SendExceptionMessage3("Startegies", "File", "Get from Web", strafile, new FileNotFoundException(strafile));
                    }
                    if (xDoc == null)
                        return false;

                    var ss = xDoc.Descendants("Strategies").FirstOrDefault();
                    var xx = ss.Elements();
                    foreach (var x in xx)
                    {
                        // var s = StrategyFactory(x);
                        var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                        var t = Type.GetType(typeName, false, true);
                        var s = Serialization.Do.DeSerialize<Strategy>(t, x, ErrMessage);
                        stra = s;
                        if (s != null)
                        {
                            s.SetParent(this);
                            s.SetTradeContext(TradeContext);
                            s.Parent = this;
                            s.Init();
                            Register(s);
                        }
                        else
                        {
                            throw new SerializationException("DeSerialize Strategy Failure: " + typeName);
                        }
                    }
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
                //if (_evl != null)
                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "Strategy Deserialization", stra == null ? "Strategy" : stra.ToString(), e);
                throw;
                //throw new SerializationException("DeSerialize Strategy Failure: " + e.Message);
                //if( _evl == null) throw new SerializationException("Serialization error");
                //_evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }

        public bool DeSerializationCollection3()
        {

            //if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");
            IStrategy stra = null;
            try
            {

                //var xDoc1 = XDocument.Load(@"SpsInit.xml");
                //var xe = xDoc1.Descendants("Strategies_XmlFileName").First();
                //var xmlfname = xe.Value;
                //var stratFiles = Builder.Build<List<string>>(xmlfname, "ArrayOfString");

                //var xDocStrats = GetFromWebStrats();
                var xDocStrats = TradeContext.ConfigurationResourse.Get("Strategies");

                if (xDocStrats == null)
                {
                    SendExceptionMessage3("Startegies", "File", "Get from Resourses", "StratFile",
                        new FileNotFoundException("StrategyFile"));
                    return false;
                }
                //var l = (from stri in xDocStrats.Element("ArrayOfString").Elements("string")
                //           select new
                //           {
                //               // Stra = stri.Element("string").Value
                //               Stra = stri.Value
                //           }
                //           ).ToList();
                var l = (from stri in xDocStrats.Element("ArrayOfString").Elements("string")
                         select stri.Value).ToList();

                // var ll = xDocStrats.GetFileNames("ArrayOfString", "string", "Strategies", GetFIlesFromWeb, true);

                foreach (var strafile in l)
                {
                    XDocument xDoc;
                    if (File.Exists(strafile))
                    {
                        xDoc = XDocument.Load(strafile);
                        //if(xDoc == null)
                        //    SendExceptionMessage3("Startegies", "File", "Get from Web", strafile, new FileNotFoundException(strafile));
                    }
                    else
                    {
                        //xDoc = GetFromWeb(strafile);
                        xDoc = TradeContext.ConfigurationResourse.Get("Strategies", strafile);
                        if (xDoc == null)
                        {
                            SendExceptionMessage3("Startegies", "File", "Get from Resourses", strafile,
                                new FileNotFoundException(strafile));
                            continue;
                        }
                    }

                    var ss = xDoc.Descendants("Strategies").FirstOrDefault();
                    var xx = ss.Elements();
                    foreach (var x in xx)
                    {
                        // var s = StrategyFactory(x);
                        var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                        var t = Type.GetType(typeName, false, true);
                        var s = Serialization.Do.DeSerialize<Strategy>(t, x, ErrMessage);
                        stra = s;
                        if (s != null)
                        {
                            s.SetParent(this);
                            s.SetTradeContext(TradeContext);
                            s.Parent = this;
                            s.Init();
                            Register(s);
                        }
                        else
                        {
                            throw new SerializationException("DeSerialize Strategy Failure: " + typeName);
                        }
                    }
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
                //if (_evl != null)
                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "Strategy Deserialization", stra == null ? "Strategy" : stra.ToString(), e);
                throw;
                //throw new SerializationException("DeSerialize Strategy Failure: " + e.Message);
                //if( _evl == null) throw new SerializationException("Serialization error");
                //_evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }

        public bool DeSerializationCollection32()
        {

            //if (_evl != null)
            //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Startegies", "Start DeSerialization", "", "");

            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "Start DeSerialization", "", "");
            IStrategy stra = null;
            try
            {

                //var xDoc1 = XDocument.Load(@"SpsInit.xml");
                //var xe = xDoc1.Descendants("Strategies_XmlFileName").First();
                //var xmlfname = xe.Value;
                //var stratFiles = Builder.Build<List<string>>(xmlfname, "ArrayOfString");

                //var xDocStrats = GetFromWebStrats();
                //var xDocStrats = TradeContext.ConfigurationResourse.Get("Strategies");
                var xDocStrats = TradeContext.ConfigurationResourse1.Get("Strategies");
                if (xDocStrats == null)
                {
                    SendExceptionMessage3("Startegies", "File", "Get from Resourses", "StratFile",
                        new FileNotFoundException("StrategyFile"));
                    return false;
                }
                //var l = (from stri in xDocStrats.Element("ArrayOfString").Elements("string")
                //           select new
                //           {
                //               // Stra = stri.Element("string").Value
                //               Stra = stri.Value
                //           }
                //           ).ToList();

                //var l = (from stri in xDocStrats.Element("ArrayOfString").Elements("string")
                //         select stri.Value).ToList();

                var xDocs = new List<XDocument>();
                var ll = xDocStrats.GetFiles("ArrayOfString","string","Strategies", "Strategies", GetFilesFromWeb, xDocs);
                var postfix = ", GS.Trade.Strategies, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                foreach (var xDoc in xDocs)
                {
                    var ss = xDoc.Descendants("Strategies").FirstOrDefault();
                    var xx = ss.Elements();
                    foreach (var x in xx)
                    {
                        
                        // var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                        // var t = Type.GetType(typeName, false, true);
                        var typeName = GetType().Namespace + '.' + x.Name + postfix;
                        var t = Type.GetType(typeName,
                        name =>
                            AppDomain.CurrentDomain.GetAssemblies()
                                        .FirstOrDefault(z => z.FullName == name.FullName),
                        null,
                        true);
                        // var s = Serialization.Do.DeSerialize<Strategy>(t, x, "GS.Trade.Strategies", ErrMessage);
                        var s = Serialization.Do.DeSerialize<Strategy>(t, x, ErrMessage);
                        stra = s;
                        if (s != null)
                        {
                            s.SetParent(this);
                            s.SetTradeContext(TradeContext);
                            s.Parent = this;
                            s.Init();
                            Register(s);
                        }
                        else
                        {
                            throw new SerializationException("DeSerialize Strategy Failure: " + typeName);
                        }
                    }
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
                //if (_evl != null)
                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "Strategy Deserialization", stra == null ? "Strategy" : stra.ToString(), e);
                throw;
                //throw new SerializationException("DeSerialize Strategy Failure: " + e.Message);
                //if( _evl == null) throw new SerializationException("Serialization error");
                //_evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }
        private XDocument GetFilesFromWeb(string root, string path)
        {
            return TradeContext.ConfigurationResourse1.Get(root, path);
        }

        public void ErrMessage(string s1, string s2)
        {
            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Strategies",
                    "Strategies", "DeSerialization", s1, s2);
        }
        public void Close()
        {
            //foreach (var s in
            //        from Strategy s in StrategyCollection where s.Position.IsOpened select s)
            foreach(var s in StrategyCollection)
            {
                if( s.Position.IsOpened)
                    s.Finish();
            }
        }
        public void CloseAll()
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "CloseAll()", "Strategies Count = " + Count, "");
            foreach (var s in StrategyCollection)
            {
                    s.SetWorkingStatus(false, "Close All");
                    s.CloseAll(1);
            }
        }
        public void CloseAllSoft()
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "CloseAllSoft()", "Strategies Count = " + Count, "");
            foreach (var s in StrategyCollection)
            {
                s.SetWorkingStatus(false, "Close All Soft");
                s.CloseAllSoft();
            }
        }

        public void CloseAllGently()
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies",
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Strategies Count = " + Count, "");
            foreach (var s in StrategyCollection)
            {
                s.SetWorkingStatus(false, "Close All Gently");
                s.CloseAll(2);
            }
        }
        public void EnableEntry()
        {
            foreach (var s in StrategyCollection)
            {
                s.EntryEnabled = true;
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "EnableEntry()", "Strategies Count = " + Count, "");
        }
        public void DisableEntry()
        {
            foreach (var s in StrategyCollection)
            {
                s.EntryEnabled = false;
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategies", "Strategies", "DiasableEntry()", "Strategies Count = " + Count, "");
        }

        private IStrategy FindByKey(string strategyKey)
        {
            lock (_strategyDictionary)
            {
                return _strategyDictionary.Values.ToList().FirstOrDefault( s => s.StrategyKey == strategyKey);
            }
        }
        public IStrategy GetByKey(string key)
        {
            lock (_strategyDictionary)
            {
                IStrategy ss;
                return _strategyDictionary.TryGetValue(key, out ss) ? ss : null;
            }
        }

        public int RegisterNewTrade(ITrade t)
        {
            var s = FindByKey(t.StrategyKey);
            if (s == null)
                return -1;

            s.RegisterNewTrade(t);

            return +1;
        }       
    }
}
