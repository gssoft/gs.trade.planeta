using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using GS.Interfaces;

namespace GS.Process 
{
        public partial class ProcessManager2
        {
            public delegate void ProcessProcedure();
            public delegate void NewTickEventHandler(DateTime newTickDateTime);

            public event NewTickEventHandler NewTickEvent;

            private readonly Thread _processThread;
            internal static IEventLog EventLog { get; private set; }

            public string Name;
            public int TimeInterval { get; set; }
            public int TimeIntervalOffset { get; set; }

            public AutoResetEvent CloseAutoEvent { get; set; }

            private int _finishRequest;

            private readonly Dictionary<string, Process2> _processDictionary = new Dictionary<string, Process2>();

            public void SetEventLog(IEventLog evl)
            {
                if (evl != null) EventLog = evl;
                else
                    throw new NullReferenceException("Process Manager:" + Name + ": EventLog == null" );             
            }
            
            public ProcessManager2(string name, int timeinterval, int timeintervaloffset)
            {
                _processThread = new Thread(Main);

                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;
            }
                    
            public ProcessManager2(string name, int timeinterval, int timeintervaloffset , IEventLog evl )
            {
                EventLog = evl;

                _processThread = new Thread(Main);

                Name = name;
                TimeInterval = timeinterval;
                TimeIntervalOffset = timeintervaloffset;

                EventLog?.AddItem(
                    EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process Manager", "Process Manager", "Init", Name, "");
            }
            public Process2 RegisterProcess( string name, int timeinterval, int timeoffset, 
                ProcessProcedure init, ProcessProcedure main, ProcessProcedure finish)
            {
                var key = Process2.GetKey(name, timeinterval, timeoffset);

                Process2 p;
                if (_processDictionary.TryGetValue(key, out p)) return p;

                p = new Process2
                        {
                            Name = name,
                            Key = key,
                            TimeInterval = timeinterval,
                            TimeIntervalOffset = timeoffset,
                            DatetimeToExecute = DateTime.Now,
                            InitProcedure = init,
                            MainProcedure = main,
                            FinishProcedure = finish,
                            IsEnabled = true
                };
                AddProcess(p);
                return p;
            }

            public Process2 RegisterProcess(string name, string key, int timeinterval, int timeoffset,
                ProcessProcedure init, ProcessProcedure main, ProcessProcedure finish)
            {
                // var key = Process2.GetKey(name, timeinterval, timeoffset);

                Process2 p;
                if (_processDictionary.TryGetValue(key, out p)) return p;

                p = new Process2
                {
                    Name = name,
                    Key = key,
                    TimeInterval = timeinterval,
                    TimeIntervalOffset = timeoffset,
                    DatetimeToExecute = DateTime.Now,
                    InitProcedure = init,
                    MainProcedure = main,
                    FinishProcedure = finish,
                    IsEnabled = true
                };
                AddProcess(p);
                return p;
            }

            private void AddProcess(Process2 p)
            {                
                _processDictionary.Add(p.Key, p);
                EventLog?.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, Name, p.Name,"Register New Process", "Register Main Method" ,p.ToString());
            }

