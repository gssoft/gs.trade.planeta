using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;

namespace GS.EventLog.DataBase.Repozitary
{
    public class EvlRepozitary
    {
        private EvlContext _db;

        //public EvlRepozitary()
        //{
        //    //_db = new EvlContext();
        //}

        public IEnumerable<Model.EventLogItem> GetItems()
        {
            using (var db = new EvlContext())
            {
                return db.EventLogItems.ToList();
            }
        }
        public IEnumerable<Model.EventLogItem> GetItems(Func<Model.EventLogItem, bool> func)
        {
            var ll = new List<Model.EventLogItem>();
            using (var db = new EvlContext())
            {
               // var l = db.EventLogItems.Where(func).ToList();
               // ll.AddRange(l.Where(func));
                return db.EventLogItems.Where(func).ToList();
            }
           // return ll;
        }

        public IEnumerable<Model.EventLog> GetEventLogs()
        {
            using (var db = new EvlContext())
            {
                return db.EventLogs.ToList();
            }
        }

        public int Add(Model.EventLogItem it)
        {
            int ret;
            using (var db = new EvlContext())
            {
                db.EventLogItems.Add(it);
                ret = db.SaveChanges();
            }
            return ret;
        }
        public int Add(Model.EventLog evl)
        {
            int ret;
            using (var db = new EvlContext())
            {
                db.EventLogs.Add(evl);
                ret = db.SaveChanges();
            }
            return ret;
        }
    }
}
