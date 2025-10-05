using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Interfaces;

namespace GS.Process
{
    public interface IProcessSlot3
    {
        string Name { get; }
        IEnumerable<IProcess3> Items { get; }
    }

    public class ProcessSlot3 : IProcessSlot3
    {
        public string Name { get; set; }
        private readonly Dictionary<string, IProcess3> _processDictionary;

        [XmlIgnore]
        public ProcessManager3 Parent { get; set; }

        public ProcessSlot3()
        {
            _processDictionary = new Dictionary<string, IProcess3>();
        }
        public IEnumerable<IProcess3> Items {
            get { return _processDictionary.Values; }
        }
        public IProcess3 RegisterProcess(IProcess3 pnew)
        {
            IProcess3 p;
            if (_processDictionary.TryGetValue(pnew.Key, out p))
                return p;
            AddProcess(pnew);
            return pnew;
        }
        public IProcess3 RegisterProcess(string name, int timeinterval, int timeoffset,
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
            if (Parent != null && Parent.EventLog != null)
                Parent.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Process", Name, "Register New Process", p.ToString(), "");
        }
    }
}
