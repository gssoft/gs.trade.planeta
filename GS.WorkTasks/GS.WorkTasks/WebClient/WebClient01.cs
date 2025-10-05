//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using GS.Extension;

// Some changes


//namespace WebClients
//{
//    public class WebClient01<T> : IDisposable, IWebClient
//    {
//         private readonly HttpClient _client;
//        public string MediaTypeHeader { get; set; }
//        public string ApiPrefix { get; set; }

//        public bool IsQueueEnabled {
//            get { return QueuePostTimeInterval > 0; }
//        }
//        public int QueuePostTimeInterval { get; set; }
//        protected int DefaultQueuePostTimeInterval = 15;

//        public bool ConnectionStatus { get; private set; }

//        protected HttpResponseMessage HttpResponse { get; set; }
//        public bool HttpResponseStatus {
//            get { return HttpResponse != null && HttpResponse.IsSuccessStatusCode;
//            }
//        }
//        public HttpStatusCode HttpStatusCode {
//            get { return HttpResponse != null ? HttpResponse.StatusCode : HttpStatusCode.BadRequest; }
//        }
//        public string HttpReasonPhrase {
//            get { return HttpResponse != null ? HttpResponse.ReasonPhrase : "Unknown"; }
//        }

//        public string ErrorMessage { get; set; }

//        private Task _taskMain;
//        private Task _taskController;

//        private DateTime _lastWorkDateTime;
//        protected ConcurrentQueue<T> Queue { get; set; }

//        protected CancellationTokenSource Cts { get; set; }
//        protected CancellationToken Token { get; set; }

//        protected CancellationTokenSource ControllerCts { get; set; }
//        protected CancellationToken ControllerToken { get; set; }

//        protected bool DoWork { get; set; }

//        protected bool IsUpToDateTime {
//            get
//            {
//                if (DateTime.Now - _lastWorkDateTime <= TimeSpan.FromSeconds(QueuePostTimeInterval))
//                    return true;
//                // Console.WriteLine("Something wrong with Work !!!");
//                return false;
//            }
//        }

//        protected AutoResetEvent AutoResetMain;
//        protected AutoResetEvent ControllerAutoReset;

//        public WebClient01()
//        {
//            _lastWorkDateTime = DateTime.Now;
//        }

//        public WebClient01(string baseAddress, string mediaType, string apiPrefix) 
//        {
//            QueuePostTimeInterval = QueuePostTimeInterval > 0 ? QueuePostTimeInterval : DefaultQueuePostTimeInterval;

//            Queue = new ConcurrentQueue<T>();
//            _client = new HttpClient
//            {
//                BaseAddress = new Uri(baseAddress)
//            };
//            _client.DefaultRequestHeaders.Accept.Add(
//               new MediaTypeWithQualityHeaderValue(mediaType));
//            ApiPrefix = apiPrefix;
//            MediaTypeHeader = mediaType;

//            AutoResetMain = new AutoResetEvent(false);
//            ControllerAutoReset = new AutoResetEvent(false);
//        }

        

//        public bool Start()
//        {
//            QueuePostTimeInterval = QueuePostTimeInterval > 0 ? QueuePostTimeInterval : DefaultQueuePostTimeInterval;
            
//            StartMainTask();

//            ControllerCts = new CancellationTokenSource();
//            ControllerToken = ControllerCts.Token;

//            _taskController = Task.Factory.StartNew(() =>
//            {
//                Console.WriteLine("Start Control Task: PostFromQueue");
//                while (true)
//                {               
//                    if (ControllerToken.IsCancellationRequested)
//                    {
//                        Console.WriteLine("Controller: ControllerTask Cancelled");
//                        //Token.ThrowIfCancellationRequested();
//                        //doWork = false;
//                        return;
//                    }
//                    if (_taskMain == null)
//                    {
//                        Console.WriteLine("Controller: Main is Null");
//                        Console.WriteLine(Environment.NewLine + "Controller: Try to Restart Main");
//                        StartMainTask();
//                        // continue;
//                    }
//                    if (_taskMain != null && (_taskMain.IsFaulted || _taskMain.IsCanceled || _taskMain.IsCompleted))
//                    {
//                        Console.WriteLine("Controller: Main is Canselled");
//                        Console.WriteLine(Environment.NewLine + "Controller: Try to Restart Main");
//                        StartMainTask();
//                        //continue;
//                    }
//                    else if (!IsUpToDateTime)
//                    {
//                        Console.WriteLine("Controller: MainTask is Not UpToDate !!!");

