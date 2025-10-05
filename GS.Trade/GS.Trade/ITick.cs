using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade
{
    public interface ITick
    {
        DateTime DT { get; }

        ITicker Ticker { get; }
        double Last {get;}
        double Volume { get; set; }
    }
    public interface ITickSimple : IBarSimple
    {
        double Last { get; }
    }
    public interface IQuote : ITick
    {
        double Bid { get; }
        double Ask { get; }
    }

}
