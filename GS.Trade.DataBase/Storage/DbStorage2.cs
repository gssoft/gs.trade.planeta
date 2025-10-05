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
    public class DbStorage2 : TradeBaseStorage
    {
        public string DataBaseName { get; set; }

        public DbStorage2()
        {
            Database.SetInitializer(new InitDb());

            DataBaseName = "DbTrade2";

            //Accounts = new AccountRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "AccountRepository",
            //    Name = "Account Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //};

            //Tickers = new TickerRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "TickerRepository",
            //    Name = "Tickers Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //};

            //Strategies = new StrategyRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "StrategyRepository",
            //    Name = "Strategies Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //}; 

            //Orders = new OrderRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "OrderRepository",
            //    Name = "Orders Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //}; 
            //Trades = new TradeRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "TradeRepository",
            //    Name = "Trade Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //}; 
            //Positions = new PositionRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "PositionRepository",
            //    Name = "Positions Current Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //}; 

            //PositionTotals = new PositionTotalRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "PositionTotalRepository",
            //    Name = "Positions Total Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //};
            //Deals = new DealRepository
            //{
            //    DataBaseName = DataBaseName,
            //    Code = "DealRepository",
            //    Name = "Deal Repository",
            //    Parent = this,
            //    UIEnabled = true,
            //    IsEnabled = true
            //}; 
        }
    }
}
