using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Trade.Data.Chart;
using GS.Trade.Trades;

namespace GS.Trade.Strategies
{
    public  abstract partial class Strategy : IChartable
    {

        public virtual IBandSeries Band
        {
            get { return null; }
        }
        public virtual IBandSeries Band2
        {
            get { return null; }
        }

        public virtual IList<IBandSeries> Bands
        {
            get { return null; }
        }
        public virtual IList<ILineSeries> ChartLines
        {
            get { return null; }
        }

        public virtual IChartDataContainer ChartDataContainer
        {
            get { return null; }
        }

        public virtual ILevelCollection Levels
        {
            get { return null; }
        }
        public virtual ILevelCollection Levels2
        {
            get { return null; }
        }
        public virtual ILineSeries LineSeries
        {
            get { return null; }
        }

        public IEnumerable<ILevel> GetActiveOrderLevels()
        {
            var ll = new List<ILevel>();

            var color = 0;
            var backGroundColor = 0;
            var text = string.Empty;

            //var li = new List<Order>();
            //lock (MyOrderCollection)
            //{
            //    li
            //}
            try
            {
                lock (OrderCollectionLocker)
                {
                    //foreach (var o in MyOrderCollection.ToList()) //.Where(o => o.IsActive))
                    foreach (var o in ActiveOrders)
                    {
                        if (!o.IsActive) continue;

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
                        var l = new Level
                        {
                            Value = price,
                            Color = color,
                            BackGroundColor = backGroundColor,
                            Text = text,
                            IsValid = () => true
                        };
                        ll.Add(l);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Strategy.GetActiveOrderLevels() Failure: " + e.Message);
            }
            var p = Position;
                if( p.IsOpened)
                {
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
                                    Text = text,
                                    IsValid = () => true
                                };
                    ll.Add(l);

                    ll.Add(new Level
                    {
                        Value = (double)p.Price3,
                        Color = color,
                        BackGroundColor = backGroundColor,
                        Text = "Price3: ",
                        IsValid = () => p.Price1 != p.Price3
                    });
                    
                }
            return ll;
        }
        public virtual IEnumerable<ILevel> ActiveOrderLevels
        {
            get{ return GetActiveOrderLevels(); }
        }

        public IEnumerable<ILineXY> GetClosedPositionLines()
        {
            var ll = new List<ILineXY>();
            lock (PositionCollectionLocker)
            {
                MyPositionCollection.Clear();
                TradeContext.Positions.GetPositionClosed(TradeKey, MyPositionCollection);

                ll.AddRange(MyPositionCollection.Select
                    (
                        p => new ChartLineXY
                            {
                                LineX1 = p.FirstTradeDT,
                                LineY1 = (double) p.Price1,
                                LineX2 = p.LastTradeDT,
                                LineY2 = (double) p.Price2,
                                Color = p.PnL > 0 ? 0x0000ff : 0xff0000,
                                Width = 2
                            }
                    ));
            }
            return ll;
        }

       

    }
}
