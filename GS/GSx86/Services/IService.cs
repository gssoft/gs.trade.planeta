using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GS.Services
{
    public interface ISimpleService : IDisposable
    {
        string Name { get;  }
        void Init();
        void Start();
        void Stop();

        event EventHandler<string> LogMsgEvent;
    }

    public interface IService : ISimpleService
    {
        void Pause();
        void Continue();
    }
}