//                        Cts.Cancel();
//                        AutoResetMain.Set();

//                        //if (_taskMain != null)
//                        //    _taskMain.Wait(3000);

//                        //if (_taskMain == null)
//                        //{
//                        //    Console.WriteLine("Controller: Main is Null");
//                        //    Console.WriteLine(Environment.NewLine + "Controller: Try to Restart Main");
//                        //    StartMainTask();
//                        //    continue;
//                        //}
//                        //if (_taskMain != null && (_taskMain.IsFaulted || _taskMain.IsCanceled || _taskMain.IsCompleted))
//                        //{
//                        //    Console.WriteLine("Controller: Main is Canselled");
//                        //    Console.WriteLine(Environment.NewLine + "Controller: Try to Restart Main");
//                        //    StartMainTask();
//                        //    //continue;
//                        //}
//                    }
//                    else
//                    {
//                        Console.WriteLine("Controller: MainTask is Working Properly");
//                    }
//                    ControllerAutoReset.WaitOne(1000 * 3);

//                }
//            }, ControllerToken);

//            return true;

//        }

//        private int _countToError;
//        private void StartMainTask()
//        {
//            Status = WebClientStatus.Starting;

//            Cts = new CancellationTokenSource();
//            Token = Cts.Token;

//            _countToError = 5;

//            _taskMain = Task.Factory.StartNew(() =>
//            {
//                //bool doWork = true;
//                Console.WriteLine("Start Working Task: PostFromQueue");
//                Status = WebClientStatus.Started;
//                while (true)
//                {
//                    try
//                    {
//                        if (_countToError-- < 0)
//                        {
//                            throw new NullReferenceException("Task get this Exception");
//                        }
//                        if (Token.IsCancellationRequested)
//                        {
//                            Console.WriteLine("MainTask is Cancelled");
//                            //Token.ThrowIfCancellationRequested();
//                            //doWork = false;
//                            return;
//                        }
//                        _lastWorkDateTime = DateTime.Now;
//                        PostFromQueue();
//                        //Console.WriteLine("MainTask.PostHandler: Wait for ResetEvent");
//                        AutoResetMain.WaitOne(1000 * (QueuePostTimeInterval));
//                        //Console.WriteLine("MainTask.PostHandler: AutoResetEvent.SET");
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine("MainTask: ***** Exception *****: " + e.Message);
//                        StopMainTask();
//                        return;
//                    }
                    
//                    //if (QueuePostTimeInterval > 0)
//                    //    Thread.Sleep(QueuePostTimeInterval * 1000);
//                    //else
//                    //    Thread.Sleep(DefaultQueuePostTimeInterval * 1000);
//                }
//            }, Token);
//        }

//        public bool Stop()
//        {
//            Console.WriteLine("Stopping ....");

//            Console.WriteLine("Try to Stop MainTask  ....");
//            Cts.Cancel();
//            AutoResetMain.Set();
            
//            if (_taskMain != null)
//                _taskMain.Wait();

//            Console.WriteLine("Try to Stop ControllerTask  ....");
//            ControllerCts.Cancel();
//            ControllerAutoReset.Set();

//            if (_taskController != null)
//                _taskController.Wait();

//            Console.WriteLine("Stopped");
//            return true;
//        }

//        private void StopMainTask()
//        {
//            Cts.Cancel();
//            AutoResetMain.Set();
//        }

//        public void Close()
//        {
//            //DoWork = false;
//            Cts.Cancel();

//        }

//        //private bool PostItem(T t)
//        //{
//        //    ErrorMessage = string.Empty;

//        //    var response = MediaTypeHeader.Trim() == "application/xml"
//        //        ? _client.PostAsXmlAsync(ApiPrefix, t).Result
//        //        : _client.PostAsJsonAsync(ApiPrefix, t).Result;

//        //    return response.IsSuccessStatusCode;
//        //}

//        private bool PostData(T t, out HttpResponseMessage response)
//        {
//            response = default(HttpResponseMessage);
//            try
//            {
//                response = MediaTypeHeader.Trim() == "application/xml"
//                    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
//                    : _client.PostAsJsonAsync(ApiPrefix, t).Result;

