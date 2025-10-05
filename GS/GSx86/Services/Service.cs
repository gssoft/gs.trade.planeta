using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Services;

namespace GS.Services
{
    abstract public class Service  : IService
    {
        public string Name { get; set; }
        public event EventHandler<string> LogMsgEvent;
        protected virtual void OnLogMsgEvent(string e)
        {
            EventHandler<string> handler = LogMsgEvent;
            if (handler != null) handler(this, e);
        }
        abstract public void Init();
        abstract public  void Start();
        abstract public  void Stop();
        abstract public void Pause();
        public abstract void Continue();
        public void Dispose()
        {
          //  throw new NotImplementedException();
        }
    }
}
