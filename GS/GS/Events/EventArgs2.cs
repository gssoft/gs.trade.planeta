using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers3;
using GS.Extension;

namespace GS.Events
{
    public interface IProcessingAction<TArg>
    {
        Action<TArg> ProcessingAction { get; set; }
    }

    public interface IEventArgs1 : IEventArgs, IProcessingAction<IEventArgs1>
    {
        string Process { get; }
    }

    public class EventArgs1 : System.EventArgs, IEventArgs1
    {
        private string _process;
        private string _category;
        private string _entity;
        private string _operation;

        public string Process
        {
            get => _process.HasValue() ? _process : "Default";
            set => _process = value.TrimUpper();
        }
        public string Category
        {
            get => _category.HasValue() ? _category : "Default";
            set => _category = value.TrimUpper();
        }
        public string Entity
        {
            get => _entity.HasValue() ? _entity : "Default";
            set => _entity = value.TrimUpper();
        }
        public string Operation
        {
            get => _operation.HasValue() ? _operation : "Default";
            set => _operation = value.TrimUpper();
        }
        public Type Type { get; set; }
        public object Object { get; set; }
        public object Sender { get; set; }
        public string Message { get; set; }
        public bool IsHighPriority { get; set; }
        public Action<IEventArgs1> ProcessingAction { get; set; }
        
        //public string OperationKey => Key.WithRight("." + Operation);
        public string OperationKey => string.Join(".", Key, Operation);

        public string Key => string.Join(".", Process, Category, Entity);

        public override string ToString()
        {
            return $"[Process:={Process}; Category:={Category}; Entity:{Entity};" +
                   $" Operation={Operation}; Message: {Message};" +
                   $" Key:{Key}; OperationKey:{OperationKey};\r\nObject:\r\n{Object}]";
        }
    }
}
