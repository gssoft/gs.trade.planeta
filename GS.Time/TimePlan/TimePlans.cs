using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;

namespace GS.Time.TimePlan
{
    //public enum TimePlanEventType
    //{
    //    TimePlanItemEvent = 1,
    //    TimePlanItem = 2,
    //    TimePlan = 3
    //} ;
    public class TimePlans : ITimePlans
    {
        private IEventLog _evl; 
        public List<TimePlan> TimePlanList = new List<TimePlan>();

        public TimePlans()
        {
            #region Fill XmlDataBase
            /*
            TimePlan tp;
            TimePlanItem tpi;
            TimePlanItemEvent tpie;
            
            tp = new TimePlan { Name = "Forts Standard", Code = "Forts.Standard", Key = "Forts.Standard".Trim().ToUpper() };
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

            TimePlanList.Add(tp);

            tp = new TimePlan { Name = "Micex Standard", Code = "Micex.Standard", Key = "Micex.Standard".Trim().ToUpper() };

            tpi = new TimePlanItem { Code = "Day", TimeStart = new TimeSpan(0, 0, 1), TimeEnd = new TimeSpan(18, 45, 00) };
            tp.AddTimePeriod(tpi);

            TimePlanList.Add(tp);

            tp = new TimePlan { Name = "Forts.AllDay", Code = "Forts.AllDay", Key = "Forts.AllDay".Trim().ToUpper() };

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

            TimePlanList.Add(tp);
          */
            //  Serialize();
            #endregion
        }
        public void Init( IEventLog evl)
        {
            if( evl == null )
                throw new NullReferenceException("TimePlans: EventLog is Null");
            _evl = evl;
            Deserialize();
            PrintTimePlanList();
        }

        public TimePlan GetTimePlan(string timePlanKey)
        {
            return (from tp in TimePlanList where tp.Key == timePlanKey.Trim().ToUpper() select tp).FirstOrDefault();
        }
        public TimePlan RegisterTimePlanEventHandler(string timePlanKey, EventHandler<ITimePlanEventArgs> act)
        {
            var tplan = (from tp in TimePlanList 
                         where tp.Key == timePlanKey.Trim().ToUpper() 
                         select tp).FirstOrDefault();
            if(tplan == null)
                throw new NullReferenceException("TimePlan:" + timePlanKey + "Is Not Found;");

            if (act != null) tplan.TimePlanEvent += act;
            return tplan;
        }
        public void NewTickEventHandler(DateTime dt)
        {
            foreach (var tp in TimePlanList)
                tp.NewTickEventHandler(dt);
        }
        public void Start()
        {
            foreach (var tp in TimePlanList)
            {
                tp.SetEnable(true);
                foreach (var tpi in tp.TimePlanItemList)
                {
                    tpi.SetEnable(true);
                    foreach (var tpie in tpi.TimePlanItemEventList)
                        tpie.SetEnable(true);
                }
            }
            /*
            foreach (var tpi in TimePlanList.SelectMany(tp => tp.TimePlanItemList))
            {
                tpi.SetEnable(true);
                foreach (var tpie in tpi.TimePlanItemEventList)
                    tpie.SetEnable(true);
            }
             */
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans" , "TimePlans", "Start", "", "");
            
        }
        public void Stop()
        {
            foreach (var tpi in TimePlanList.SelectMany(tp => tp.TimePlanItemList))
            {
                tpi.SetEnable(false);
                foreach (var tpie in tpi.TimePlanItemEventList)
                    tpie.SetEnable(false);
            }
            Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans", "TimePlans", "Stop", "", "");
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
                var sr = new XmlSerializer(typeof(List<TimePlan>));
                sr.Serialize(tr, TimePlanList);
                tr.Close();

                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans", "TimePlans", "Serialization",
                    $"FileName={xmlfname}", "Count=" + TimePlanList.Count);

                return true;
            }
            catch (Exception e)
            {
                Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TimePlans", "TimePlans", "Serialization",
                    $"FileName={xmlfname}", e.ToString());

                tr?.Close();
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

