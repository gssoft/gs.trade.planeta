using System;
using GS.Interfaces;

namespace GS.EventLog
{
    public class ConsoleEventLog : IEventLog
    {
        public void AddItem(EvlResult result, string operation, string description)
        {
            Console.WriteLine("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
        }
        public void AddItem(EvlResult result, EvlSubject subject, 
                                string source, string operation, string description, string objects)
        {
            Console.WriteLine("{0:G} {1} {2} {3} {4} {5} {6}", 
                DateTime.Now, result, subject, source, operation, description, objects);
        }

        public void ClearSomeData(int count)
        {
        }
    }
}
