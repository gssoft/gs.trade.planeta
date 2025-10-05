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
    public class DbStorage32 : TradeBaseStorage
    {
        public string DataBaseName { get; set; }

        public DbStorage32()
        {
            Database.SetInitializer(new InitDb());

            DataBaseName = "DbTrade2";

            //GSExceptions = new ExceptionDbRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "ExceptionDbRepository",
            //    Name = "ExceptionDb Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = false
            //};

            GSExceptions = Builder.
                Build2<IEntityRepository<IGSExceptionDb, IGSException>>(@"Init\TradeRepositories.xml", "ExceptionDbRepository");
            GSExceptions.Parent = this;
            
            //Accounts = new AccountRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "AccountRepository",
            //    Name = "Account Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = false
            //};

            Accounts = Builder.Build2<IAccountRepository32>(@"Init\TradeRepositories.xml", "AccountRepository32");
            Accounts.Parent = this;

            //Tickers = new TickerRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "TickerRepository",
            //    Name = "Tickers Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = false
            //};

            Tickers = Builder.Build2<IEntityRepository<ITickerDb, ITicker>>(@"Init\TradeRepositories.xml", "TickerRepository32");
            Tickers.Parent = this;

            //Strategies = new StrategyRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "StrategyRepository",
            //    Name = "Strategies Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = false
            //};

            Strategies = Builder.Build2<IEntityRepository<IStrategyDb, IStrategy>>(@"Init\TradeRepositories.xml", "StrategyRepository32");
            Strategies.Parent = this;
            //Orders = new OrderRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "OrderRepository",
            //    Name = "Orders Repository",
            //    Parent = this,
            //    UIEnabled = false,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = true
            //};

            Orders = Builder.Build2<IEntityRepository<IOrderDb, IOrder3>>(@"Init\TradeRepositories.xml", "OrderRepository32");
            Orders.Parent = this;
            //Trades = new TradeRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "TradeRepository",
            //    Name = "Trade Repository",
            //    Parent = this,
            //    UIEnabled = false,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = true
            //};

            Trades = Builder.Build2<IEntityRepository<ITradeDb, ITrade3>>(@"Init\TradeRepositories.xml", "TradeRepository32");
            Trades.Parent = this;
            //Positions = new PositionRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "PositionRepository",
            //    Name = "Positions Current Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = true
            //};

            Positions = Builder.Build2<IEntityRepository<IPositionDb, IPosition2>>(@"Init\TradeRepositories.xml", "PositionRepository32");
            Positions.Parent = this;
            //PositionTotals = new PositionTotalRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "PositionTotalRepository",
            //    Name = "PositionTotal Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = true
            //};

            PositionTotals = Builder.
                Build2<IEntityRepository<IPositionDb, IPosition2>>(@"Init\TradeRepositories.xml", "PositionTotalRepository32");
            PositionTotals.Parent = this;
            //Deals = new DealRepository32
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "DealRepository",
            //    Name = "Deal Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true,
            //    IsEvlEnabled = true,
            //    IsQueueEnabled = true
            //};

            Deals = Builder.Build2<IEntityRepository<IDealDb, IDeal>>(@"Init\TradeRepositories.xml", "DealRepository32");
            Deals.Parent = this;
        }
    }
}
