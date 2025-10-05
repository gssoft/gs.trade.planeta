using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Process
{
    
    public interface IProcess
    {
        void Init();

        void Execute();
        
        void Finish();
    }
    
}
