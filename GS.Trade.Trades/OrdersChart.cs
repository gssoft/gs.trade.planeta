using System.Collections.Generic;
using System.Linq;
using GS.ICharts;

namespace GS.Trade.Trades
{
    public partial class Orders : ILevelCollection
    {
        private readonly List<ILevel> _levelCollection = new List<ILevel>();
        public IList<ILevel> GetLevelCollection(string tradeKey)
        {
            _levelCollection.Clear();

            lock (_orderLocker)
            {
                foreach (var o in OrderCollection.Where(
                        o => string.IsNullOrWhiteSpace(tradeKey)
                                 ? o.IsActive
                                 : o.IsActive && o.TradeKey == tradeKey))
                {
                    var color = 0;
                    var backGroundColor = 0;
                    var text = string.Empty;

                    if (o.IsBuy)
                    {
                        //color = 0x0000ff;
                        color = 0;
                        backGroundColor = 0xa5e9ff;
                        text = "Buy ";
                    }
                    else if (o.IsSell)
                    {
                        color = 0;
                        backGroundColor = 0xffc8c8;
                        text = "Sell ";
                    }

                    var price = 0d;

                    if (o.IsLimit)
                    {
                        text += "Limit";
                        price = o.LimitPrice;
                    }
                    else if (o.IsStopLimit)
                    {
                        text += "Stop";
                        price = o.StopPrice;
                    }

                    var l = new Level {Value = price, Color = color, BackGroundColor = backGroundColor, Text = text};
                    _levelCollection.Add(l);
                }
            }
            return _levelCollection;
        }
    }
}
