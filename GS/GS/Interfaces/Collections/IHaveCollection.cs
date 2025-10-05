using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;

namespace GS.Interfaces.Collections
{
    public interface IHaveCollection<in TKey, TItem>
         where TItem : class, IHaveKey<TKey>
    {
        IEnumerable<TItem> Items { get; }
        TItem Register(TItem item);
        TItem GetByKey(TKey key);
    }
}
