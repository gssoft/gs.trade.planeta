using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using GS.ConsoleAS;
using GS.EventLog;
using GS.EventLog.DataBase1.Dal;
using GS.EventLog.Dto;

namespace GS.Web.Api.Service01.Controllers
{
    public class EventLogItemsAsyncController : ApiController
    {
       // private EvlContext1 db = new EvlContext1();

        // GET: api/DbEventLogItems
        public IEnumerable<EventLogItemDto> GetEventLogItems()
        {
            //var evlis =  db.EventLogItems.ToList();
            //var dtos = evlis.Select(i => db.CreateEventLogItemDto(i)).ToList();
            //return dtos;
            return null;
        }

        // POST: api/DbEventLogs
        //[ResponseType(typeof(EventLogItemDto))]
        //public IHttpActionResult PostEventLogItem(EventLogItemDto dto)
        //{
        //    db.Add(dto);
        //    Console.WriteLine("PostItem: " + dto.ToString());
        //    // return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
        //    return Ok();
        //}

        // POST: api/DbEventLogItemsAsync
        //[ResponseType(typeof(DbEventLogItem))]
        public async Task<IHttpActionResult> PostEventLogItem(EventLogItemDto dto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //db.EventLogItems.Add(dbEventLogItem);
            //await db.SaveChangesAsync();
            //return CreatedAtRoute("DefaultApi", new { id = dbEventLogItem.EventLogItemID }, dbEventLogItem);

            // DbEventLog2.Instance.ReceiverDto.EnQueue(dto);
            ConsoleSync.WriteLineT(dto.ToString());

            //Console.WriteLine("PostItem: " + dto.ToString());
            // return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
            return Ok();
        }
    }
}
