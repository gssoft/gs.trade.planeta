using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GS.Interfaces;

namespace GS.Process
{
    public class SimpleProcess
    {
        public delegate void ExecuteProcess();

        private readonly IEventLog _evl;

        private readonly Thread _processThread;
        protected string Name;

        protected int TimeInterval;
        protected int TimeIntervalOffset;
        protected DateTime DatetimeToExecute;

        private volatile short _finishRequest;

        public ExecuteProcess CallbackExecuteProcess;

        protected short Status;

        public SimpleProcess(string name, int timeinterval, int timeintervaloffset, ExecuteProcess cb)
        {
                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
                DatetimeToExecute = DateTime.Now;

                CallbackExecuteProcess = cb;
                _processThread = new Thread(Execute);

            _finishRequest = 0;
        }
        public SimpleProcess(string name, int timeinterval, int timeintervaloffset, ExecuteProcess cb, IEventLog evl)
        {
            Name = name;
            TimeInterval = timeinterval;
            TimeIntervalOffset = timeintervaloffset;
            DatetimeToExecute = DateTime.Now;

            CallbackExecuteProcess = cb;
            _processThread = new Thread(Execute);

            _evl = evl;

            _finishRequest = 0;
        }
        public void Execute()
        {
            while (true)
            {            
                var dt = DateTime.Now;
                //if (dt < DatetimeToExecute) continue;
                var d = ((dt.Minute*60 + dt.Second + TimeInterval)/TimeInterval)*TimeInterval -
                        (dt.Minute*60 + dt.Second) + TimeIntervalOffset;

                DatetimeToExecute = dt.AddSeconds(d);

                if (CallbackExecuteProcess != null) CallbackExecuteProcess();

                Thread.Sleep(d*1000);
                if (_finishRequest > 0) return;
            }
        }
        public void Start()
        {
            if (_processThread.IsAlive) return;

            _finishRequest = 0;
            _processThread.Start();

            if (_evl != null)
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Start", ToString(), "");
        }
        public void Stop()
        {
            if (_processThread.IsAlive)
            {
                _finishRequest = 1;
                // Thread.Sleep(TimeInterval * 1000);
                _processThread.Abort();
                _processThread.Join();

                if (_evl != null)
                    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Stop", ToString(), "");
                return;
            }
            
                if (_evl != null)
                    _evl.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Name, Name, "Already Stoped", ToString(), "");

                return;
            
        }

        public override string ToString()
        {
            return String.Format("Name={0},TimeInt={1},TimeOffset={2}", Name, TimeInterval, TimeIntervalOffset);
        }

    }
}
