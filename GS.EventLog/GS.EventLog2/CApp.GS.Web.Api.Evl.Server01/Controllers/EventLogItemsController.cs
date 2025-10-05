using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using  GS.EventLog;
using  GS.EventLog.DataBase1.Dal;
using  GS.EventLog.DataBase1.Model;
using  GS.EventLog.Dto;

namespace CApp.GS.Web.Api.Evl.Server01.Controllers
{
    public class EventLogItemsController : ApiController
    {
        private EvlContext1 db = new EvlContext1();

        // GET: api/DbEventLogItems
        public IEnumerable<EventLogItemDto> GetEventLogItems()
        {
            var evlis =  db.EventLogItems.ToList();
            var dtos = evlis.Select(i => db.CreateEventLogItemDto(i)).ToList();
            return dtos;
        }

        // POST: api/DbEventLogs
        //[ResponseType(typeof(EventLogItemDto))]
        public IHttpActionResult PostEventLogItem(EventLogItemDto dto)
        {
            //var dbevl = db.Register("App", dto);
            //dto.ID = dbevl.EventLogID;
           
            // db.Add(dto);

            DbEventLog2.Instance.ReceiverDto.EnQueue(dto);

            // Console.WriteLine("PostItem: " + dto.ToString());

            // return CreatedAtRoute("DefaultApi", new { id = dbevl.EventLogID }, dto);
            return Ok();
        }
    }
}