                TimePlanList = Serialization.Do.DeSerialize<List<TimePlan>>(x, 
                    (s1, s2) => Evlm(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                    "TimePlans", "TimePlans", s1, String.Format("FileName={0}", xmlfname), s2)
                                    );
                foreach (var tp in TimePlanList)
                {
                    tp.TimePlans = this;
                    foreach (var tpi in tp.TimePlanItemList)
                    {
                        tpi.TimePlan = tp;
                        foreach (var tpie in tpi.TimePlanItemEventList)
                            tpie.TimePlanItem = tpi;
                    }
                }
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans", "TimePlans", "DeSerialization",
                    $"FileName={xmlfname}", "Count=" + TimePlanList.Count);
                
            }
            catch (Exception e)
            {
                _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TimePlans", "TimePlans", "DeSerialization",
                    $"FileName={xmlfname}", e.ToString());

                throw new SerializationException("TimePlans.Deserialization Failure " + xmlfname);
            }
            return true;
        }
        private void PrintTimePlanList()
        {
            foreach (var tp in TimePlanList)
            {
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans", tp.Code, "Desrialization", "", tp.ToString());
                foreach (var tpi in tp.TimePlanItemList)
                {
                    Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlanItem", tpi.Code, "Desrialization", "", tpi.ToString());
                    foreach (var tpie in tpi.TimePlanItemEventList)
                        //var s = tpie.Description;
                        Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlanItemEvent", tpie.Code, "Desrialization", "", tpie.ToString());
                }
            }
        }

        public void Evlm(EvlResult result, EvlSubject subject,
                                   string source, string entity, string operation, string description, string obj)
        {
            _evl?.AddItem(result, subject, source, entity, operation, description, obj);
        }
    }
    public class TimePlan : ITimePlan
    {
        public event EventHandler<ITimePlanEventArgs> TimePlanEvent;

        public TimePlans TimePlans { get; set; }
        [XmlIgnore]
        public bool Enabled { get; private set; }
       // public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get { return Code.Trim().ToUpper(); }}

        public readonly List<TimePlanItem> TimePlanItemList = new List<TimePlanItem>();

        private readonly object _timePlanItems = new object();
        public void AddTimePeriod(TimePlanItem tp)
        {
            lock (_timePlanItems)
            {
                TimePlanItemList.Add(tp);
            }
        }     
        public void NewTickEventHandler(DateTime dt)
        {
           // if (TimePlanEvent == null)
           //     return;
            try
            {
                foreach (var tpi in TimePlanItemList)
                {   
                    if( ! tpi.UpdateEnable(dt) ) continue;
                    //Enabled = true;
                    if( TimePlanEvent == null) continue;

                    //foreach (var tpie in tpi.TimePlanItemEventList.Where(tpie => tpie.Enabled))
                    foreach (var tpie in tpi.TimePlanItemEventList) //.Where(tpie => tpie.Enabled))
                    {
                        if (!tpie.Enabled)
                            continue;
                        if (!tpie.IsEventReady(dt, tpi.TimeStart, tpi.TimeEnd))
                            continue;
                        //TimePlanEvent(tpie, new TimePlanEventArgs(TimePlanEventType.TimePlanItemEvent, tpie.Category, tpie.Code, tpi.TimePlan.Code, tpi.Code, tpie.Code, tpie.Description));
                        FireTimePlanEvent(tpie, TimePlanEventType.TimePlanItemEvent, Code, tpi.Code, tpie.Code, tpie.Msg);

                        TimePlans.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                                            tpi.Code, tpie.Key, "FireEvent", tpie.Description, tpie.ToString());
                        tpie.SetEnable(false);
                    }
                }
                var en = TimePlanItemList.Any(tpi => tpi.Enabled);
                if (Enabled == en) return;
                SetEnable(en);
            }
            catch (Exception e)
            {
                throw new NullReferenceException("TimePlan.NewTickEventHandler() Exception:" + e.Message);
            }

           // Enabled = en;
           // TimePlans.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Key, Enabled ? "ENABLED" : "DISABLED", ToString(), "");
        }
        public void FireTimePlanEvent(object sender, TimePlanEventType tptype, string tpcode, string tpicode, string tpiecode, string msg)
        {
            TimePlanEvent?.Invoke(sender, new TimePlanEventArgs(tptype, tpcode, tpicode, tpiecode, msg));
        }

        public void SetEnable(bool value)
        {
            Enabled = value;
            TimePlans.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "TimePlans", Key, Enabled ? "ENABLED" : "DISABLED", "", ToString());
        }
        public override string ToString()
        {
            return String.Format("Type={0};Key={1};Code={2}", GetType(), Key, Code);
        } 
    }
    public class TimePlanItem
    {
      //  public string Name { get; set; }
        public string Code { get; set; }
        public string Key => $"{TimePlan.Key}; {Code.Trim().ToUpper()}";

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
        [XmlIgnore]
        public bool Enabled { get; private set; }

        public TimePlan TimePlan { get; set; }

        public readonly List<TimePlanItemEvent> TimePlanItemEventList = new List<TimePlanItemEvent>();
       /*
       public IEnumerable<TimePlanItemEvent> TpieList
       {
           get { return _tpieList; }
       }
       */
        //public event EventHandler<TimePlanEventArgs> TimePlanItemEvent;

        private readonly object _timePlanItems = new object();
        public void AddTimeItemEvent(TimePlanItemEvent tpie)
        {
            lock (_timePlanItems)
            {
                TimePlanItemEventList.Add(tpie);
            }
        }
        /*
        public void NewTick(DateTime dt)
        {
            if (TimePlan == null || TimePlanItemEvent == null)
                return;
            foreach (var tpie in TimePlanItemEventList.Where(tpie => tpie.IsEventReady(dt, TimeStart, TimeEnd)))
            {
                TimePlanItemEvent(this, new TimePlanEventArgs(tpie.Description));
            }
        }
        */
        public bool UpdateEnable(DateTime dt)
        {
            try
            {
                if (Enabled == false && 
                    dt.TimeOfDay.CompareTo(TimeStart) > 0 && dt.TimeOfDay.CompareTo(TimeEnd) < 0)
                {
                    SetEnable(true);
                    TimePlan.FireTimePlanEvent(this, TimePlanEventType.TimePlanItem, TimePlan.Code, Code, string.Empty, "START");
                    foreach (var tpie in TimePlanItemEventList)
                        tpie.SetEnable(true);
                }
                else if ( Enabled &&
                            (dt.TimeOfDay.CompareTo(TimeStart) <= 0 || dt.TimeOfDay.CompareTo(TimeEnd) >= 0)
                        )
                {
                    SetEnable(false);
                    TimePlan.FireTimePlanEvent(this, TimePlanEventType.TimePlanItem, TimePlan.Code, Code, string.Empty, "FINISH");
                    foreach (var tpie in TimePlanItemEventList)
                        tpie.SetEnable(false);
                }
                return Enabled;
            }
            catch (Exception e)
            {
                throw new NullReferenceException("TimePlanItem.UpdateEnable() Exception:" + e.Message );
            }
        }
        public void SetEnable(bool  value)
        {
            Enabled = value;

            TimePlan.TimePlans.Evlm(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, "TimePlans", Key, Enabled?"ENABLED":"DISABLED","",ToString());
        }

        public override string ToString()
        {
            try
            {
                return String.Format("Type={0};Key={1};Code={2};Start={3};End={4}", GetType(), Key, Code, TimeStart, TimeEnd);
            }
            catch (Exception e)
            {
                throw new NullReferenceException("TimePlanItem.ToString() Exception:" + e.Message );
            }
            
        } 
    }
    public class TimePlanItemEvent
    {
        public TimePlanItem TimePlanItem { get; set; }

        private bool _enabled;
        public bool Enabled { get { return _enabled; }
        }

      //  public string Name { get; set; }
        public string Code { get; set; }
        public string Key => $"{TimePlanItem.Key}; {Code.Trim().ToUpper()}; {Msg.Trim().ToUpper()}";
        public string Msg{ get; set; }

        [XmlIgnore]
        public TimeSpan Duration;

        [XmlElement("Duration")]
        public string XmlDuration
        {
        get { return Duration.ToString(); }
        set { Duration = TimeSpan.Parse(value); }
        }
        public string Description {
            get
            {
                return Msg + " " + Code + " " + (
                        (Code.Trim().ToUpper()) == "AFTER"
                            ? TimePlanItem.TimeStart.ToString()
                            : TimePlanItem.TimeEnd.ToString());
            }
        }
       
        public bool IsEventReady(DateTime dt,  TimeSpan ts1, TimeSpan ts2)
        {
            
            switch(Code.Trim().ToUpper())
            {
                case "AFTER":
                    //var dtt = dt.Date.Add(ts1.Add(Duration));
                    if ( dt.CompareTo(dt.Date.Add(ts1.Add(Duration))) > 0)
                        return true;
                    break;
                case "TOEND":
                    //dtt = dt.Date.Add(ts2.Add(-Duration));
                    if (dt.CompareTo(dt.Date.Add(ts2.Add(-Duration))) >= 0)
                        return true;
                    break;
            }
            return false;
        }
        public void SetEnable(bool value)
        {
            _enabled = value;
           // TimePlanItem.TimePlan.TimePlans.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Key, _enabled ? "ENABLED" : "DISABLED", ToString(), "");
        }
        public override string ToString()
        {
            try
            {
                return String.Format("Type={0};Key={1};Code={2};Msg={3};Duration={4}", GetType(), Key, Code, Msg, Duration);
            }
            catch (Exception e)
            {
                throw new NullReferenceException("TimePlanItemEvent.ToString() Exception:" + e.Message );
            }
        }
    }
    public class TimePlanEventArgs : EventArgs, ITimePlanEventArgs
    {
        public TimePlanEventType EventType { get; set; }
        public string TimePlanCode { get; set; }
        public string TimePlanItemCode { get; set; }
        public string TimePlanItemEventCode { get; set; }
        //  public readonly string Code;
        public string Msg { get; set; }

        //  public readonly string Description;

        public TimePlanEventArgs(TimePlanEventType et, string timePlanCode, string timePlanItemCode, string timePlanItemEventCode, string msg)
        {
            TimePlanItemEventCode = timePlanItemEventCode.Trim().ToUpper();
            TimePlanItemCode = timePlanItemCode.Trim().ToUpper();
            TimePlanCode = timePlanCode.Trim().ToUpper();

            EventType = et;
            Msg = msg.Trim().ToUpper();

        }
        public override string ToString()
        {
            try
            {
                return string.Format("TPlan={0};TPItem={1};TPIEvent={2};Msg={3}",
                                 TimePlanCode, TimePlanItemCode, TimePlanItemEventCode, Msg );
            }
            catch (Exception e)
            {
                throw new NullReferenceException("TimePlanEventArgs.ToString() Exception:" + e.Message );
            }
        }
    }
}
