using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.WebClients
{
    public class WebClient<T> : IDisposable
    {
        private readonly HttpClient _client;
        // public string BaseAddress { get; set; }
        public string MediaTypeHeader { get; set; }
        public string ApiPrefix { get; set; }
        public bool IsQueueEnabled { get; set; }
        public int QueuePostTimeInterval { get; set; }
        protected int DefaultQueuePostTimeInterval = 5;

        public bool ConnectionStatus { get; private set; }

        public bool HttpResponseStatus { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public string HttpReasonPhrase { get; private set; }

        public string ErrorMessage { get; set; }

        protected ConcurrentQueue<T> Queue { get; set; }

        protected CancellationTokenSource Cts { get; set; }
        protected CancellationToken Token { get; set; }
        protected bool DoWork { get; set; }

        public WebClient(string baseAddress, string mediaType, string apiprefix)
        {
            Queue = new ConcurrentQueue<T>();
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue(mediaType));
            ApiPrefix = apiprefix;
            MediaTypeHeader = mediaType;

            Cts = new CancellationTokenSource();
            Token = Cts.Token;
            // DoWork = true;
            Task.Factory.StartNew(() =>
            {
                //bool doWork = true;
                Console.WriteLine("Start Process: PostFromQueue");
                while (true)
                {
                    if (Token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task Cancelled");
                        //Token.ThrowIfCancellationRequested();
                        //doWork = false;
                        return;
                    }
                    PostFromQueue();
                    if(QueuePostTimeInterval>0)
                        Thread.Sleep(QueuePostTimeInterval*1000);
                    else
                        Thread.Sleep(DefaultQueuePostTimeInterval * 1000);
                }
            }, Token);
        }

        public void Close()
        {
            //DoWork = false;
            Cts.Cancel();

        }

        private bool PostItem(T t)
        {
            ErrorMessage = string.Empty;
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;

            var response = MediaTypeHeader.Trim() == "application/xml"
                ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                : _client.PostAsJsonAsync(ApiPrefix, t).Result;

            HttpResponseStatus = response.IsSuccessStatusCode;
            HttpStatusCode = response.StatusCode;
            HttpReasonPhrase = response.ReasonPhrase;

            return response.IsSuccessStatusCode;
        }

        private bool PostItem(T t, out HttpResponseMessage response)
        {
            response = MediaTypeHeader.Trim() == "application/xml"
                ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                : _client.PostAsJsonAsync(ApiPrefix, t).Result;
            

            return response.IsSuccessStatusCode;
        }

        public bool PostData(T t)
        {
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;
            try
            {
                var response = MediaTypeHeader.Trim() == "application/xml"
                    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                    : _client.PostAsJsonAsync(ApiPrefix, t).Result;

                HttpResponseStatus = response.IsSuccessStatusCode;
                HttpStatusCode = response.StatusCode;
                HttpReasonPhrase = response.ReasonPhrase;

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
            }
            return false;
        }

        public bool PostData3(T t)
        {
            try
            {
                if (PostItem(t))
                    return true;
                Queue.Enqueue(t);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
                Queue.Enqueue(t);
            }
            return false;
        }

        public bool PostFromQueue()
        {
            while (Queue.Count > 0)
            {
                T t;
                Queue.TryPeek(out t);
                Console.WriteLine("PostFromQueue: Tr:{0}", t);
                try
                {
                    if (!PostItem(t))
                        return false;
                    Queue.TryDequeue(out t);
                }
                catch (Exception e)
                {
                    ErrorMessage = e.ExceptionMessage();
                    Console.WriteLine("PostFromQueue() Failure.\r\nErrorMessage{0}", ErrorMessage);
                    return false;
                }
            }
            return true;
        }

        public bool PostData2(T t)
        {
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;
            try
            {
                bool resp=true;
                if (Queue.Count <= 0)
                {
                    var response = MediaTypeHeader.Trim() == "application/xml"
                   ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                   : _client.PostAsJsonAsync(ApiPrefix, t).Result;

                    HttpResponseStatus = response.IsSuccessStatusCode;
                    HttpStatusCode = response.StatusCode;
                    HttpReasonPhrase = response.ReasonPhrase;

                    return response.IsSuccessStatusCode;
                }
                while (Queue.Count > 0)
                {
                    Queue.Enqueue(t);
                    T tr;
                    if (!Queue.TryPeek(out tr))
                        continue;
                    var response = MediaTypeHeader.Trim() == "application/xml"
                        ? _client.PostAsXmlAsync(ApiPrefix, tr).Result
                        : _client.PostAsJsonAsync(ApiPrefix, tr).Result;

                    HttpResponseStatus = response.IsSuccessStatusCode;
                    HttpStatusCode = response.StatusCode;
                    HttpReasonPhrase = response.ReasonPhrase;
                    resp = response.IsSuccessStatusCode;
                    if (response.IsSuccessStatusCode)
                        Queue.TryDequeue(out tr);
                    else
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
                Queue.Enqueue(t);
            }
            return false;
        }

        public IEnumerable<T> GetData()
        {
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;

            HttpResponseMessage response = _client.GetAsync(ApiPrefix).Result;

            HttpResponseStatus = response.IsSuccessStatusCode;
            HttpStatusCode = response.StatusCode;
            HttpReasonPhrase = response.ReasonPhrase;

            if (!response.IsSuccessStatusCode) 
                return null;
            var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
            return items;
            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
        }
        public IEnumerable<T> GetData(out bool operationStatus)
        {
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;

            HttpResponseMessage response = _client.GetAsync(ApiPrefix).Result;

            operationStatus = response.IsSuccessStatusCode;

            HttpStatusCode = response.StatusCode;
            HttpReasonPhrase = response.ReasonPhrase;

            if (!response.IsSuccessStatusCode)
                return null;
            var items = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
            return items;
            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
        }
        public T GetItem(out bool operationStatus, string parameters)
        {
            HttpReasonPhrase = string.Empty;
            HttpStatusCode = HttpStatusCode.Unused;

            HttpResponseMessage response = _client.GetAsync(ApiPrefix+parameters).Result;

            operationStatus = response.IsSuccessStatusCode;

            HttpStatusCode = response.StatusCode;
            HttpReasonPhrase = response.ReasonPhrase;

            if (!response.IsSuccessStatusCode)
                return default(T);
            var item = response.Content.ReadAsAsync<T>().Result;
            return item;
            //Console.WriteLine("Content:{0} Message:{1} Status:{2} Reason:{3}", response.Content, response.RequestMessage,  response.StatusCode, response.ReasonPhrase);
        }
        public override string ToString()
        {
            return string.Format("BaseAdr:{0}; Api:{1}; HttpResponse:{5}; HttpStatus:{2}; HttpReason:{3}; ErrorMess:{4}",
                                        _client.BaseAddress, ApiPrefix, HttpStatusCode,HttpReasonPhrase,ErrorMessage,HttpResponseStatus);
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
            Close();
            Thread.Sleep(1000);
            PostFromQueue();
            _client.Dispose();
        }
    }
}
