using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;

namespace Evl1
{
    class Program
    {
        private static  IEventLogs _evl;
        private static volatile bool _working;
 
        static void Main(string[] args)
        {
            _evl = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            _evl.Init();

            //Test1(500);

            Test2(50,300);

            Console.ReadLine();
        }

        private static void Test1(int msec)
        {
            for (var i = 0; i < 500; i++)
            {
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
                    i.ToString(), i.ToString(), i.ToString(), i.ToString(), i.ToString());

                Thread.Sleep(msec);
            }
        }
        private static void Test2(int msec1, int msec2)
        {
            _working = true;
            Task.Factory.StartNew(() =>
            {
                while (_working)
                {
                    _evl.DeQueueProcess();
                    Thread.Sleep(msec2);
                }
                Console.WriteLine("DeQueueProcess Task is Completed");

            });
            for (var i = 0; i < 100; i++)
            {
                _evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
                    i.ToString(), i.ToString(), i.ToString(), i.ToString(), i.ToString());

                Thread.Sleep(msec1);
            }
            Console.WriteLine("The End");
            Console.ReadLine();
            _working = false;
            Console.WriteLine("The Full End");

        }
    }
}
