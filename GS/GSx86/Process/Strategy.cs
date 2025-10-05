using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GS.Process
{
    abstract class Strategy
    {
            private readonly Thread _processThread;
            private readonly string _name;

            public int TimeInterval { get; set; }
            public int TimeIntervalOffset { get; set; }

            private DateTime _datetimeToExecute;
            private int _finishRequest = 0;

            private readonly List<IProcess> _processCollection;

        protected Strategy()
            {
                _processCollection = new List<IProcess>();

                _datetimeToExecute = DateTime.Now;
                _processThread = new Thread( Execute );

                _name = "Strategy Main Process";
                TimeInterval = 1;
                TimeIntervalOffset = 0;
            }

        protected Strategy(string name, int timeinterval, int timeintervaloffset)
            {
                _processCollection = new List<IProcess>();

                _datetimeToExecute = DateTime.Now;
                _processThread = new Thread(Execute);

                _name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
            }

            public void AddProcess(IProcess pr)
            {
                _processCollection.Add(pr);
            }
            private void Init()
            {
                //Console.WriteLine("{0} {1} Init Process", DateTime.Now, _name);

                _datetimeToExecute = DateTime.Now;
                _finishRequest = 0;

                foreach (var process in _processCollection)
                {
                    process.Init();
                }
            }
            private void Execute()
            {
                while (true)
                {
                    // Console.WriteLine("{0} {1} Manager -- Execute Process", DateTime.Now, _datetimeToExecute);

                    var dt = DateTime.Now;
                    var d=0;
                    if (dt >= _datetimeToExecute && _finishRequest == 0)
                    {
                        d = ((dt.Minute*60 + dt.Second + TimeInterval)/TimeInterval)*TimeInterval -
                                (dt.Minute*60 + dt.Second) + TimeIntervalOffset;
                        _datetimeToExecute = dt.AddSeconds(d);
                        Console.WriteLine("{0} {1} {2} {3} Execute Process", DateTime.Now, _datetimeToExecute, d, _name);

                        foreach (var process in _processCollection)
                        {
                            process.Execute();
                        }
                    }
                    Thread.Sleep(d*1000);
                }
            }
            private void Finish()
            {
                //Console.WriteLine("{0} {1} Finish Process", DateTime.Now, _name);
                foreach (var process in _processCollection)
                {
                    process.Finish();
                }
            }
            public void Start()
            {   
                Init();
                if (_processThread == null) return;

                if (!_processThread.IsAlive)
                {
                    this._processThread.Start();
                }
                else
                {
                }
            }
            public void Stop()
            {
                if (_processThread == null) return;

                if (_processThread.IsAlive)
                {
                   // _datetimeToExecute = _datetimeToExecute.AddYears(1);
                    _finishRequest = 1;
                    Thread.Sleep(TimeInterval * 1000);

                    ////Console.WriteLine("{0} {1} {2} Finish Process", DateTime.Now, _datetimeToExecute, _name);

                    Finish();

                    _processThread.Abort();
                    _processThread.Join();
                }
                else
                {
                }
            }
        }
    class StrategyProcess
    {
        
    }

}
