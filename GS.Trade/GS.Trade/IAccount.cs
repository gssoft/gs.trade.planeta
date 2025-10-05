using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Containers5;
using GS.Elements;
using GS.Interfaces;

namespace GS.Trade
{
    public interface IAccounts : IElement1<string>
    {
        IAccount GetAccount(string key);
        IEnumerable<IAccount> GetAccounts { get; }
        IAccount this[string index] { get; }
        IAccount Register(IAccount acc);
        void Load();

        IAccount CreateInstance(IAccountBase ab);
        IAccount CreateInstanceSimple(string key);
    }
 
    public interface IAccountBase : IHaveKey<string>, IHaveId<int>
    {
        string Code { get; set; }
        string Name { get; set; }
        string Alias { get; set; }
        string TradePlace { get; set; }

        decimal Balance { get; set; }
    }

    public interface IAccount : IAccountBase //, IElement1<string>
    {
        IAccounts Accounts { get; set; }
        IElement1<string> Parent { get; set; }

        decimal Limit { get; }
        decimal DailyProfitLimit { get;}
        decimal DailyLossLimit { get; }
    }

    public interface IAccountDb : IAccountBase
    {
    }
}
