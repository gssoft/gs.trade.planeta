using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Model;

namespace GS.Trade.DataBase.EFRepositary
{
    public class ExceptionEFRepository : EFRepository<string, Model.GSException>
    {
        public override string Key { get { return Code; } } // From Element1
        
        public ExceptionEFRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        public override Model.GSException GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }
    public class AccountEFRepository : EFRepository<string, Model.Account>
    {
        public AccountEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }
        public override Model.Account GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }
    public class TickerEFRepository : EFRepository<string, Model.Ticker>
    {
        public TickerEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }
        public override Model.Ticker GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }
    public class StrategyEFRepository : EFRepository<string, Model.Strategy>
    {
        public StrategyEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }

        public override Strategy GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }

    public class OrderEFRepository : EFRepository<string, Model.Order>
    {
        public OrderEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }

        public override Order GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }
    public class TradeEFRepository : EFRepository<string, Model.Trade> 
    {
        public TradeEFRepository(DbContext dbContext) 
            : base(dbContext)
        {
            
        }
        public override string Key
        {
            get { return Code; }
        }

        public override Model.Trade GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }

    public class PositionEFRepository : EFRepository<string, Model.Position>
    {
        public PositionEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }

        public override Position GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }
    public class TotalEFRepository : EFRepository<string, Model.Total>
    {
        public TotalEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }

        public override Total GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }

    public class DealEFRepository : EFRepository<string, Model.Deal>
    {
        public DealEFRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
        public override string Key
        {
            get { return Code; }
        }

        public override Deal GetByKey(string key)
        {
            return DbSet.FirstOrDefault(e => e.Key == key);
        }
    }

}
