using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Interfaces
{
    public interface IHaveInit
    {
        //void Init();
        void Init(IEventLog ievl);
    }
    // Use in Element1 from 2018.05.28
    public interface IHaveInit<in T>
    {
        void Init();
        // Init for EventLog Init
        void Init(T t);
    }
}
