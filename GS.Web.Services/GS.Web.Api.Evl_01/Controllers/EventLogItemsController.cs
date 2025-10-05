using System.Collections.Generic;
using System.Web.Http;
using GS.ConsoleAS;
using GS.EventLog.Dto;

namespace GS.Web.Api.Evl_01.Controllers
{
    public class EventLogItemsController : ApiController
    {
        //private EvlContext1 db = new EvlContext1();

        // GET: api/DbEventLogItems
        public IEnumerable<string> GetEventLogItems()
        {
            //var evlis =  db.EventLogItems.ToList();
            //var dtos = evlis.Select(i => db.CreateEventLogItemDto(i)).ToList();
            // return dtos;
            return new [] {"1","2"};
        }

        // POST: api/DbEventLogs
        //[ResponseType(typeof(EventLogItemDto))]
        public IHttpActionResult PostEventLogItem(EventLogItemDto dto)
        {
            //var dbevl = db.Register("App", dto);
            //dto.ID = dbevl.EventLogID;
           
            // db.Add(dto);

            // DbEventLog2.Instance.ReceiverDto.EnQueue(dto);
            ConsoleSync.WriteLineT(dto.ToString());

            // Console.WriteLine("PostItem: " + dto.ToString());

            // return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
            return Ok();
        }
    }
}
