using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers3;
using GS.Extension;

namespace GS.Events
{
    public interface IEventArgs : IHaveStringKey
    {
        string Category { get;}
        string Entity { get;  }
        string Operation { get; }
        object Object { get;  }
        Type Type { get; }
        string Message { get; }
        object Sender { get; }
        bool IsHighPriority { get; }
        string OperationKey { get; }
    }

    public class EventArgs : System.EventArgs, IEventArgs
    {
        private string _category;
        private string _entity;
        private string _operation;

        public string Category
        {
            get => _category;
            set => _category = value.TrimUpper();
        }

        public string Entity
        {
            get => _entity;
            set => _entity = value.TrimUpper();
        }

        public string Operation
        {
            get => _operation;
            set => _operation = value.TrimUpper();
        }

        public Type Type { get; set; }
        public object Object { get; set; }
        public object Sender { get; set; }
        public string Message { get; set; }
        public bool IsHighPriority { get; set; }

        public string OperationKey => Key.WithRight("." + Operation);

        public string Key => Category.WithRight("." + Entity);

        public override string ToString()
        {
            return
                $"[Category:={Category}; Entity:{Entity}; Operation={Operation}; Message: {Message}; Key:{Key}; OperationKey:{OperationKey};\r\nObject:\r\n{Object}]";
        }
    }
}
