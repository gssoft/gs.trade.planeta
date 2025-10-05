using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

using System.Net.Http;
using System.Net.Http.Headers;
using GS.EventLog.Dto;
using GS.Events;
using GS.Interfaces;

using GS.EventLog;
using GS.EventLog.DataBase1.Model;
using GS.EventLog.DataBase1;

// using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
// using EventLogItemDto = GS.EventLog.DataBase1.Model.EventLogItemDto;

namespace GS.EventLog
{
    // using DbEventLog = GS.EventLog.DataBase1.Model.DbEventLog;
    // using EventLogItemDto = GS.EventLog.DataBase1.Model.EventLogItemDto;
    public class WebEventLog1 : Evl, IEventLog
    {
        public string EventLogKey { get; set; }
        public string BaseAddress { get; set; }

        public string RequestHeader { get; set; }

        public string ApiEventLogs { get; set; }

        public string ApiEventLogItems { get; set; }

        //[XmlIgnore]
        //public DataBase.Model.DbEventLog WebDbEventLog { get; private set; }
        [XmlIgnore]
        public EventLogDto WebDbEventLog { get; private set; }

        private readonly HttpClient _client;

        private long _index = 0;

        public WebEventLog1()
        {
           // EventLogKey = "GS.Trade.EventLog";
            //  BaseAddress = "http://localhost:2598/";
            //  BaseAddress = "http://localhost/ApiEventLog/";
            // BaseAddress = "http://81.176.229.34/ApiEventLog/";
        //    RequestHeader = "application/xml";

       //     ApiEventLogs = "api/eventlogs/";
       //     ApiEventLogItems = "api/eventlogitems/";

          //  IsAsync = false;

           _client = new HttpClient();
            //_client.BaseAddress = new Uri(BaseAddress);

            //// Add an Accept header for JSON format.
            //_client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue(RequestHeader));
        }

        public IEventLog Primary { get { return this; } }

        public override void Init()
        {
            _client.BaseAddress = new Uri(BaseAddress);

            // Add an Accept header for JSON format.
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(RequestHeader));

           InitEventLog();

            //var evlog = new DbEventLog { Alias = EventLogKey, Code = EventLogKey, Name = EventLogKey, Description = "Descript" };
            //int addEventLog = AddEventLog(evlog);
            /*
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
            var ret = AddEventLog(evl);
             */
            /*
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
            */

        }
       
        private void InitEventLog()
        {
            if (WebDbEventLog != null)
                return;
            var evlog = new EventLogDto { Code = EventLogKey, Name = EventLogKey, Description = "Description" };
            var evl = AddEventLog(evlog);
            if (evl != null)
                WebDbEventLog = evl;
            else
            {
                IsEnabled = false;

                SendExceptionMessage3(FullCode, GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    evlog.ToString(),
                    new NullReferenceException("WebEventLog is Null"));
            }
            // throw new NullReferenceException("EventLog " + EventLogKey + " is Null");
        }

        private EventLogDto AddEventLog(EventLogDto evl)
        {
            //if (IsAsync)
            //    AddEventLogAsync(evl);
            //else
                WebDbEventLog = AddEventLogSync(evl);

            return WebDbEventLog;
        }

        private EventLogDto AddEventLogSync(EventLogDto evl)
        {
            try
            {
                using (HttpResponseMessage response = _client.PostAsJsonAsync(ApiEventLogs, evl).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var evlUri = response.Headers.Location;
                        var eventLog = response.Content.ReadAsAsync<EventLogDto>().Result;

                        //    Console.WriteLine("Insert(Add) Success EventLog Uri= {0}", evlUri.ToString());
                        return eventLog;
                    }
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                return null;
            }
            catch (Exception e)
            {
                IsEnabled = false;
                SendExceptionMessage3(FullCode, GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod().Name,ToString(), e);
            }
            return null;
        }
        private async void AddEventLogAsync(EventLogDto evl)
        {
            var task = _client.PostAsJsonAsync(ApiEventLogs, evl)
                .ContinueWith(x =>
                {
                    //  bool IsSuccsess = x.Result.IsSuccessStatusCode;
                    if (x.Result.IsSuccessStatusCode)
                        WebDbEventLog = x.Result.Content.ReadAsAsync<EventLogDto>().Result;
                });
            await task;
        }
        private void AddItem(EventLogItemDto evli)
        {
            if (WebDbEventLog == null)
                InitEventLog();
            if (!IsEnabled)
                return;
            evli.Index = _index++;

            //AddItemAsync(evli);
            if( IsAsync)
                AddElementAsync<EventLogItemDto>(evli, ApiEventLogItems);
            else
                AddItemSync(evli);
        }

