using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using GS.EventLog.DataBase.Dal;
using GS.EventLog.DataBase.Model;

namespace WebEventLog_03.Controllers
{
    public class EventLogsController : EntitySetController<DbEventLog, int>
    {
        private readonly EvlContext _db = new EvlContext("EventLog");

        [Queryable]
        public override IQueryable<DbEventLog> Get()
        {
            return _db.EventLogs;
        }
    }
}
