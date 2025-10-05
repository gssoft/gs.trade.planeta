using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;
using GS.EventLog.DataBase.Repository;

namespace EventLog3
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<EvlContext>(new GS.EventLog.DataBase.Dal.Initializer());

            var r = new EvlRepository();

            var evls = r.GetEventLogs();
            foreach (var evl in evls)
                Console.WriteLine(evl.ToString());

            GS.EventLog.DataBase.Model.DbEventLog ev = evls.FirstOrDefault();

               XmlSerializer ser = new XmlSerializer(typeof(DbEventLog));
        //    TextWriter text = new StreamWriter(@"C:\mySer.xml");
        //    ser.Serialize(text, ev);
        //    text.Close();

            Console.ReadLine();


            var evlis = r.GetItems();
            foreach (var evli in evlis)
                Console.WriteLine(evli.ToString());

            if( ev != null)
                Console.WriteLine("Count={0}", r.EventLogItemsCount(ev));

            Console.ReadLine();

        }
    }
}
