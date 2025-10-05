using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Trades.UI.Model
{
    public class PositionTotalNpc2 : PositionNpc2
    {
        public PositionTotalNpc2()
        {
            Status = PosStatusEnum.Closed;
        }
        public PositionTotalNpc2(IPosition2 ip) : base(ip)
        {
            Status = PosStatusEnum.Closed;
        }
        public override decimal PnL1 => (Price2 - Price1) * Quantity;
    }
}
