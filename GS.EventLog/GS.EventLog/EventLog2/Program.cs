using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog;
using GS.EventLog.DataBase.Dal;


namespace EventLog2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database.SetInitializer<EvlContext>(new GS.EventLog.DataBase.Dal.Initializer());

            //var evl = new DbEventLog
            //    {
            //        Async = true,
            //        //Debug = true,
            //        Alias = "EventLog_02",
            //        Code = "TestEventLog_02",
            //        Name = "EventLogName",
            //        Description = "EventLog for Testing"
            //    };
            //evl.Init();

            //Console.WriteLine("ItemsCount={0}", evl.Count().ToString());

            //for (var i = 0; i < 100; i++)
            //{
            //    var r =  i%2 == 0 ? EvlResult.SUCCESS : EvlResult.FATAL; 

            //    evl.AddItem( r, EvlSubject.PROGRAMMING, "EventLogTest", "EventLogItem", "Add", "Try to Add Async EventLogIten", "");
            //}
            //Console.WriteLine("Operation Complete");
            //Console.ReadLine();
            //Console.WriteLine("ItemsCount={0}", evl.Count().ToString());
            //Console.ReadLine();
        }
    }
}
