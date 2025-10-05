using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;

namespace GS.Status
{
    public enum SSStatusEnum
    {
        Unknown=0,
        Starting, Started, TryToStart,
        Working, Running,
        Stopping, Stopped, TryToStop,
        Finishing, Finished, TryToFinish,
        Completed, TryToComplete,
        Canceled, TryToCancel,
        Faulted  
    }
    public class StartStopStatus // : Element1<String>
    {
        public DateTime ChangedDT { get; set; }

        public StartStopStatus()
        {
            Status = SSStatusEnum.Unknown;
        }
        public bool IsActive {
            get
            {
                return
                    Status == SSStatusEnum.Starting ||
                    Status == SSStatusEnum.Running;
            }
        }

        public event EventHandler<StateChange> StateChanged;

        protected virtual void OnStateChanged(StateChange e)
        {
            var handler = StateChanged;
            if (handler != null) handler(this, e);
        }

        private  SSStatusEnum _status ;
        public SSStatusEnum Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                    return;
                var old = _status;
                _status = value;
                ChangedDT = DateTime.Now;

                var sc = new StateChange(_status, old);

                //if (IsEvlEnabled)
                //    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentCode, ParentCode, sc.ToString(), "", "");

                OnStateChanged(sc);
            } 
        }
        //public override string Key
        //{
        //    get { return Code.HasValue() ? FullCode : GetType().FullName; }
        //}
        public override string ToString()
        {
            return String.Format("Status: {0}, Changed: {1}", Status, ChangedDT.ToString("G"));
        }
    }

    public class StateChange
    {
        public StateChange(SSStatusEnum newState, SSStatusEnum oldState)
        {
            OldState = oldState;
            NewState = newState;
        }
        public SSStatusEnum NewState { get; private set; }
        public SSStatusEnum OldState { get; private set; }

        public override string ToString()
        {
            //return String.Format("Status Changed: NewState: {0}, OldState: {1}", NewState, OldState);
            return String.Format("NewState: {0}, OldState: {1}", NewState, OldState);
        }
    }
}
