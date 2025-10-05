using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Clients
{
    public class WebClient
    {
        public string Code { get; set; }
        public string BaseAddress { get; set; }
        public string ApiString { get; set; }
        public string RequestHeader { get; set; }
        public bool IsAsync { get; set; }
        public bool IsEnabled { get; set; }

        [XmlIgnore]
        public string KeyStr {
            get { return Code; }
        }

        private HttpClient _client;
        private WebClients _webClients; 

        public WebClient()
        {
            //BaseAddress = "http://localhost/WebApi_01/";
            //RequestHeader = "application/json";
            //ApiString = "api/tickers/";
          
            //  IsAsync = false;

            
            //_client.BaseAddress = new Uri(BaseAddress);

            //// Add an Accept header for JSON format.
            //_client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue(RequestHeader));
        }
        public void Init(WebClients wcs)
        {
            _webClients = wcs;
            _client = new HttpClient {BaseAddress = new Uri(BaseAddress)};

            // Add an Accept header for JSON format.
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(RequestHeader));
            wcs.Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "WebClient", Code, "Init", ToString(),"");
          //  Console.WriteLine("Init: " + ToString());
        }
        public override string ToString()
        {
            return string.Format("[Type:{0}, Code:{1}, BaseAddress:{2}, ApiString:{3}, Header:{4}, ASync: {5}, Enable: {6}]",
                                    GetType(), Code, BaseAddress, ApiString, RequestHeader, IsAsync, IsEnabled);
        }
        protected T GetSync<T>(int id)
        {
            var requestStr = ApiString + id;
            using (HttpResponseMessage response = _client.GetAsync(requestStr).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var t = response.Content.ReadAsAsync<T>().Result;
                    var url = response.Headers.Location;
                    _webClients.Evlm(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "WebClient", Code, "Get: " + requestStr, (int)response.StatusCode +" "+ response.ReasonPhrase, t.ToString());
                    return t;
                }
                else
                {
                    _webClients.Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, "WebClient", Code, "Get Failure: " + requestStr, (int)response.StatusCode +" " + response.ReasonPhrase,"");
                }
            }
            //  Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            return default(T);
        }

        protected T GetSync<T>(string par)
        {
            var requestStr = ApiString + par;
            using (HttpResponseMessage response = _client.GetAsync(requestStr).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var t = response.Content.ReadAsAsync<T>().Result;
                    var uri = response.Headers.Location;
                    //Console.WriteLine("Get {0} {1}", t, uri);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _webClients.Evlm(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "WebClient", Code, "Get: " + requestStr, (int)response.StatusCode + " " + response.ReasonPhrase, t.ToString());
                    return t;
                }
                else
                {
                    //Console.WriteLine("Get Error {0}", requestStr);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _webClients.Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, "WebClient", Code, "Get Failure: " + requestStr, (int)response.StatusCode + " " + response.ReasonPhrase, "");
                }
            }
            
            return default(T);
        }

        protected bool PostSync<T>(T element)
        {
            using (HttpResponseMessage response = _client.PostAsJsonAsync(ApiString, element).Result)
            {
                return response.IsSuccessStatusCode;
            }
        }
        protected T Post2Sync<T>(T element)
        {
            var e = default(T);
            using (HttpResponseMessage response = _client.PostAsJsonAsync(ApiString, element).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    e = response.Content.ReadAsAsync<T>().Result;
                    var uri = response.Headers.Location;

                    //Console.WriteLine("Post {0}", e);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

                    _webClients.Evlm(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "WebClient", Code, "Post: " + ApiString, (int)response.StatusCode + " " + response.ReasonPhrase, e.ToString());
                    return e;

                }
                else
                {
                    //Console.WriteLine("Put Error {0} {1}", element, ApiString);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _webClients.Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, "WebClient", Code, "Post Failure: " + ApiString, (int)response.StatusCode + " " + response.ReasonPhrase, "");
                    return default(T);
                }
                
            }
        }

        protected bool PutSync<T>(int id, T t)
        {
            var requestStr = ApiString + id;
            using (HttpResponseMessage response = _client.PutAsJsonAsync(requestStr, t).Result)
            {
                return response.IsSuccessStatusCode;
            }
        }
        protected bool Put2Sync<T>(int id, T element)
        {
            //var e = default(T);
            var requestStr = ApiString + id;
            using (HttpResponseMessage response = _client.PutAsJsonAsync(requestStr, element).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    // e = response.Content.ReadAsAsync<T>().Result;
                    // var uri = response.Headers.Location;

                    //Console.WriteLine("Put {0}", element);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _webClients.Evlm(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "WebClient", Code, "Put: " + requestStr, (int)response.StatusCode + " " + response.ReasonPhrase, "");

                    return true;
                }
                else
                {
                    //Console.WriteLine("Put Error {0} {1}", element, requestStr);
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _webClients.Evlm(EvlResult.FATAL, EvlSubject.PROGRAMMING, "WebClient", Code, "Put Failure: " + requestStr, (int)response.StatusCode + " " + response.ReasonPhrase, "");
                    return false;
                }
            }
        }

        protected async void PostAsync<T>(T element)
        {
            await _client.PostAsJsonAsync(ApiString, element);
        }
        protected async void Post2Async<T>(T element)
        {
            var task = _client.PostAsJsonAsync(ApiString, element)
                .ContinueWith(x =>
                {
                    if (x.Result.IsSuccessStatusCode)
                    {
                        // EventLog
                        var el = x.Result.Content.ReadAsAsync<T>().Result;
                        var uri = x.Result.Headers.Location;
                    }
                    else
                    {
                        //  EventLog
                    }
                });
            await task;
        }

        protected async void PutAsync<T>(T element, string apiStr)
        {
            await _client.PutAsJsonAsync(apiStr, element);
        }
        protected async void Put2Async<T>(T element, string apiStr)
        {
            var task = _client.PutAsJsonAsync(apiStr, element)
                .ContinueWith(x =>
                {
                    if (x.Result.IsSuccessStatusCode)
                    {
                        var e = x.Result.Content.ReadAsAsync<T>().Result;
                        var evlUri = x.Result.Headers.Location;
                        //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                    else
                    {
                        //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }
                });
            await task;

        }
       
    }
}
