using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;

namespace Test_CaWorkTask_01.Task
{
    public class UnitOfWork
    {
        public string ProccessKey { get; }
        public string Category { get; }
        public string Entity { get; }

        public string Key => string.Join(".", ProccessKey, Category, Entity);

        public Action<IEventArgs> Action { get; private set; }
        public IEventArgs EventArgs { get; private set; }

        public UnitOfWork(string proccessKey, string category, string entity,
            Action<IEventArgs> action, IEventArgs arg)
        {
            ProccessKey = proccessKey;
            Category = category;
            Entity = entity;

            Action = action;
            EventArgs = arg;
        }
    }
}
