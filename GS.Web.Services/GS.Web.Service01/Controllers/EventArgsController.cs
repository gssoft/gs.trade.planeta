using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GS.Compress;
using GS.ConsoleAS;
using GS.Events;
using GS.Serialization;
using EventArgs = System.EventArgs;

namespace GS.Web.Service01.Controllers
{
    public class EventArgsController : ApiController
    {
        [HttpGet]
        [ActionName("EventArgs")]
        public EventArgs Get()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ConsoleAsync.WriteLineT(m);

            var args = new EventArgs
            {
                Category = "Caregory",
                Entity = "Entity",
                IsHighPriority = true,
                Message = "Message",
                Process = "Process",
                Sender = GetType().FullName,
                Object = new DataTransferObj { Code = "Code", Name = "Name"},
                FullTypeName = typeof(DataTransferObj).FullName
            };
            ConsoleAsync.WriteLineT(args.ToString());
            return args;
        }
    }
    public class EventArgsBytesController : ApiController
    {
        [HttpGet]
        [ActionName("EventArgsBytes")]
        public byte[] GetEventArgsBytes()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ConsoleSync.WriteLineT(m);

            var args = new EventArgsBytes
            {
                Category = "Caregory",
                Entity = "Entity",
                IsHighPriority = true,
                Message = "Message",
                Process = "Process",
                Sender = GetType().FullName,
                Object = new DataDto {Code = "Code", Name = "Name"},
                FullTypeName = typeof (DataTransferObj).FullName
            };
            try
            {
                var bytes = BinarySerialization.SerializeToByteArray(args);
                ConsoleSync.WriteLineT(bytes.ToString());
                ConsoleSync.WriteLineT(bytes.Length.ToString());
                return bytes;
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(ex.ToString());
                ConsoleSync.WriteLineT(Environment.NewLine);
                ConsoleSync.WriteLineT(ex.Message);
            }
            return new byte[] {1, 2};
        }
    }
    public class EventArgsBytesCmprsController : ApiController
    {
        [HttpGet]
        [ActionName("EventArgsBytesCmprs")]
        public byte[] GetEventArgsBytesCmprs()
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ConsoleSync.WriteLineT(m);

            var args = new EventArgsBytes
            {
                Category = "Caregory",
                Entity = "Entity",
                IsHighPriority = true,
                Message = "Message",
                Process = "Process",
                Sender = GetType().FullName,
                Object = new DataDto { Code = "Code", Name = "Name" },
                FullTypeName = typeof(DataTransferObj).FullName
            };
            try
            {
                var bytes = BinarySerialization.SerializeToByteArray(args);
                var bytesZip = Compressor.Compress(bytes);
                ConsoleSync.WriteLineT(bytesZip.ToString());
                ConsoleSync.WriteLineT(bytesZip.Length.ToString());
                return bytesZip;
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(ex.ToString());
                ConsoleSync.WriteLineT(Environment.NewLine);
                ConsoleSync.WriteLineT(ex.Message);
            }
            return new byte[] { 1, 2 };
        }
    }
}
