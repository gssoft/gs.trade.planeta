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
            get { return _category; }
            set { _category = value.TrimUpper(); }
        }

        public string Entity
        {
            get { return _entity; }
            set { _entity = value.TrimUpper(); }
        }

        public string Operation
        {
            get { return _operation; }
            set { _operation = value.TrimUpper(); }
        }

        public Type Type { get; set; }
        public Object Object { get; set; }
        public Object Sender { get; set; }
        public string Message { get; set; }
        public bool IsHighPriority { get; set; }

        public string OperationKey
        {
            get
            {
                return Key.WithRight("." + Operation);
            }
        }

        public string Key
        {
            get
            {
                return Category.WithRight("." + Entity);
            }
        }
        public override string ToString()
        {
            return String.Format("[Catagory:={0}; Entity:{1}; Operation={2}; Message: {3}; Key:{4}; OperationKey:{5};\r\nObject:\r\n{6}]",
                                        Category, Entity, Operation, Message, Key, OperationKey, Object);
        }
    }
}
