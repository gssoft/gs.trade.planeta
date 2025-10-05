using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using GS.EventLog.Dto;
using GS.Web.Api.Service01.Dto;

namespace GS.Web.Api.Service01.Controllers
{
    public class EventLogsController : ApiController
    {
       public IEnumerable<EventLogDto> GetEventLogs()
        {

            Console.WriteLine("GetEventLog");
            //var evls = db.EventLogs.ToList();
            //var dtos = evls.Select(e => db.CreateEventLogDto(e)).ToList();

            //var evls = db.EventLogs.Select(e => db.CreateEventLogDto(e)).ToList();
            return null;
        }

        // POST: api/DbEventLogs
        [ResponseType(typeof(EventLogDto))]
        public IHttpActionResult PostEventLog(EventLogDto dto)
        {
            //var dbevl =  db.Register( "App" , dto);
            //dto.ID = dbevl.EventLogID;

            Console.WriteLine("PostEvl: " + dto.ToString());

            // return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
            return Ok();
        }
    }
}
