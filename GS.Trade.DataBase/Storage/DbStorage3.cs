using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Init;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class DbStorage3 : TradeBaseStorage
    {
        public string DataBaseName { get; set; }

        public DbStorage3()
        {
            Database.SetInitializer(new InitDb());

            DataBaseName = "DbTrade2";

            GSExceptions = new ExceptionDbRepository
            {
                DataBaseName = DataBaseName,
                Code = "ExceptionDbRepository",
                Name = "ExceptionDb Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = true
            };

            Accounts = new AccountRepository32
            {
                DataBaseName = DataBaseName,
                Code = "AccountRepository",
                Name = "Account Repository",
                Parent = this,
                IsUIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = false
            };

            Tickers = new TickerRepository3
            {
                DataBaseName = DataBaseName,
                Code = "TickerRepository",
                Name = "Tickers Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = false
            };

            Strategies = new StrategyRepository3
            {
                DataBaseName = DataBaseName,
                Code = "StrategyRepository",
                Name = "Strategies Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = true
            };

            Orders = new OrderRepository3
            {
                DataBaseName = DataBaseName,
                Code = "OrderRepository",
                Name = "Orders Repository",
                Parent = this,
                UIEnabled = false,
                IsEnabled = true,
                IsQueueEnabled = true
            };
            Trades = new TradeRepository3
            {
                DataBaseName = DataBaseName,
                Code = "TradeRepository",
                Name = "Trade Repository",
                Parent = this,
                UIEnabled = false,
                IsEnabled = true,
                IsQueueEnabled = true
            };
            Positions = new PositionRepository3
            {
                DataBaseName = DataBaseName,
                Code = "PositionRepository",
                Name = "Positions Current Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = true
            };

            PositionTotals = new PositionTotalRepository3
            {
                DataBaseName = DataBaseName,
                Code = "PositionTotalRepository",
                Name = "PositionTotal Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = true
            };
            Deals = new DealRepository3
            {
                DataBaseName = DataBaseName,
                Code = "DealRepository",
                Name = "Deal Repository",
                Parent = this,
                UIEnabled = true,
                IsEnabled = true,
                IsQueueEnabled = true
            };
        }
    }
}
