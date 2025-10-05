using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Containers;

namespace GS.Trade.Strategies.Managers
{
    //public class TradestrategyManagers : SetContainer<string, ITradeStrategyManager>
    //{
    //    virtual public ITradeStrategyManager Register(ITradeStrategyManager p)
    //    {
    //        return AddNew(p);
    //    }
    //}
    //public class TradeStrategyManager : ListContainer<string, IStrategy>, ITradeStrategyManager
    //{
    //    public string Code { get; set; }
    //    public string Name { get; set; }

    //    private int _position;
    //    public int Position
    //    {
    //        get
    //        {
    //            return ItemCollection.Aggregate(0, (current, s) => (int)(current + (((IStrategy)s).Position.Pos)));
    //        }
    //    }

    //    public string Key
    //    {
    //        get { return Code; }
    //    }

    //    public IStrategy Register(IStrategy strategy)
    //    {
    //        return AddNew(strategy);
    //    }

    //    public bool IsShort { get { return Position < 0; } }
    //    public bool IsLong { get { return Position > 0; } }
    //    public bool IsNeutral { get { return Position == 0; } }

    //    public override string ToString()
    //    {
    //        return string.Format("[Code={0}; Name={1}; Key={2}]", Code, Name, Key);
    //    }
    //}
   
}
