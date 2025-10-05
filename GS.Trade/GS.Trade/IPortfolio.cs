using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Containers;

namespace GS.Trade
{
    public interface IPortfolio : IContainerItem<string>
    {
        int Position { get; }

        IEnumerable<IStrategy> Items { get; }
        long Count { get; }

        IStrategy Register(IStrategy s);

        bool IsShort { get; }
        bool IsLong { get; }
        bool IsNeutral { get; }
    }

    public interface ITradeStrategyManager : IContainerItem<string>
    {
        int Position { get; }

        IEnumerable<IStrategy> Items { get; }
        long Count { get; }

        IStrategy Register(IStrategy s);

        bool IsShort { get; }
        bool IsLong { get; }
        bool IsNeutral { get; }
    }
    public interface IEntryManager : IContainerItem<string>
    {
        //int Position { get; }

        //IEnumerable<IStrategy> Items { get; }
        //long Count { get; }

        IStrategy Register(IStrategy s);
        void Init();
        void PositionChanged(IPosition old, IPosition n, PositionChangedEnum e);

        //bool IsShort { get; }
        //bool IsLong { get; }
        //bool IsNeutral { get; }
    }
}
