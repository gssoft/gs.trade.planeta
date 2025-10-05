using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade
{
    public interface IStrategyManager
    {
        string Name { get; }
        string Code { get; }
        string Key {get;}
    }
}
