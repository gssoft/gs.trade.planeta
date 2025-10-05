using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Interfaces;


namespace GS.Process
{
    public partial class ProcessManager2
    {
        public class Process2 : IProcess2
        {
            public string Name { get; internal set; }
            public string Key { get; internal set; }

            public int TimeInterval { get; internal set; }
            public int TimeIntervalOffset { get; internal set; }
            public DateTime DatetimeToExecute { get; internal set; }

            public int Status { get; internal set; }
            public bool IsEnabled { get; set;}

            public event NewTickEventHandler NewTickEvent;

            internal ProcessProcedure InitProcedure;
            internal ProcessProcedure MainProcedure;
            internal ProcessProcedure FinishProcedure;

            // public string Key { get { return GetKey(Name, TimeInterval, TimeIntervalOffset); } }

            public Process2()
            {
                IsEnabled = true;
            }

            public Process2(string name, int timeinterval, int timeintervaloffset)
            {
                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
                DatetimeToExecute = DateTime.Now;

                IsEnabled = true;
            }
            public Process2(string name, string key, int timeinterval, int timeintervaloffset)
            {
                Name = name;
                Key = key;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
                DatetimeToExecute = DateTime.Now;

                IsEnabled = true;
            }
            public static string GetKey(string name, int timeInterval, int timeoffset)
            {
                return name.Trim().ToUpper() + ";" + timeInterval + ";" + timeoffset;
            }

            public void Init()
            {
                EventLog?.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process", Name, "Init Process", "", ToString());

                InitProcedure?.Invoke();
            }

            public void Main()
            {
                var dt = DateTime.Now;
                if (dt < DatetimeToExecute) return;
                var d = ((dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond + TimeInterval) / TimeInterval) * TimeInterval -
                        (dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond) + TimeIntervalOffset;

                // EventLog.AddItem(EvlResult.SUCCESS, Name + ": " + dt.ToString("T") + " " + dt.Millisecond, d.ToString());

                DatetimeToExecute = dt.AddMilliseconds(d);

                MainProcedure?.Invoke();
                NewTickEvent?.Invoke(dt);
            }
            public void Finish()
            {
                EventLog?.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name,"Finish Process", ToString(), "");

                FinishProcedure?.Invoke();
            }

            //protected abstract void InitProcess();
            //protected abstract void ExecuteProcess();
            //protected abstract void FinishProcess();
            public override string ToString()
            {
                return $"Type={GetType()};Name={Name};Key={Key}TimiInt={TimeInterval};TimeOffset={TimeIntervalOffset}";
            }
        }
    }
}
