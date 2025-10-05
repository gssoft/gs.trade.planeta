using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Trades;
using GS.Trade.Trades.Orders3;

namespace GS.Trade.Strategies
{
    public class DefaultStrategy : Strategy
    {
        public override IBars Bars { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override int MaxBarsBack { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public  void Init()
        {
            base.Init();
        }

        public override void Main()
        {
            return;
            // throw new NotImplementedException();
        }
    }
}
