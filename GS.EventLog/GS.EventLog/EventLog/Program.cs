using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.EventLog;
using GS.EventLog.DataBase;
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Repository;
using GS.Interfaces;
//using EventLogItem = GS.EventLog.DataBase.Model.DbEventLogItem;

namespace EventLog
{
    public delegate int AddDb(GS.EventLog.DataBase.Model.DbEventLogItem evli);
    class Program
    {
        private static GS.EventLog.DataBase.Model.DbEventLog _dbEvl;
        private static EvlRepository _repo;
        private static AddDb _adder;

        static void Main(string[] args)
        {
            Database.SetInitializer<EvlContext>(new GS.EventLog.DataBase.Dal.Initializer());

            _adder = AddToDb;

            _repo = new EvlRepository();
            _dbEvl = new GS.EventLog.DataBase.Model.DbEventLog 
                {Alias = "DbEventLog", Code = "Evl", Name = "DbEvl", Description = "DataBase EventLog"};
            //_repo.Add(_dbEvl);
           

            var evl = new MemEventLog();
           evl.EventLogItemEvent += NewItem;

            for (var i = 0; i < 100; i++)
            {
                evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Test", "EventLog", "AddItem",
                            "Try to AddItem to MemEventLog", i.ToString());

                //var evli = new GS.EventLog.DataBase.Model.DbEventLogItem
                //{
                //    DT = DateTime.Now,
                //    ResultCode = EvlResult.SUCCESS ,
                //    Subject = EvlSubject.PROGRAMMING,
                //    Source = i.ToString(),
                //    Entity = i.ToString(),
                //    Operation = i.ToString(),
                //    Object = i.ToString(),
                //  //  EventLog = _dbEvl,
                //    EventLogID = 243
                //};
                //int add = _repo.Add(evli);


                // Thread.Sleep(300);
            }
            // for (var i = 230; i < 330; i++)
           //     evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Test", "EventLog", "AddItem", "Try to AddItem to MemEventLog", i.ToString());
            
            foreach(var i in evl.Items)
                Console.WriteLine(i.ToString());

            Console.WriteLine("Count=" + evl.Count);
            Console.ReadLine();

            //int added = _repo.Add(_dbEvl);

            //Console.WriteLine("Add Count=" + added);
            //Console.ReadLine();

            var ii = _repo.GetItems(Filtr1);
            Console.WriteLine("DbCount=" + ii.Count().ToString(CultureInfo.InvariantCulture));
            foreach(var i in ii)
                Console.WriteLine(i.ToString());
            Console.ReadLine();
        }

        public static bool Filtr1(GS.EventLog.DataBase.Model.DbEventLogItem evli )
        {

            return evli.ResultCode == EvlResult.SUCCESS;
        }

        public static  void NewItem( object sender, EventArgs a)
        {
            var ev = (IEventLogEvent) a;
            var op = ev.Operation;
            var i = ev.EventLogItem;

            var evli = new GS.EventLog.DataBase.Model.DbEventLogItem
                {
                    DT = DateTime.Now,
                    ResultCode = i.ResultCode,
                    Subject = i.Subject,
                    Source = i.Source,
                    Entity = i.Entity,
                    Operation = i.Operation,
                    Object = i.Object,
                 //   EventLog = _dbEvl,
                    EventLogID = 767
                };

            //_repo.Add(evli);
            //_dbEvl.Items.Add(evli);
            IAsyncResult iar = _adder.BeginInvoke(evli, new AsyncCallback(AddComplete), _repo);


            Console.WriteLine(String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                                                i.DT, i.ResultCode, i.Subject, i.Source, i.Subject, i.Entity,i.Operation,i.Object));
        }

        static int AddToDb(GS.EventLog.DataBase.Model.DbEventLogItem evli)
        {
            return _repo.Add(evli);
        }

        private static void AddComplete(IAsyncResult iar)
        {
            AsyncResult ar = (AsyncResult) iar;
            var adder = (AddDb) ar.AsyncDelegate;
            Console.WriteLine("AsyncResult={0}", adder.EndInvoke(iar));
        }
    }
}
