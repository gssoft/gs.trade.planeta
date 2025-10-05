using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Exceptions;
using GS.Serialization;
using GS.Trade.DataBase.Init;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class DbStorage33 : TradeBaseStorage
    {
        public string DataBaseName { get; set; }

        public DbStorage33()
        {
            Database.SetInitializer(new InitDb());

            //DataBaseName = "DbTrade2";

            GSExceptions = Builder.
                Build2<IEntityRepository<IGSExceptionDb, IGSException>>(@"Init\TradeRepositories.xml", "ExceptionDbRepository");
            GSExceptions.Parent = this;

            Accounts = Builder.Build2<IAccountRepository32>(@"Init\TradeRepositories.xml", "AccountRepository33");
            Accounts.Parent = this;         

            Tickers = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository33");
            Tickers.Parent = this;

            Strategies = Builder.Build2<IEntityRepository<IStrategyDb, IStrategy>>(@"Init\TradeRepositories.xml", "StrategyRepository33");
            Strategies.Parent = this;
         
            Orders = Builder.Build2<IEntityRepository<IOrderDb, IOrder3>>(@"Init\TradeRepositories.xml", "OrderRepository33");
            Orders.Parent = this;

            Trades = Builder.Build2<IEntityRepository<ITradeDb, ITrade3>>(@"Init\TradeRepositories.xml", "TradeRepository33");
            Trades.Parent = this;

            Positions = Builder.Build2<IEntityRepository<IPositionDb, IPosition2>>(@"Init\TradeRepositories.xml", "PositionRepository33");
            Positions.Parent = this;

            PositionTotals = Builder.
                Build2<IEntityRepository<IPositionDb, IPosition2>>(@"Init\TradeRepositories.xml", "PositionTotalRepository33");
            PositionTotals.Parent = this;     

            Deals = Builder.Build2<IEntityRepository<IDealDb, IDeal>>(@"Init\TradeRepositories.xml", "DealRepository33");
            Deals.Parent = this;
        }
    }
}
