using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.Exceptions
{
    public interface IExceptionRepository
    {
        bool IsEnabled { get; }

        void Init(IEventLog evl);

        IGSExceptionDb GetByKey(string key);

        bool Add(IGSException p);
        bool AddNew(IGSException p);
        IGSException AddOrGet(IGSException p);
    }
}
