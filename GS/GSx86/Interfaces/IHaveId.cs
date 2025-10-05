using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Interfaces
{
    public interface IHaveId<TId>
    {
        TId Id { get; set; }
    }

    public interface IHaveID<TId>
    {
        TId ID { get; set; }
    }
}
