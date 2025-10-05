using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.Dto;

namespace CApp.GS.Web.Api.Evl.Server02.Controllers
{
    public class EventLogsController : ApiController
    {
        private readonly EvlContext1 _db = new EvlContext1();

        public IEnumerable<EventLogDto> GetEventLogs()
        {
            var evls = _db.EventLogs.ToList();
            var dtos = evls.Select(e => _db.CreateEventLogDto(e)).ToList();

            //var evls = _db.EventLogs.Select(e => _db.CreateEventLogDto(e)).ToList();
            return dtos;
        }

        // POST: api/DbEventLogs
        [ResponseType(typeof(EventLogDto))]
        public IHttpActionResult PostEventLog(EventLogDto dto)
        {
            var dbevl =  _db.Register( "App" , dto);
            dto.ID = dbevl.EventLogID;

            Console.WriteLine("PostEvl: " + dto.ToString());

            return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
        }
    }
}
