using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;

namespace GS.Trade.Trades.Time
{
    public class TimePlans3
    {
        public readonly List<TimePlan3> TimePlanCollection = new List<TimePlan3>();

        public TimePlans3()
        {
            /*
            TimePlan2 tp;
            TimePlanItem tpi;
            TimePlanItemEvent tpie;
            
            tp = new TimePlan2 { Name = "Forts Standard", Code = "Forts.Standard", Key = "Forts.Standard".Trim().ToUpper() };
            tpi = new TimePlanItem { Code = "Day", TimeStart = new TimeSpan(10, 0, 0), TimeEnd = new TimeSpan(18, 45, 00) };
            tpie = new TimePlanItemEvent
                           {
                               Category = "ToEnd",
                               Name = "1_Min_To_End",
                               Code = "1_Min_To_End",
                               Key = "1_Min_To_End",
                               Duration = new TimeSpan(0,1,0)
                           };
            tpi.AddTimeItemEvent(tpie);

            tp.AddTimePeriod(tpi);

            tpi = new TimePlanItem { Code = "Evening", Key = "Evening", TimeStart = new TimeSpan(19, 00, 00), TimeEnd = new TimeSpan(23, 50, 00) };
            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "5_Min_To_End",
                Code = "5_Min_To_End",
                Key = "5_Min_To_End",
                Duration = new TimeSpan(0, 5, 0)
            };
            tpi.AddTimeItemEvent(tpie);
            tp.AddTimePeriod(tpi);

            TimePlanCollection.Add(tp);

            tp = new TimePlan2 { Name = "Micex Standard", Code = "Micex.Standard", Key = "Micex.Standard".Trim().ToUpper() };

            tpi = new TimePlanItem { Code = "Day", TimeStart = new TimeSpan(0, 0, 1), TimeEnd = new TimeSpan(18, 45, 00) };
            tp.AddTimePeriod(tpi);

            TimePlanCollection.Add(tp);

            tp = new TimePlan2 { Name = "Forts.AllDay", Code = "Forts.AllDay", Key = "Forts.AllDay".Trim().ToUpper() };

            tpi = new TimePlanItem { Code = "Day+Evening", TimeStart = new TimeSpan(0, 0, 1), TimeEnd = new TimeSpan(23, 50, 00) };

            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "15_Sec_To_End",
                Code = "15_Sec_To_End",
                Key = "15_Sec_To_End",
                Duration = new TimeSpan(0, 0, 15)
            };
            tpi.AddTimeItemEvent(tpie);
            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "30_Sec_To_End",
                Code = "30_Sec_To_End",
                Key = "30_Sec_To_End",
                Duration = new TimeSpan(0, 0, 30)
            };
            tpi.AddTimeItemEvent(tpie);

            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "1_Min_To_End",
                Code = "1_Min_To_End",
                Key = "1_Min_To_End",
                Duration = new TimeSpan(0, 1, 0)
            };
            tpi.AddTimeItemEvent(tpie);

            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "3_Min_To_End",
                Code = "3_Min_To_End",
                Key = "3_Min_To_End",
                Duration = new TimeSpan(0, 3, 0)
            };
            tpi.AddTimeItemEvent(tpie);

            tpie = new TimePlanItemEvent
            {
                Category = "ToEnd",
                Name = "5_Min_To_End",
                Code = "5_Min_To_End",
                Key = "5_Min_To_End",
                Duration = new TimeSpan(0, 5, 0)
            };
            tpi.AddTimeItemEvent(tpie);

            tp.AddTimePeriod(tpi);

            TimePlanCollection.Add(tp);
          */
          //  Serialize();

        }
        public void Init()
        {
            Deserialize();
        }

        public TimePlan3 GetTimePlan(string timePlanKey)
        {
            return (from tp in TimePlanCollection where tp.Key == timePlanKey.Trim().ToUpper() select tp).FirstOrDefault();
        }
        public TimePlan3 RegisterTimePlanEventHandler(string timePlanKey, EventHandler<TimePlanEventArgs> act)
        {
            var tplan = (from tp in TimePlanCollection where tp.Key == timePlanKey.Trim().ToUpper() select tp).FirstOrDefault();
            if (tplan != null)
                if (act != null) tplan.TimePlanEvent += act;
            return tplan;
        }
        public void NewTickEventHandler(DateTime dt)
        {
            foreach (var tp in TimePlanCollection)
                tp.NewTickEventHandler(dt);
        }
        public bool Serialize()
        {
            string xmlfname = null;
            TextWriter tr = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");
                var xe = xDoc.Descendants("TimePlans_XmlFileName").First();
                xmlfname = xe.Value;

                //var tl = _tickerDictionary.Values.ToList();

                tr = new StreamWriter(xmlfname);
                // var sr = new XmlSerializer(typeof(Dictionary<string,Ticker>));  // !!! Not Support !!!!!
                var sr = new XmlSerializer(typeof(List<TimePlan3>));
                sr.Serialize(tr, TimePlanCollection);
                tr.Close();

               // _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Tickers", "Serialization",
               //  String.Format("FileName={0}", xmlfname), "Count=" + _tickerDictionary.Count);

                return true;
            }
            catch (Exception e)
            {
              //  _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "Serialization",
              //  String.Format("FileName={0}", xmlfname), e.ToString());

                if (tr != null) tr.Close();
                return false;
            }
        }

