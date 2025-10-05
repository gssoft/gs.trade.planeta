using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.WorkTasks;

namespace WebClients
{
    public class WebClient03<T> : Element1<string>
    {
        public Worker<T> Worker { get; set; }

          public WorkTask WorkTask { get; set; }

        private HttpClient _client;
        public string BaseAddress { get; set; }
        public string ApiPrefix { get; set; }
        public string MediaTypeHeader { get; set; }
        public bool IsQueueEnabled { get; set; }
   
        public int QueuePostTimeInterval { get; set; }
        protected int DefaultQueuePostTimeInterval = 15;

        [XmlIgnore]
        public bool ConnectionStatus { get; private set; }

        protected HttpResponseMessage HttpResponse { get; set; }
        public bool HttpResponseStatus
        {
            get
            {
                return HttpResponse != null && HttpResponse.IsSuccessStatusCode;
            }
        }
        public HttpStatusCode HttpStatusCode
        {
            get { return HttpResponse != null ? HttpResponse.StatusCode : HttpStatusCode.BadRequest; }
        }
        public string HttpReasonPhrase
        {
            get { return HttpResponse != null ? HttpResponse.ReasonPhrase : "Unknown"; }
        }

        [XmlIgnore]
        public string ErrorMessage { get; set; }

        private DateTime _lastWorkDateTime;
        protected ConcurrentQueue<T> Queue { get; set; }
        [XmlIgnore]
        public int ErrorCount { get; private set; }
        public int ErrorCountToStop { get; set; }

        private MediaTypeFormatter _mediaTypeFormatter;

        public WebClient03()
        {
            _lastWorkDateTime = DateTime.Now;

            Queue = new ConcurrentQueue<T>();
            WorkTask = new WorkTask();

            IsQueueEnabled = true;
        }

        public WebClient03(string baseAddress, string mediaType, string apiPrefix)
        {
            QueuePostTimeInterval = QueuePostTimeInterval > 0 ? QueuePostTimeInterval : DefaultQueuePostTimeInterval;

            Queue = new ConcurrentQueue<T>();
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue(mediaType));
            ApiPrefix = apiPrefix;
            MediaTypeHeader = mediaType;

            WorkTask = new WorkTask
            {
                TimeInterval = 15,
                Code = "WebClient.Values",
                ErrorCountToStop = 1000,
                TaskFunc = PostFromQueueAsync,
            };
        }

