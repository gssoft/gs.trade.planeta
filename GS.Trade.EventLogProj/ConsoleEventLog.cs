using System;
using GS.Trade.Interfaces;

namespace GS.Trade.EventLog
{
    public class ConsoleEventLog : IEventLog
    {
        public void AddItem(EnumEventLog result, string operation, string description)
        {
            Console.WriteLine("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
        }
        public void AddItem(EnumEventLog result, EnumEventLogSubject subject, 
                                string source, string operation, string description, string objects)
        {
            Console.WriteLine("{0:G} {1} {2} {3} {4} {5} {6}", 
                DateTime.Now, result, subject, source, operation, description, objects);
        }
    }
}
