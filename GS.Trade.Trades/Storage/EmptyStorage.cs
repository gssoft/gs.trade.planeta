using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;

namespace GS.Trade.Trades.Storage
{
    public class EmptyStorage : Element1<string>
    {
        public override string Key => "I'm EmptyStorage";
    }
}