//                ErrorMessage = "Ok";
//                return response.IsSuccessStatusCode;
//            }
//            catch (Exception e)
//            {
//                ErrorMessage = e.ExceptionMessage();
//            }
//            return false;
//        }
//        private HttpResponseMessage PostData(T t)
//        {
//            var response = default (HttpResponseMessage);
//            try
//            {
//                ErrorMessage = "Ok";
//                response = MediaTypeHeader.Trim() == "application/xml"
//                    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
//                    : _client.PostAsJsonAsync(ApiPrefix, t).Result;
//                HttpResponse = response;
//                return response;
//            }
//            catch (Exception e)
//            {
//                ErrorMessage = e.ExceptionMessage();
//                HttpResponse = response;
//            }
//            return null;
//        }

//        public bool PostItem(T t)
//        {
//            ErrorMessage = "Ok";
//            try
//            {
//                if (IsQueueEnabled)
//                {
//                    Queue.Enqueue(t);
//                    AutoResetMain.Set();
//                }
//                else
//                {
//                    var response = PostData(t);
//                    HttpResponse = response;
//                    return response != null;
//                }
//            }
//            catch (Exception e)
//            {
//                ErrorMessage = e.ExceptionMessage();
//            }
//            return false;
//        }

//        public bool PostFromQueue()
//        {
//            long postCnt = 0;
//            while (Queue.Count > 0)
//            {
//                T t;
//                Queue.TryPeek(out t);
//                Console.WriteLine("MainTask.PostFromQueue: {0}", t);
//                try
//                {
//                    if (PostData(t)==null)
//                        return false;
//                    Queue.TryDequeue(out t);
//                    postCnt++;
//                }
//                catch (Exception e)
//                {
//                    ErrorMessage = e.ExceptionMessage();
//                    Console.WriteLine("PostFromQueue() Exception.\r\nErrorMessage{0}", ErrorMessage);
//                    return false;
//                }    
//            }
//            Console.WriteLine("Posts Count:{0}", postCnt);
//            return true;
//        }

//        public IEnumerable<T> GetItems()
//        {
//            ErrorMessage = "Ok";
//            var response = default(HttpResponseMessage);
//            try
//            {
//                response = _client.GetAsync(ApiPrefix).Result;
//                HttpResponse = response;
//                if (!response.IsSuccessStatusCode)
//                    return null;
//                var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
//                return items;
//            }
//            catch (Exception e)
//            {
//                HttpResponse = response;
//                ErrorMessage = e.ExceptionMessage();
//            }
//            return null;

//            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
//        }
//        public IEnumerable<T> GetItems(string parameters)
//        {
//            ErrorMessage = "Ok";
//            var response = default(HttpResponseMessage);
//            try
//            {
//                response = _client.GetAsync(ApiPrefix + parameters).Result;
//                HttpResponse = response;
//                if (!response.IsSuccessStatusCode)
//                    return null;
//                var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
//                return items;
//            }
//            catch (Exception e)
//            {
//                HttpResponse = response;
//                ErrorMessage = e.ExceptionMessage();
//            }
//            return null;

//            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
//        }
//        /// <summary>
//        /// Get Item
//        /// </summary>
//        /// <param name="parameters"></param>
//        /// <returns></returns>
//        public T GetItem(string parameters)
//        {
//            ErrorMessage = "Ok";
//            var response = default(HttpResponseMessage);
//            try
//            {
//                response = _client.GetAsync(ApiPrefix + parameters).Result;
//                HttpResponse = response;
//                if (!response.IsSuccessStatusCode)
//                    return default(T);
//                return response.Content.ReadAsAsync<T>().Result;
//            }
//            catch (Exception e)
//            {
//                HttpResponse = response;
//                ErrorMessage = e.ExceptionMessage();
//            }
//            return default(T);
//        }
//        public override string ToString()
//        {
//            return string.Format("BaseAdr:{0}; Api:{1}; HttpResponse:{5}; HttpStatus:{2}; HttpReason:{3}; ErrorMess:{4}",
//                                        _client.BaseAddress, ApiPrefix, HttpStatusCode, HttpReasonPhrase, ErrorMessage, HttpResponseStatus);
//        }

//        public void Dispose()
//        {
//            Console.WriteLine("Dispose");
//            Close();

//            if (_taskMain != null)
//                _taskMain.Wait();

//            PostFromQueue();
//            _client.Dispose();
//        }

//        public WebClientStatus Status { get; private set; }
//    }
//}
