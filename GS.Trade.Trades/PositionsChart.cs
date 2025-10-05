using System.Collections.Generic;
using System.Linq;
using GS.ICharts;

namespace GS.Trade.Trades
{
    public partial class Positions : ILevelCollection, ILineXYCollection
    {
        private readonly List<ILevel> _levelCollection = new List<ILevel>();
        private readonly List<ILineXY> _lineXYCollection = new List<ILineXY>();

        public IList<ILevel> GetLevelCollection(string tradeKey)
        {
            _levelCollection.Clear();
            lock (_lockPosition)
            {
                foreach (var p in PositionCollection.Where(
                                p => string.IsNullOrWhiteSpace(tradeKey)
                                    ? p.IsOpened
                                    : p.IsOpened && p.TradeKey == tradeKey))
                {
                    var color = 0;
                    var backGroundColor = 0;
                    var text = string.Empty;

                    if (p.IsLong)
                    {
                        color = 0;
                        backGroundColor = 0x64c8ff;
                        text = "Long: +" + p.Quantity;
                    }
                    else if (p.IsShort)
                    {
                        color = 0;
                        backGroundColor = 0xffc8c8;
                        text = "Short: -" + p.Quantity;
                    }

                    var l = new Level
                                {
                                    Value = (double) p.Price1,
                                    Color = color,
                                    BackGroundColor = backGroundColor,
                                    Text = text
                                };
                    _levelCollection.Add(l);
                }
            }
            return _levelCollection;
        }
        public IList<ILineXY> GetLineXYCollection(string tradeKey)
        {
            _lineXYCollection.Clear();

            lock (_lockPosition)
            {
                foreach (var p in PositionCollection.Where(
                    p => string.IsNullOrWhiteSpace(tradeKey)
                        ? p.IsClosed && ! p.IsNeutral
                        : p.IsClosed && !p.IsNeutral && p.TradeKey == tradeKey))
                {
                    _lineXYCollection.Add(p);
                }
            }
            return _lineXYCollection;
        }
        public void GetPositionClosed(string tradeKey, IList<IPosition> ps)
        {
           // ps.Clear();
            lock (_lockPosition)
            {
                foreach (var p in PositionCollection.Where(
                    p => string.IsNullOrWhiteSpace(tradeKey)
                        ? p.IsClosed && !p.IsNeutral
                        : p.IsClosed && !p.IsNeutral && p.TradeKey == tradeKey))
                {
                    ps.Add(p);
                }
            }
        }
    }
}
