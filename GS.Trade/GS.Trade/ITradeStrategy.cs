using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade
{
    public interface ITradeStrategy
    {
        string Code{get;}
        string Key {get;}

        int Pos {get;}

        void SetMode(int mode);
    }
}
