using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.DataBase.Model
{
    partial class Ticker
    {
        public void CopyFrom(Ticker t)
        {
             Name = t.Name;
             Alias = t.Alias;

             TradeBoard = t.TradeBoard;
             Code = t.Code;

             Decimals = t.Decimals;
             MinMove = t.MinMove;
             
             BaseContract = t.BaseContract;

             Margin = t.Margin;
             PriceLimit = t.PriceLimit;

             LaunchDate = t.LaunchDate;
             ExpireDate = t.ExpireDate;
        }
    }
}
