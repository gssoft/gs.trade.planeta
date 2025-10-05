using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Repository;

namespace Model
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<EvlContext>(new GS.EventLog.DataBase.Dal.Initializer());

            var r = new EvlRepository();
            var evl1 = new DbEventLog
                {
                    Alias = "MyLog",
                    Code = "TradeLog",
                    Name = "Frosya",
                    Description = "EventLog fo my Trading"
                };
            var evl2 = r.AddNew(evl1);

            var evls = r.GetEventLogs();
            foreach (var evl in evls)
                Console.WriteLine(evl.ToString());

            GS.EventLog.DataBase.Model.DbEventLog ev = evls.FirstOrDefault();
            try
            {
            XmlSerializer ser2 = new XmlSerializer(typeof(DbEventLogItem));

            XmlSerializer ser = new XmlSerializer(typeof(DbEventLog));
            TextWriter text = new StreamWriter(@"EventLog.xml");
            ser.Serialize(text, ev);
            text.Close();
            }
        catch(Exception e)
    {
        Console.WriteLine("Serialization is FaultEventlog={0}\n{1}", ev.ToString(), e.Message);
    }
    if( ev != null)
                Console.WriteLine("Eventlog={0} is OK Serialized", ev.ToString() );
            Console.ReadLine();


           // var evl3 = evls.FirstOrDefault(i => i.ItemsCount > 0);
            DbEventLogItem evli = null;
           // DbEventLog evlog;
            foreach (var evlis in evls.Select(evl => r.GetItems(evl)))
            {
              //  evlog = evls;
                evli = evlis.FirstOrDefault();
                if (evli != null)
                    break;
            }


        //    var ee = evlog;

                if (evli != null)
                {
                    //var e = new DbEventLogItem
                    //    {
                    //        EventLogID = evli.EventLogID,
                    //        DT = evli.DT,
                    //        ResultCode = evli.ResultCode,
                    //        Subject = evli.Subject,
                    //        Source = evli.Source,
                    //        Entity = evli.Entity,
                    //        Operation = evli.Operation,
                    //        Description = evli.Description,
                    //        Object = evli.Operation
                    //    };
                    try
                    {

                        XmlSerializer ser3 = new XmlSerializer(typeof (DbEventLogItem));
                        StringWriter text2 = new StringWriter();
                        ser3.Serialize(text2, evli);
                     //   string xml = text2.ToString();

                        Console.WriteLine("EventlogItem={0} is OK Serialized", evli.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Serialization is Fault .Eventlog={0}\n{1}", evli.ToString(), e.Message);
                        throw e;
                    }
                }
            
            Console.ReadLine();

        }
    }
}
