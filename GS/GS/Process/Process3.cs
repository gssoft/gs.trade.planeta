using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Interfaces;

namespace GS.Process
{
    public interface IProcess3
    {
        void Init();
        void Main();
        void Finish();

        string Key { get; }
        bool IsEnabled { get; }
    }

    public class Process3 : IProcess3
    {
        public IEventLog EventLog { get; set; }

            public string Name { get; internal set; }
            //public string Key { get; internal set; }

            public int TimeInterval { get; internal set; }
            public int TimeIntervalOffset { get; internal set; }
            public DateTime DatetimeToExecute { get; internal set; }

            public int Status { get; internal set; }
            public bool IsEnabled { get; set;}

            public event EventHandler<Events.EventArgs> NewTickEvent;

            internal Action InitProcedure;
            internal Action MainProcedure;
            internal Action FinishProcedure;

            public Process3()
            {
                IsEnabled = true;
            }

            public Process3(string name, int timeinterval, int timeintervaloffset)
            {
                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
                DatetimeToExecute = DateTime.Now;

                IsEnabled = true;
            }

            public void Init()
            {
                if (EventLog != null)
                    EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process", Name, "Init Process", "", ToString());

                if (InitProcedure != null)
                {
                    InitProcedure();
                }
            }

            public void Main()
            {
                var dt = DateTime.Now;
                if (dt < DatetimeToExecute) return;
                var d = ((dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond + TimeInterval) / TimeInterval) * TimeInterval -
                        (dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond) + TimeIntervalOffset;

                // EventLog.AddItem(EvlResult.SUCCESS, Name + ": " + dt.ToString("T") + " " + dt.Millisecond, d.ToString());

                DatetimeToExecute = dt.AddMilliseconds(d);

                if (MainProcedure != null) MainProcedure();
                //if (NewTickEvent != null) NewTickEvent(dt);

            }
            public void Finish()
            {
                if (EventLog != null)
                    EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process", Name, "Finish Process", ToString(), "");

                if (FinishProcedure != null) 
                {
                    FinishProcedure();
                }
            }

        public string Key {
            get { return  Name.TrimUpper() + "." + TimeInterval + "." + TimeIntervalOffset; }
        }


        public override string ToString()
            {
                return String.Format("Type={0};Name={1};TimiInt={2};TimeOffset={3}", 
                    GetType(), Name, TimeInterval, TimeIntervalOffset);
            }
        }
}
