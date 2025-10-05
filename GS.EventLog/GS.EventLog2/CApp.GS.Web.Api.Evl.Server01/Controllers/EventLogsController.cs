using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using  GS.EventLog;
using  GS.EventLog.DataBase1.Dal;
using  GS.EventLog.Dto;
using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;

namespace CApp.GS.Web.Api.Evl.Server01.Controllers
{
    public class EventLogsController : ApiController
    {
        private EvlContext1 db = new EvlContext1();

        public IEnumerable<EventLogDto> GetEventLogs()
        {
            var evls = db.EventLogs.ToList();
            var dtos = evls.Select(e => db.CreateEventLogDto(e)).ToList();

            //var evls = db.EventLogs.Select(e => db.CreateEventLogDto(e)).ToList();
            return dtos;
        }

        // POST: api/DbEventLogs
        [ResponseType(typeof(EventLogDto))]
        public IHttpActionResult PostEventLog(EventLogDto dto)
        {
            var dbevl =  db.Register( "App" , dto);
            dto.ID = dbevl.EventLogID;

            Console.WriteLine("PostEvl: " + dto.ToString());

            return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
        }
    }
}
