using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Interfaces;

namespace GS.Process
{
        abstract public class Process : IProcess
        {
            protected string Name;

            protected int TimeInterval;
            protected int TimeIntervalOffset;
            protected DateTime DatetimeToExecute;

            protected short Status;

            protected Process(string name, int timeinterval, int timeintervaloffset)
            {
                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
                DatetimeToExecute = DateTime.Now;
            }
            public void Init()
            {
                InitProcess();
            }
            public void Execute()
            {
                var dt = DateTime.Now;
                if (dt < DatetimeToExecute) return;
                var d = ((dt.Minute * 60 + dt.Second + TimeInterval) / TimeInterval) * TimeInterval -
                        (dt.Minute * 60 + dt.Second) + TimeIntervalOffset;

                DatetimeToExecute = dt.AddSeconds(d);

                ExecuteProcess();

            }
            public void Finish()
            {
                FinishProcess();
            }
            protected abstract void InitProcess();
            protected abstract void ExecuteProcess();
            protected abstract void FinishProcess();
        }
}
