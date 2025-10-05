using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;

namespace GS.Process
{
    public class ProcessManager3
    {
            //public delegate void ProcessProcedure();
            
        public delegate void NewTickEventHandler(DateTime newTickDateTime);
          
        public event NewTickEventHandler NewTickEvent;

        private readonly Thread _processThread;
        [XmlIgnore]
        public IEventLog EventLog { get; set; }

        public string Name { get; set; }
        public int TimeInterval { get; set; }
        public int TimeIntervalOffset { get; set; }

        public List<ProcessSlot3> ProcessSlots { get; set; }

        private int _finishRequest;
            
        private readonly Dictionary<string, IProcess3> _processDictionary = new Dictionary<string, IProcess3>();

        public ProcessManager3()
        {
            _processThread = new Thread(Main);
            ProcessSlots = new List<ProcessSlot3>();
        }

        public IProcess3 RegisterProcess(IProcess3 pnew)
        {
            IProcess3 p;
            if (_processDictionary.TryGetValue(pnew.Key, out p))
                return p;
            AddProcess(pnew);
            return pnew;
        }
        public IProcess3 RegisterProcess( string name, int timeinterval, int timeoffset, 
                Action init, Action main, Action finish)
            {
                IProcess3 p;
                var pnew = new Process3
                {
                    Name = name,
                    TimeInterval = timeinterval,
                    TimeIntervalOffset = timeoffset,
                    DatetimeToExecute = DateTime.Now,
                    InitProcedure = init,
                    MainProcedure = main,
                    FinishProcedure = finish,
                    IsEnabled = true
                };
                if (_processDictionary.TryGetValue(pnew.Key, out p))
                        return p;
                AddProcess(pnew);
                return pnew;
            }
            private void AddProcess(IProcess3 p)
            {                
                _processDictionary.Add(p.Key, p);
                if (EventLog != null)
                    EventLog.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, "Process", Name,"Register New Process", p.ToString(),"");
            }

        public IProcessSlot3 this[string index]
        {
            get
            {
                return ProcessSlots.FirstOrDefault(ps => string.Equals(ps.Name, index,
                    StringComparison.CurrentCultureIgnoreCase));
            }
        }


            public void Init()
            {        
                if (EventLog != null)
                    EventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process", Name, "Init", "Init All Processes", "");
                
                _finishRequest = 0;

                //ProcessSlots.Add(new ProcessSlot3 {Name = "One"});
                //ProcessSlots.Add(new ProcessSlot3 {Name = "Two"});
                //ProcessSlots.Add(new ProcessSlot3 {Name = "THree"});

            foreach (var ps in ProcessSlots)
                ps.Parent = this;

            foreach (var process in _processDictionary.Values)
            {
                    process.Init();
            }

            foreach (var p in ProcessSlots.SelectMany(ps => ps.Items))
            {
                p.Init();
            }
            
            }
            public bool Serialize()
            {
                string xmlfname = null;
                TextWriter tr = null;
                try
                {
                    tr = new StreamWriter("ProcessManager3.xml");
                    var sr = new XmlSerializer(typeof(ProcessManager3));
                    sr.Serialize(tr, this);
                    tr.Close();

                    return true;
                }
                catch (Exception e)
                {

                    if (tr != null) tr.Close();
                    return false;
                }
            }

        public bool Serialize(string file)
        {
           return  GS.Serialization.Do.Serialize(file, this);
        }

        private void Main()
            {
                while (_finishRequest == 0)
                {

                        DateTime d1 = DateTime.Now;

                        if (NewTickEvent != null) NewTickEvent(DateTime.Now);

                        foreach (var process in _processDictionary.Values)  //.Where(process => process.IsEnabled))
                        {
                            if (!process.IsEnabled) continue;

                            process.Main();
                        }
                        if (_finishRequest == 1) break;
                      
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
            private void Finish()
            {
                if (EventLog != null)
                    EventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish", "Finish All Processes", "");

                foreach (var process in _processDictionary.Values)
                {
                    process.Finish();
                }
            }
            public void Start()
            {       
                if (_processThread == null) return;
                if (_processThread.IsAlive) return;
                _finishRequest = 0;
                Init();
                _processThread.Start();
                                      
                if (EventLog != null)
                    EventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Start Process Manager", "", "");
            }
            public void Stop()
            {
                if (_processThread == null) return;
                if (!_processThread.IsAlive) return;
                _finishRequest = 1;

                //Thread.Sleep(TimeInterval);
                //Thread.Sleep(1000);
            }
            private void XStop()
            {
                Finish();
                /*
                    if (EventLog != null)
                        EventLog.AddItem(
                            EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, "Finish", "Finish All Processes", "");
                    */
                _processThread.Abort();
                _processThread.Join();

                if (EventLog != null)
                    EventLog.AddItem(
                        EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Abort Process Manager", "", "");

                //_processDictionary.Clear();
            }

            public void Close()
            {
                _processDictionary.Clear();
            }

            public override string ToString()
            {
                return  String.Format("Type={0};Name={1};TimiInt={2};TimeOffset={3};Count={4}", 
                    GetType(), Name, TimeInterval, TimeIntervalOffset,_processDictionary.Count);
            }

        }
}
