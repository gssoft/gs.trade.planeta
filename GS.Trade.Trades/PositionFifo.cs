using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Trades
{
    public class PositionFifo : Position
    {
        private List<ITrade3> _trades = new List<ITrade3>();

        public int RegisterNewTrade(ITrade3 t)
        {
            if ( t == null || null != _trades.FirstOrDefault(lt => lt.Key == t.Key) )
                return -1;
            if (IsNeutral)
            {
                AddTrade(t);

                Operation = (PosOperationEnum) t.Operation;
                Quantity = t.Quantity;
                Price1 = t.Price;
                Price2 = t.Price;
                Status = PosStatusEnum.Opened;

                FirstTradeDT = t.DT;
                FirstTradeNumber = t.Number;

                LastTradeDT = t.DT;
                LastTradeNumber = t.Number;

                if (t.Operation == GS.Trade.TradeOperationEnum.Buy)
                {
                    LastTradeBuyPrice = (float) t.Price;
                    LastTradeSellPrice = 0f;
                }
                else if (t.Operation == GS.Trade.TradeOperationEnum.Sell)
                {
                    LastTradeSellPrice = (float) t.Price;
                    LastTradeBuyPrice = 0f;
                }
            }
            else if (IsShort)
            {
            }
            else if (IsLong)
            {
            }

            return +1;
        }

        private int AddTrade(ITrade3 t)
        {
            lock (_trades)
            {
                if (null != _trades.FirstOrDefault(lt => lt.Key == t.Key))
                    return -1;
                _trades.Add(t);
                return +1;
            }
        }
        private void RenoveTrade(ITrade3 t)
        {
            lock (_trades)
            {
                var tr = _trades.FirstOrDefault(lt => lt.Key == t.Key);
                if (tr == null)
                    return;
                 _trades.Remove(tr);
            }
        }
    }
}
