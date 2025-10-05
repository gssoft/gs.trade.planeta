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
    public class WebClient2<T> : IDisposable
    {
        private readonly HttpClient _client;
        public string MediaTypeHeader { get; set; }
        public string ApiPrefix { get; set; }

        public bool IsQueueEnabled {
            get { return QueuePostTimeInterval > 0; }
        }
        public int QueuePostTimeInterval { get; set; }
        protected int DefaultQueuePostTimeInterval = 5;

        public bool ConnectionStatus { get; private set; }

        protected HttpResponseMessage HttpResponse { get; set; }
        public bool HttpResponseStatus {
            get { return HttpResponse != null && HttpResponse.IsSuccessStatusCode;
            }
        }
        public HttpStatusCode HttpStatusCode {
            get { return HttpResponse != null ? HttpResponse.StatusCode : HttpStatusCode.BadRequest; }
        }
        public string HttpReasonPhrase {
            get { return HttpResponse != null ? HttpResponse.ReasonPhrase : "Unknown"; }
        }

        public string ErrorMessage { get; set; }

        private Task _t;
        protected ConcurrentQueue<T> Queue { get; set; }

        protected CancellationTokenSource Cts { get; set; }
        protected CancellationToken Token { get; set; }
        protected bool DoWork { get; set; }

        public WebClient2(string baseAddress, string mediaType, string apiprefix)
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
            _t = Task.Factory.StartNew(() =>
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
                    if (QueuePostTimeInterval > 0)
                        Thread.Sleep(QueuePostTimeInterval * 1000);
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

        //private bool PostItem(T t)
        //{
        //    ErrorMessage = string.Empty;

        //    var response = MediaTypeHeader.Trim() == "application/xml"
        //        ? _client.PostAsXmlAsync(ApiPrefix, t).Result
        //        : _client.PostAsJsonAsync(ApiPrefix, t).Result;

        //    return response.IsSuccessStatusCode;
        //}

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
        private HttpResponseMessage PostData(T t)
        {
            var response = default (HttpResponseMessage);
            try
            {
                ErrorMessage = "Ok";
                response = MediaTypeHeader.Trim() == "application/xml"
                    ? _client.PostAsXmlAsync(ApiPrefix, t).Result
                    : _client.PostAsJsonAsync(ApiPrefix, t).Result;
                HttpResponse = response;
                return response;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
                HttpResponse = response;
            }
            return null;
        }

        private async Task<HttpResponseMessage>  PostDataAsync(T t)
        {
            return await _client.PostAsJsonAsync(ApiPrefix, t);
        }

        public bool PostItem(T t)
        {
            ErrorMessage = "Ok";
            try
            {
                if (IsQueueEnabled)
                    Queue.Enqueue(t);
                else
                {
                    var response = PostData(t);
                    HttpResponse = response;
                    return response != null;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
            }
            return false;
        }

        public bool PostFromQueue()
        {
            long postCnt = 0;
            while (Queue.Count > 0)
            {
                T t;
                Queue.TryPeek(out t);
                Console.WriteLine("PostFromQueue: Tr:{0}", t);
                try
                {
                    if (PostData(t)==null)
                        return false;
                    Queue.TryDequeue(out t);
                    postCnt++;
                }
                catch (Exception e)
                {
                    ErrorMessage = e.ExceptionMessage();
                    Console.WriteLine("PostFromQueue() Failure.\r\nErrorMessage{0}", ErrorMessage);
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
            if (_t != null)
                _t.Wait();
            PostFromQueue();
            _client.Dispose();
        }
    }
}
