using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public interface IDeals
    {
        bool AddNew(IPosition2 p);
        IEnumerable<IPosition2> Items { get; }
    }
}
