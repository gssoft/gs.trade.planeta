namespace GS.Trade.Trades.Deals
{
    public class Deals : Containers5.ListContainer<string, IPosition2>, IDeals
    {
        public override bool AddNew(IPosition2 p)
        {
            if (!base.Add(p))
                    return false;

            p.Strategy.Strategies.OnStrategyTradeEntityChangedEvent(new Events.EventArgs
            {
                Category = "Deals",
                Entity = "Deal",
                Operation = "Add",
                Object = p
            });
            return true;
        }
    }
}
