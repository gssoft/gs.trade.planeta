using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Triggers;

namespace GS.Trade.Strategies.TradeMode
{
    public class TradeMode
    {
        public Trigger ChangeModeCond { get; set; }
        public Func<bool> ChangeModeFunc { get; set; }

        public Func<bool> TakeLong { get; set; }
        public Func<bool> TakeShort { get; set; }

        public Func<int> TakeEntry { get; set; }

        public TradeMode ChangeMode()
        {
            return ChangeModeCond.SetValue(ChangeModeFunc()) ? this : null;
        }

        public void Clear()
        {
            ChangeModeCond.Reset();
        }

    }
}
