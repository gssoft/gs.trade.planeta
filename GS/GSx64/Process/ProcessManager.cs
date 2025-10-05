using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GS.Interfaces;

namespace GS.Process 
{
    //using EventLog;
        public class ProcessManager
        {
            private readonly Thread _processThread;
            private readonly IEventLog _eventLog;
            private string _name;
            public int TimeInterval { get; set; }
            public int TimeIntervalOffset { get; set; }

            private DateTime _datetimeToExecute;
            private int _finishRequest = 0;

            private readonly List<IProcess> _processCollection;

            public ProcessManager()
            {
                _processCollection = new List<IProcess>();

                _datetimeToExecute = DateTime.Now;
                _processThread = new Thread( Execute );

                _name = "Process Manager";
                TimeInterval = 1;
                TimeIntervalOffset = 0;
            }
            
            public ProcessManager(string name, int timeinterval, int timeintervaloffset)
            {
                _processCollection = new List<IProcess>();

                _datetimeToExecute = DateTime.Now;
                _processThread = new Thread(Execute);

                _name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
            }
                    
            public ProcessManager(string name, int timeinterval, int timeintervaloffset , IEventLog evl )
            {
                _eventLog = evl;
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
                if (_eventLog != null)
                    _eventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process Manager", _name, "Init Process Manager", "Init All Processes", "");
                
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
                    var dt = DateTime.Now;
                    var d=0;
                    if (dt >= _datetimeToExecute && _finishRequest == 0)
                    {
                        d = ((dt.Minute*60 + dt.Second + TimeInterval)/TimeInterval)*TimeInterval -
                                (dt.Minute*60 + dt.Second) + TimeIntervalOffset;
                        _datetimeToExecute = dt.AddSeconds(d);

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
                
                if (_eventLog != null)
                    _eventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, _name.ToString(), _name.ToString(), "Finish Process Manager", "Finish All Processes", "");
                
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
                    _processThread.Start();
                                      
                    if (_eventLog != null)
                        _eventLog.AddItem(
                            EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, _name.ToString(), _name.ToString(), "Start Process Manager", "", "");
                      
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

                    Finish();
                    
                    if (_eventLog != null)
                        _eventLog.AddItem(
                            EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, _name.ToString(), _name.ToString(), "Stop Process Manager", "Finish All Processes", "");
                    
                    _processThread.Abort();
                    _processThread.Join();
                    
                    if (_eventLog != null)
                        _eventLog.AddItem(
                            EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, _name.ToString(), _name.ToString(), "Stop Process Manager", "", "");
                      
                }
                else
                {
                }
            }
        }
}