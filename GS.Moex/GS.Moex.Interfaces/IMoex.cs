using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Moex.Interfaces
{
    public interface IItemHolder<T>
    {
        List<T> Items { get; }
        void Clear();
    }
    public interface IHaveInit
    {
        void Init();
    }
}
