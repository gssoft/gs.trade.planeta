using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase.Model;
using GS.Interfaces;

namespace WebEventLogClient
{
    public class WebEventLog : IEventLog
    {
        public string EventLogKey { get; set; }
        public string BaseAddress { get; set; }
        public string RequestHeader { get; set; }
        public string ApiEventLogs { get; set; }
        public string ApiEventLogItems { get; set; }

        private readonly HttpClient _client;

        private int _eventLogId;

        public WebEventLog()
        {
            EventLogKey = "GS.Trade.EventLog";
            BaseAddress = "http://127.0.0.1/";
            RequestHeader = "application/xml";

            ApiEventLogs = "api/eventlogs/";
            ApiEventLogItems = "api/eventlogitems/";
            
            _eventLogId = 0;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseAddress);

            // Add an Accept header for JSON format.
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(RequestHeader));
        }

        public void Init()
        {
            HttpResponseMessage response = _client.GetAsync(ApiEventLogs).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var eventlogs = response.Content.ReadAsAsync<IEnumerable<DbEventLog>>().Result;
                foreach (var e in eventlogs)
                {
                    Console.WriteLine("{0}", e);
                    if (e.Name == EventLogKey)
                        _eventLogId = e.EventLogID;
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            if (_eventLogId != 0) return;

            var evl = new DbEventLog { Alias = EventLogKey, Code = EventLogKey, Name = EventLogKey, Description = "Descript" };
            Uri evlUri = null;

            response = _client.PostAsJsonAsync(ApiEventLogs, evl).Result;
            if (response.IsSuccessStatusCode)
            {
                evlUri = response.Headers.Location;
                Console.WriteLine("Insert(Add) Success EventLog Uri= {0}", evlUri.ToString());
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

        }

        public void AddItem(EvlResult result, string operation, string description)
        {
            throw new NotImplementedException();
        }

        public void AddItem(EvlResult result, EvlSubject subject, string source, string operation, string description, string objects)
        {
           // throw new NotImplementedException();
        }

        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
                            string objects)
        {
            throw new NotImplementedException();
        }

        public void ClearSomeData(int count)
        {
           // throw new NotImplementedException();
        }
    }
}
