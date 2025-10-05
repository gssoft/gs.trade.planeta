using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Configurations;
using GS.Exceptions;
using GS.Serialization;
using GS.Trade.DataBase.Init;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class DbStorage34 : TradeBaseStorage
    {
        public string DataBaseName { get; set; }

        public DbStorage34()
        {
            Database.SetInitializer(new InitDb());

            //DataBaseName = "DbTrade2";

            var configurationResourse = ConfigurationResourse1.Instance;
            var xdoc = configurationResourse.Get("Repository");
            if (xdoc == null)
            {
                SendExceptionMessage3(Code, "TradeStorage", "Configuration", "TradeStorage", new FileNotFoundException());
                return;
            }

            GSExceptions = Builder.
                Build2<IEntityRepository<IGSExceptionDb, IGSException>>(xdoc, "ExceptionDbRepository");
            if (GSExceptions == null)
            {
                throw new NullReferenceException(Code + ": GSExceptions Repository Init Failure");   
            }
            GSExceptions.Parent = this;

            Accounts = Builder.Build2<IAccountRepository32>(xdoc, "AccountRepository33");
            if (Accounts == null)
            {
                throw new NullReferenceException(Code + ": Accounts Repository Init Failure");
            }
            Accounts.Parent = this;

            Tickers = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(xdoc, "TickerRepository33");
            if (Tickers == null)
            {
                throw new NullReferenceException(Code + ": Tickers Repository Init Failure");
            }
            Tickers.Parent = this;

            Strategies = Builder.Build2<IEntityRepository<IStrategyDb, IStrategy>>(xdoc, "StrategyRepository33");
            if (Strategies == null)
            {
                throw new NullReferenceException(Code + ": Strategies Repository Init Failure");
            }
            Strategies.Parent = this;

            Orders = Builder.Build2<IEntityRepository<IOrderDb, IOrder3>>(xdoc, "OrderRepository33");
            if (Orders == null)
            {
                throw new NullReferenceException(Code + ": Orders Repository Init Failure");
            }
            Orders.Parent = this;

            Trades = Builder.Build2<IEntityRepository<ITradeDb, ITrade3>>(xdoc, "TradeRepository33");
            if (Trades == null)
            {
                throw new NullReferenceException(Code + ": Trades Repository Init Failure");
            }
            Trades.Parent = this;

            Positions = Builder.Build2<IEntityRepository<IPositionDb, IPosition2>>(xdoc, "PositionRepository33");
            if (Positions == null)
            {
                throw new NullReferenceException(Code + ": Positions Repository Init Failure");
            }
            Positions.Parent = this;

            PositionTotals = Builder.
                Build2<IEntityRepository<IPositionDb, IPosition2>>(xdoc, "PositionTotalRepository33");
            if (PositionTotals == null)
            {
                throw new NullReferenceException(Code + ": PositionTotals Repository Init Failure");
            }
            PositionTotals.Parent = this;

            Deals = Builder.Build2<IEntityRepository<IDealDb, IDeal>>(xdoc, "DealRepository33");
            if (Deals == null)
            {
                throw new NullReferenceException(Code + ": Deals Repository Init Failure");
            }
            Deals.Parent = this;
        }
    }
}
