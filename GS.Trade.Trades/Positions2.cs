using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.Trade.Trades
{
    public class Positions2 : IPositions
    {
        public void Init(IEventLog evl)
        {
            throw new NotImplementedException();
        }

        public void GetPositionClosed(string tradeKey, IList<IPosition> ps)
        {
            throw new NotImplementedException();
        }

        public void GetPositionClosed(int index, IList<IPosition> pcList)
        {
            throw new NotImplementedException();
        }

        public IPosition Register2(string account, string strategy, ITicker ticker)
        {
            throw new NotImplementedException();
        }

        public void PositionCalculate5(ITrade t)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<IPosition> NewPositionClosed;
        public event EventHandler<IPositionsEventArgs> PositionsEvent;
        public void GetPositionsOpenedToObserve()
        {
            throw new NotImplementedException();
        }

        public void GetAllCurrents(List<IPosition> ps)
        {
            throw new NotImplementedException();
        }
    }
}