        private async void AddElementAsync<T>(T element, string apiStr)
        {
            T elementAdded;
         //   Console.WriteLine("Request --- Insert(Add) EventLogItem");
            var task = _client.PostAsJsonAsync(apiStr, element)
                .ContinueWith(x =>
                {
                    //  bool IsSuccsess = x.Result.IsSuccessStatusCode;
                    var y = x;
                    if (y.Result.IsSuccessStatusCode)
                    {
                        elementAdded = y.Result.Content.ReadAsAsync<T>().Result;
                        var uri = y.Result.Headers.Location;
                        // Console.WriteLine("Response --- Insert Success Uri= {0}\n {1}", uri.ToString(), elementAdded);
                    }
                });
            await task;
        }

        private async void AddItemAsync(EventLogItemDto evli)
        {
            EventLogItemDto evliAdded;
            // Console.WriteLine("Request --- Insert(Add) EventLogItem");
            var task = _client.PostAsJsonAsync(ApiEventLogItems, evli)
                .ContinueWith(x =>
                {
                    //  bool IsSuccsess = x.Result.IsSuccessStatusCode;
                    var y = x;
                    if (y.Result.IsSuccessStatusCode)
                    {
                        evliAdded = y.Result.Content.ReadAsAsync<EventLogItemDto>().Result;
                        var uri = y.Result.Headers.Location;
                        // Console.WriteLine("Response --- Insert Success Uri= {0}\n {1}", uri.ToString(), evliAdded);
                    }
                });
            await task;
        }

        private void AddItemSync(EventLogItemDto evli)
        {
            //evli.EventLogID = WebDbEventLog.ID;
            try
            {
                using (HttpResponseMessage response = _client.PostAsJsonAsync(ApiEventLogItems, evli).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var evlUri = response.Headers.Location;
                        Console.WriteLine("AddItemSync(): Success: {0}, Phrase: {1}",
                            response.StatusCode, response.ReasonPhrase);
                        // return 1;
                    }
                    else // (int)response.StatusCode, response.ReasonPhrase);
                        Console.WriteLine("AddItemSync() ERROR: {0} ({1})", response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                IsEnabled = false;
                SendExceptionMessage3(FullCode, GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod().Name,ToString(), e);
            }
            //return null;
            //return -1;

        }

        public void AddItem(EvlResult result, string operation, string description)
        {
            throw new NotImplementedException();
        }

        //public void AddItem(EvlResult result, EvlSubject subject, string source, string operation, string description, string objects)
        //{
        //    if (WebDbEventLog == null)
        //        InitEventLog();

        //    AddItem(new EventLogItemDto
        //    {
        //        DT = DateTime.Now,
        //        EventLogID = WebDbEventLog.ID,
        //        ResultCode = result,
        //        Subject = subject,
        //        Source = source,
        //        Operation = operation,
        //        Description = description,
        //        Object = objects
        //    });
        //}

        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
                            string objects)
        {
            if (WebDbEventLog == null)
                InitEventLog();
            if (!IsEnabled)
                return;
            AddItem(new EventLogItemDto
            {
                DT = DateTime.Now,
                EventLogID = WebDbEventLog.ID,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = entity,
                Operation = operation,
                Description = description,
                Object = objects
            });
        }

        public event EventHandler<IEventArgs> EventLogItemsChanged;

        protected virtual void OnEventLogItemsChanged(IEventArgs e)
        {
            EventHandler<IEventArgs> handler = EventLogItemsChanged;
            if (handler != null) handler(this, e);
        }

        public override IEnumerable<IEventLogItem> Items {
            get { throw new NotImplementedException();}
        }

        public void AddItem(IEventLogItem i)
        {
            if (WebDbEventLog == null)
                InitEventLog();

            AddItem(new EventLogItemDto
            {
                DT = i.DT,
                EventLogID = WebDbEventLog.ID,
                ResultCode = i.ResultCode,
                Subject = i.Subject,
                Source = i.Source,
                Entity = i.Entity,
                Operation = i.Operation,
                Description = i.Description,
                Object = i.Object
            });
        }

        public void ClearSomeData(int count)
        {
            // throw new NotImplementedException();
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[Type:{0}, Name:{1}, BaseAddress:{2}, ApiEventLogs:{3},  ApiEventLogItems:{4},  ASync: {5}, Enable: {6}]",
                    GetType(), Name, BaseAddress, ApiEventLogs, ApiEventLogItems, IsAsync, IsEnabled);
        }
    }
        
 }