        public void Init()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue(MediaTypeHeader));

            switch (MediaTypeHeader)
            {
                case "application/json":
                    _mediaTypeFormatter = new JsonMediaTypeFormatter();
                    break;

                case "application/xml":
                case "text/xml":
                    _mediaTypeFormatter = new XmlMediaTypeFormatter();
                    break;
                default:
                    _mediaTypeFormatter = new JsonMediaTypeFormatter();
                    break;
            }           

            // WorkTask.TaskFunc = PostFromQueue;
            WorkTask.TaskFunc = PostFromQueueAsync;
        }

        public bool Start()
        {
            Status = WebClientStatus.Starting;
            ErrorCount = 0;
            if (WorkTask.TaskFunc == null)
                WorkTask.TaskFunc = PostFromQueue;
            WorkTask.Start();
            Status = WebClientStatus.Started;

            return true;
        }

        public bool Stop()
        {
            Console.WriteLine("Stopping ....");

            Console.WriteLine("Try to Stop MainTask  ....");

            Status = WebClientStatus.Stopping;

            if (WorkTask != null)
                WorkTask.Stop();

            Status = WebClientStatus.Stopped;

            Console.WriteLine("Try to Stop ControllerTask  ....");
            //ControllerCts.Cancel();
            //ControllerAutoReset.Set();

            //if (_taskController != null)
            //    _taskController.Wait();

            Console.WriteLine("Stopped");
            return true;
        }

        public void Close()
        {
           if(WorkTask!=null)
               WorkTask.Stop();
        }

        private bool PostData(T t, out HttpResponseMessage response)
        {
            response = default(HttpResponseMessage);
            try
            {
                response = MediaTypeHeader.Trim() == "application/xml"
                    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                    : _client.PostAsJsonAsync(ApiPrefix, t).Result;

                ErrorMessage = "Ok";
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
            }
            return false;
        }
        private bool PostData(T t)
        {
            if (Status != WebClientStatus.Started)
                return false;

            var response = default(HttpResponseMessage);
            try
            {
                ErrorMessage = "Ok";
                //response = MediaTypeHeader.Trim() == "application/xml"
                //    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                //    : _client.PostAsJsonAsync(ApiPrefix, t).Result;
                response = _client.PostAsync(ApiPrefix, t, _mediaTypeFormatter).Result;
                HttpResponse = response;
              
                if (response != null && response.IsSuccessStatusCode)
                    return true;

                ErrorProcess();
                return false;
            }
            catch (Exception e)
            {
                ErrorProcess(); 
                ErrorMessage = e.ExceptionMessage();
                HttpResponse = response;
            }
            return false;
        }
        private async Task<bool> PostDataAsync(T t)
        {
            var response = default(HttpResponseMessage);
            try
            {
                ErrorMessage = "Ok";

                response = await _client.PostAsync(ApiPrefix, t, _mediaTypeFormatter);
                if (response.IsSuccessStatusCode)
                    return true;
                ErrorProcess();
                return false;
            }
            catch (Exception e)
            {
                ErrorProcess();
                ErrorMessage = e.ExceptionMessage();
                return false;
            }
            return true;
        }
        private bool PostDataExc(T t)
        {
            if (Status != WebClientStatus.Started)
                return false;

            var response = default(HttpResponseMessage);
            try
            {
                ErrorMessage = "Ok";
                var task = _client.PostAsync(ApiPrefix, t, _mediaTypeFormatter);
                response = task.Result;
                if(task.Exception != null)
                    throw new Exception(task.Exception.ExceptionMessage());

                if (response != null && response.IsSuccessStatusCode)
                    return true;
            }
            catch (Exception e)
            {
                ErrorProcess();
                ErrorMessage = e.ExceptionMessage();
                HttpResponse = response;
            }
            return false;
        }
        public bool PostItem(T t)
        {
            ErrorMessage = "Ok";
            try
            {
                if (IsQueueEnabled)
                {
                    Queue.Enqueue(t);
                    //AutoResetMain.Set();
                    WorkTask.DoWork();
                }
                else
                {
                    return PostData(t);
                }
            }
            catch (Exception e)
            {
                ErrorProcess();
                ErrorMessage = e.ExceptionMessage();
            }
            return false;
        }

        public bool PostFromQueue()
        {
            long postCnt = 0;
            while (Queue.Count > 0)
            {
                var dt1 = DateTime.Now;
                T t;
                Queue.TryPeek(out t);
                //Console.WriteLine("MainTask.PostFromQueue: {0}", t);              
                try
                {
                    if (!PostData(t))
                        return false;

                    Queue.TryDequeue(out t);
                    postCnt++;
                    var dt2 = DateTime.Now;
                    var posttime = dt2 - dt1;
                    ConsoleAsync.WriteLineT("MainTask.PostFromQueue: {0}; Erros: {1} Time: {2}", t, ErrorCount, posttime.ToString("c"));
                }
                catch (Exception e)
                {
                    ErrorProcess(); 
                    ErrorMessage = e.ExceptionMessage();
                    ConsoleAsync.WriteLineT("PostFromQueue() Exception.\r\nErrorMessage{0}", ErrorMessage);
                    return false;
                }
            }
            ConsoleAsync.WriteLineT("Posts Count:{0}", postCnt);
            return true;
        }
        public bool PostFromQueueAsync()
        {
            long postCnt = 0;
            while (Queue.Count > 0)
            {
                var dt1 = DateTime.Now;
                T t;
                Queue.TryPeek(out t);
                //Console.WriteLine("MainTask.PostFromQueue: {0}", t);              
                try
                {
                    var task = PostDataAsync(t);

                    var result = task.Result;
                    if (task.Exception != null)
                        throw new Exception(task.Exception.ExceptionMessage());
                    if (!result)
                        return false;

                    Queue.TryDequeue(out t);
                    postCnt++;
                    var dt2 = DateTime.Now;
                    var posttime = dt2 - dt1;
                    Console.WriteLine("MainTask.PostFromQueue: {0}; Erros: {1} Time: {2}", t, ErrorCount, posttime.ToString("c"));
                }
                catch (Exception e)
                {
                    ErrorProcess();
                    ErrorMessage = e.ExceptionMessage();
                    Console.WriteLine("PostFromQueue() Exception.\r\nErrorMessage{0}", ErrorMessage);
                    return false;
                }
            }
            Console.WriteLine("Posts Count:{0}", postCnt);
            return true;
        }
        public IEnumerable<T> GetItems()
        {
            ErrorMessage = "Ok";
            var response = default(HttpResponseMessage);
            try
            {
                response = _client.GetAsync(ApiPrefix).Result;
                HttpResponse = response;
                if (!response.IsSuccessStatusCode)
                    return null;
                var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                return items;
            }
            catch (Exception e)
            {
                HttpResponse = response;
                ErrorMessage = e.ExceptionMessage();
            }
            return null;

            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
        }
        public IEnumerable<T> GetItems(string parameters)
        {
            ErrorMessage = "Ok";
            var response = default(HttpResponseMessage);
            try
            {
                response = _client.GetAsync(ApiPrefix + parameters).Result;
                HttpResponse = response;
                if (!response.IsSuccessStatusCode)
                    return null;
                var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                return items;
            }
            catch (Exception e)
            {
                HttpResponse = response;
                ErrorMessage = e.ExceptionMessage();
            }
            return null;

            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
        }
        /// <summary>
        /// Get Item
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T GetItem(string parameters)
        {
            ErrorMessage = "Ok";
            var response = default(HttpResponseMessage);
            try
            {
                response = _client.GetAsync(ApiPrefix + parameters).Result;
                HttpResponse = response;
                if (!response.IsSuccessStatusCode)
                    return default(T);
                return response.Content.ReadAsAsync<T>().Result;
            }
            catch (Exception e)
            {
                HttpResponse = response;
                ErrorMessage = e.ExceptionMessage();
            }
            return default(T);
        }
        public override string ToString()
        {
            return string.Format("BaseAdr:{0}; Api:{1}; HttpResponse:{5}; HttpStatus:{2}; HttpReason:{3}; ErrorMess:{4}",
                                        _client.BaseAddress, ApiPrefix, HttpStatusCode, HttpReasonPhrase, ErrorMessage, HttpResponseStatus);
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
            Close();

            PostFromQueue();

            _client.Dispose();
        }

        [XmlIgnore]
        public WebClientStatus Status { get; private set; }

        private void ErrorProcess()
        {
            if (++ErrorCount > ErrorCountToStop)
                Stop();
        }

        public override string Key
        {
            get { return Code.HasValue() ? Code : "Unknown"; }
        }
    }
}