            private void Init()
            {
                EventLog?.AddItem(
                    EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Process Manager"  ,"Init", "Init All Processes", "");

                _finishRequest = 0;

                foreach (var process in _processDictionary.Values)
                {
                    process.Init();
                }
            }
            private void Main()
            {
                while (_finishRequest == 0)
                {
                    //var dt = DateTime.Now;
                    
                   // var d=0;
                  //  if ( /* dt >= _datetimeToExecute && */ _finishRequest == 0)
                  //  {
                        /*
                        d = ((dt.Minute*60 + dt.Second + TimeInterval)/TimeInterval)*TimeInterval -
                                (dt.Minute*60 + dt.Second) + TimeIntervalOffset;
                        */
                    /*
                        var d = ((dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond + TimeInterval) / TimeInterval) * TimeInterval -
                                (dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond) + TimeIntervalOffset;
                     */ 
                        // _datetimeToExecute = dt.AddSeconds(d);

                        //EventLog.AddItem(EvlResult.SUCCESS, "Main: " + dt.ToString("T") + " " + dt.Millisecond, d.ToString() );

                        DateTime d1 = DateTime.Now;

                    NewTickEvent?.Invoke(DateTime.Now);

                    foreach (var process in _processDictionary.Values.Where(process => process.IsEnabled))
                        {
                            process.Main();
                        }
                        if (_finishRequest == 1) break;

                        DateTime d2 = DateTime.Now;
                        if ((d2 - d1).Milliseconds > 1000)
                        {
                            EventLog?.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                                Name, Name, "Main Too Long: " + (d2 - d1).Milliseconds, d1.ToString("H:mm:ss.fff"),
                                d2.ToString("H:mm:ss.fff"));
                            // ((TimeSpan)(d2.TimeOfDay - d1.TimeOfDay)).ToString("H:mm:ss.fff"));
                        }
                    var dt = DateTime.Now;
                        var d = ((dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond + TimeInterval) / TimeInterval) * TimeInterval -
                                    (dt.Minute * 60 * 1000 + dt.Second * 1000 + dt.Millisecond) + TimeIntervalOffset;
                 //   }
                    Thread.Sleep(d);
                }
                if( _finishRequest == 1)
                {
                    XStop();
                }
            }
            
            private bool _finishingProcess;
            private void Finish()
            {
                _finishingProcess = true;
                try
                {
                    EventLog?.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "ProcessManager" , "Start Finish All Processes", "Count:" + _processDictionary.Count, ToString());
                    List<Process2> list;
                    try
                    {
                        //List<Process2> list; // = new List<Process2>();
                        lock (_processDictionary)
                        {
                            list = _processDictionary.Values.ToList();
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Finish Process - Failure. Process.Finish.ToList " + e.Message);
                    }

                    foreach (var process in list)
                    {
                        try
                        {
                            if (process != null)
                            {
                                process.Finish();
                            }
                            else
                            {
                                EventLog?.AddItem(
                                    EvlResult.WARNING, EvlSubject.TECHNOLOGY, Name, "Process=Null", "Finish",
                                    "Finish All Processes", "");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Finish Process - Failure. Process.Finish.Loop " + e.Message);
                        }
                    }
                    EventLog?.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "ProcessManager", "Complete Finish All Processes", "Count:" + _processDictionary.Count, ToString());
                }
                catch (Exception e)
                {
                    throw new Exception("Finish Process - Failure. " + e.Message);
                }
                finally
                {
                    
                    CloseAutoEvent.Set();
                    _finishingProcess = false;
                }

                CloseAutoEvent.Set();
                _finishingProcess = false;
            }
            public void Start()
            {       
                if (_processThread == null) return;
                if (_processThread.IsAlive) return;
                _finishRequest = 0;
                Init();
                _processThread.Start();

                EventLog?.AddItem(
                    EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Start Process Manager", "", "");
            }
            public void Stop()
            {
                if (_processThread == null) return;
                if (!_processThread.IsAlive) return;

                _finishRequest = 1;
                _finishingProcess = true;

                //Thread.Sleep(TimeInterval);
                //Thread.Sleep(1000);
            }
            private void XStop()
            {
                Finish();
              
                //_processThread.Abort();
                //_processThread.Join();

                EventLog?.AddItem(
                    EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Abort Process Manager", "", "");

                //_processDictionary.Clear();
            }

            public void Close()
            {
                //while (_finishingProcess)
                //{
                //}
                //lock (_processDictionary)
                //{
                //  //  _processDictionary.Clear();
                //}
            }

            public override string ToString()
            {
                return
                    $"Type={GetType()};Name={Name};TimiInt={TimeInterval};TimeOffset={TimeIntervalOffset};Count={_processDictionary.Count}";
            }

        }
}