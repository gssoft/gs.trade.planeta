using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.Trade
{
    public interface IPositions
    {
        // void AddPosition(IPosition p);
        void Init(IEventLog evl);
        void GetPositionClosed(string tradeKey, IList<IPosition> ps);
        void GetPositionClosed(int index, IList<IPosition> pcList);
        IPosition Register2(string account, string strategy, ITicker ticker);

        void PositionCalculate5(ITrade t);

        event EventHandler<IPosition> NewPositionClosed;
        event EventHandler<IPositionsEventArgs> PositionsEvent;

        void GetPositionsOpenedToObserve();
        void GetAllCurrents(List<IPosition> ps );
    }

    public interface IPositionsEventArgs
    {
        string WhatsHappens { get; }
        IPosition Position { get; }
    }
}
