using System;

namespace TestAssembly
{
    public class ConsoleMessage
    {
        public void Message()
        {
            Console.WriteLine($"{DateTime.Today.TimeOfDay.ToString("g")}, Type: {GetType().FullName}, Method: Message()" );
        }
    }
}
