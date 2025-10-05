using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Elements;
using GS.Extension;
using GS.Works;

namespace GS.Works
{
    using GS.ConsoleAS;
    public abstract class Work2 : Element1<string>, IWork2<string>
    {
        public WorkStatusEnum Status { get; private set; }

        public abstract bool InitFunc();
        public abstract bool MainFunc();
        public abstract bool FinishFunc();

        [XmlIgnore]
        public int ErrorCount { get; private set; }
        public int ErrorCountToStop { get; set; }
        public string Message { get; private set; }

        protected Work2()
        {
            IsEnabled = true;
        }

        public bool Init()
        {
            //if (InitFunc == null)
            //    return false;
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
            if (!IsEnabled)
                return false;
           
            Status = WorkStatusEnum.Working;
            try
            {
                if (MainFunc())
                    return true;
                // return !IsErrorLimitReached();
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
            return !IsErrorLimitReached();
        }

        public bool Finish()
        {
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
            return Main();
        }

        private bool IsErrorLimitReached()
        {
            if (++ErrorCount < ErrorCountToStop)
                return false;
            Status = WorkStatusEnum.Faulted;
            Message = string.Format("ErrorLimit {0} is Reached. ErrorCount: {1}",
                                    ErrorCountToStop, ErrorCount);
            return true;
        }

        public override string Key
        {
            get { return Code.HasValue() ? Code : "Unknown"; }
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
    }
}
