using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Queues;
using GS.Works;
using GS.WorkTasks;

namespace GS.Works
{

    public enum WorkStatusEnum
    {
        Unknown, Initialization, Working, Faulted, Finishing, Finished, Created, Canceled 
    }

    public class Work1<T> : Element1<string>, IWork1<T>
    {
        public IWorkTask3 WorkTask { get; set; }
        public QueueFifo2<T> InputQueue { get; set; }

        public WorkStatusEnum Status { get; private set; }
        public Func<bool> InitFunc { get; set; }
        public Func<bool> MainFunc { get; set; }
        public Func<bool> FinishFunc { get; set; }

        public int TimeInterval { get; set; }

        [XmlIgnore]
        public int ErrorCount { get; private set; }
        public int ErrorCountToStop { get; set; }
        public long Iterations { get; set; }
        public DateTime FirstDT { get; private set; }
        public DateTime LastDT { get; private set; }

        public void EnQueue(T t)
        {
            InputQueue.Push(t);
            WorkTask?.DoWork();
        }

        public bool DoWork()
        {
            if (WorkTask == null)
                return false;
            
            WorkTask.DoWork();
            return true;
        }

        public string Message { get; private set; }

        public Work1()
        {
            IsEnabled = true;
            InputQueue = new QueueFifo2<T>();
            FirstDT = LastDT = DateTime.MinValue;
        }

        public void Init()
        {

        }

        public bool InitWorks()
        {
            if (InitFunc == null)
                return false;
            SetDT();
            ConsoleAsync.WriteLineT("Work: {0} Init...", WorkFullName);
            try
            {
                Status = WorkStatusEnum.Initialization;
                if (InitFunc())
                {
                    Status = WorkStatusEnum.Created;
                    return true;
                }
                Status = WorkStatusEnum.Faulted;
                return false;
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                Status = WorkStatusEnum.Faulted;
            }
            return false;
        }

        public bool Main()
        {
            if (MainFunc == null)
                return false;

            if (!IsEnabled)
                return false;

            SetDT();
            Iterations++;
            Status = WorkStatusEnum.Working;
            try
            {
                if (MainFunc())
                    return true;
                // return !IsErrorLimitReached();
            }
            catch (Exception e)
            {
                IsEnabled = false;
                
                OnExceptionEvent(e);
            }
            return !IsErrorLimitReached();
        }

        public bool Finish()
        {
            if (FinishFunc == null)
                return false;
            SetDT();
            ConsoleAsync.WriteLineT("Work: {0} Finish...", WorkFullName);
            try
            {
                Status = WorkStatusEnum.Finishing;
                if (FinishFunc())
                {
                    Status = WorkStatusEnum.Finished;
                    return true;
                }
                Status = WorkStatusEnum.Faulted;
                return false;
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                Status = WorkStatusEnum.Faulted;
            }
            return false;
        }

        public bool Do()
        {
            //return Main();
            WorkTask?.DoWork();
            return true;
        }

        private bool IsErrorLimitReached()
        {
            if (++ErrorCount < ErrorCountToStop)
                return false;
            Status = WorkStatusEnum.Faulted;
            Message = $"ErrorLimit {ErrorCountToStop} is Reached. ErrorCount: {ErrorCount}";
            return true;
        }

        public override string Key
        {
            get { return Code.HasValue() ? Code : GetType().FullName; }
        }

        public string WorkFullName
        {
            get
            {
                var parent = Parent;
                var str = new StringBuilder(Code);
                while (parent != null)
                {
                    str.Append(@"\" + parent.Code);
                    parent = parent.Parent;
                }
                return str.ToString();
            }
        }

        private void SetDT()
        {
            if (FirstDT == DateTime.MinValue)
                FirstDT = DateTime.Now;
            LastDT = DateTime.Now;
        }
    }
}