        public bool Deserialize()
        {
            string xmlfname = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");

                var xe = xDoc.Descendants("TimePlans_XmlFileName").First();
                xmlfname = xe.Value;

                var x = XElement.Load(xmlfname);

                var tp = Serialization.Do.DeSerialize<List<TimePlan3>>(x, null
                   // (s1, s2) => _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                   //                 "Tickers", s1, String.Format("FileName={0}", xmlfname), s2)
                                    );
                /*
                f = new StringReader(xmlfname);
                var newSr = new XmlSerializer(typeof(List<Ticker>));
                var tl = (List<Ticker>)newSr.Deserialize(f);
                f.Close();
                */
                foreach (var tpi in tp)
                {
                    tpi.TimePlans = this;
                    /*
                    if (!CheckTicker(t)) continue;
                    t.Tickers = this;
                    t.SetEventLog(_evl);
                    AddTicker(t);
                     */
                }
            
            }
            catch (Exception e)
            {
              //  _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "DeSerialization",
              //  String.Format("FileName={0}", xmlfname), e.ToString());

                throw new SerializationException("TimePlans.Deserialization Failure " + xmlfname);
            }
            return true;
        }
    }
    public class TimePlan3
    {
        public event EventHandler<TimePlanEventArgs> TimePlanEvent;

        public TimePlans3 TimePlans { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        public readonly List<TimePlanItem> TimePlanItemCollection = new List<TimePlanItem>();

        private readonly object _timePlanItems = new object();
        public void AddTimePeriod(TimePlanItem tp)
        {
            lock (_timePlanItems)
            {
                TimePlanItemCollection.Add(tp);
            }
        }     
        public void NewTickEventHandler(DateTime dt)
        {
            if (TimePlanEvent == null)
                return;
            foreach (var tpi in TimePlanItemCollection)
                foreach(var tpie in tpi.TpieList)
                    if( tpie.IsEventReady(dt, tpi.TimeStart, tpi.TimeEnd) )
                        TimePlanEvent(this, new TimePlanEventArgs(Key, tpi.Key, tpie.Key));
        }
    }
    public class TimePlanItem
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        [XmlIgnore]
        public TimeSpan TimeStart { get; set; }
        [XmlElement("TimeStart")]
        public string XmlTimeStart
        {
            get { return TimeStart.ToString(); }
            set { TimeStart = TimeSpan.Parse(value); }
        }
        [XmlIgnore]
        public TimeSpan TimeEnd { get; set; }
        [XmlElement("TimeEnd")]
        public string XmlTimeEnd
        {
            get { return TimeEnd.ToString(); }
            set { TimeEnd = TimeSpan.Parse(value); }
        }

        public TimePlan3 TimePlan { get; set; }

       public readonly List<TimePlanItemEvent> TpieList = new List<TimePlanItemEvent>();
       /*
       public IEnumerable<TimePlanItemEvent> TpieList
       {
           get { return _tpieList; }
       }
       */
        public event EventHandler<TimePlanEventArgs> TimePlanItemEvent;

        private readonly object _timePlanItems = new object();
        public void AddTimeItemEvent(TimePlanItemEvent tp)
        {
            lock (_timePlanItems)
            {
                TpieList.Add(tp);
            }
        }
        public void NewTick(DateTime dt)
        {
            if (TimePlan == null || TimePlanItemEvent == null)
                return;
            foreach (var tpie in TpieList.Where(tpie => tpie.IsEventReady(dt, TimeStart, TimeEnd)))
            {
                TimePlanItemEvent(this, new TimePlanEventArgs(TimePlan.Key, Key, tpie.Key));
            }
        }
    }
    public class TimePlanItemEvent
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }
        public string Category{ get; set; }
/*
        public string Unit { get; set; }
        public int Value { get; set; }
*/
        [XmlIgnore]
        public TimeSpan Duration;

        [XmlElement("Duration")]
        public string XmlDuration
        {
        get { return Duration.ToString(); }
        set { Duration = TimeSpan.Parse(value); }
        }
       /*
        private TimeSpan GetTimeSpan()
        {
            switch (Unit.Trim().ToUpper())
            {
                case "S":
                    return  new TimeSpan(0,0,Value);
                case "M":
                    return new TimeSpan(0,Value,0);
                case "H":
                    return  new TimeSpan(Value,0,0);
            }
            return TimeSpan.Zero;
        }
        */
        public bool IsEventReady(DateTime dt,  TimeSpan ts1, TimeSpan ts2)
        {
            /*
            switch(Category.Trim().ToUpper())
            {
                case "After":
                    var dtt = dt.Date.Add(ts1.Add(GetTimeSpan()));
                    if (dt.CompareTo(dt.Date.Add(ts1.Add(GetTimeSpan()))) >= 0)
                        return true;
                    break;
                case "ToEnd":
                    dtt = dt.Date.Add(ts2.Add(-GetTimeSpan()));
                    if (dt.CompareTo(dt.Date.Add(ts2.Add(-GetTimeSpan()))) >= 0)
                        return true;
                    break;
            }
             */ 
            return false;
        }
    }
    public class TimePlanEventArgs : EventArgs
    {
        public readonly string TimePlanStr;
        public readonly string TimePlanItemStr;
        public readonly string TimePlanItemEventStr;

        public TimePlanEventArgs(string tpMsg, string tpiMsg, string tpieMsg)
        {
            TimePlanStr = tpMsg;
            TimePlanItemStr = tpiMsg;
            TimePlanItemEventStr = tpieMsg;
        }
    }
}
