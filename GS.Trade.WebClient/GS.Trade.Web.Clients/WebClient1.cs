using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Clients
{
    public class WebClient1
    {
        public string EventLogKey { get; set; }
        public string BaseAddress { get; set; }

        public string RequestHeader { get; set; }

        public string ApiAccounts { get; set; }
        public string ApiTickers { get; set; }
        public string ApiStrategies { get; set; }

        public string ApiEventLogItems { get; set; }

        //[XmlIgnore]
        //public DataBase.Model.DbEventLog WebDbEventLog { get; private set; }

        private readonly HttpClient _client;

        private long _index = 0;

         public WebClient1()
        {
           // EventLogKey = "GS.Trade.EventLog";
            //BaseAddress = "http://localhost:2527/";
            BaseAddress = "http://localhost/WebApi_01/";
            // BaseAddress = "http://81.176.229.34/ApiEventLog/";
            RequestHeader = "application/json";

             ApiAccounts = "api/accounts/";
             ApiTickers = "api/tickers/";
             ApiStrategies = "api/strategies/";

       //     ApiEventLogs = "api/atrategies/";
       //     ApiEventLogItems = "api/eventlogitems/";

          //  IsAsync = false;

           _client = new HttpClient();
            //_client.BaseAddress = new Uri(BaseAddress);

            //// Add an Accept header for JSON format.
            //_client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue(RequestHeader));
        }
         public void Init()
         {
             _client.BaseAddress = new Uri(BaseAddress);

             // Add an Accept header for JSON format.
             _client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue(RequestHeader));

          //   InitEventLog();

         }

        public IAccount Register(IAccount acc)
        {
            var a = new Account
            {
                Alias = acc.Name,
                Code = acc.Code,
                Name = acc.Name,
                TradePlace = acc.TradePlace
            };
            var b = AddElementSync(a, ApiAccounts);
            if (b != null)
            {
                acc.Id = b.Id;
            }
            return acc;
        }
        public Account Register(Account acc)
        {
            var a = new Account
            {
                Alias = acc.Name,
                Code = acc.Code,
                Name = acc.Name,
                TradePlace = acc.TradePlace
            };
            var b = AddElementSync(a, ApiAccounts);
            if (b != null)
            {
                acc.Id = b.Id;
            }
            return acc;
        }

    //    <Ticker>
    // <ID>1</ID>
    //<Code>RIZ3</Code>
    //<ClassCode>SPBFUT</ClassCode>
    //<BaseContract>RTS</BaseContract>
    //<Name>RTSI September,2011</Name>
    //<Key>RTSI_1109</Key>
    //<Symbol>F_RTSI</Symbol>
    //<Decimals>0</Decimals>
    //<FormatF>F0</FormatF>
    //<Format>N0</Format>
    //<FormatAvg>N2</FormatAvg>
    //<FormatM>N2</FormatM>
    //<MinMove>10</MinMove>
    //<From>2011-03-15T10:00:00</From>
    //<To>2011-06-15T18:45:00</To>
    //<IsNeedLoadFromDataBase>false</IsNeedLoadFromDataBase>
    //<LoadMode>2</LoadMode>

        public ITicker Register(ITicker t)
        {
            var ti = new Ticker
            {
                Alias = t.Name,
                Name = t.Name,
                Code = t.Code,
                TradeBoard = t.ClassCode,
                BaseContract = t.BaseContract,

                MinMove = t.MinMove,
                Decimals = t.Decimals
            };
            var b = AddElementSync(ti, ApiTickers);
            if (b != null)
            {
                t.Id = b.Id;
            }
            return t;
        }
        public Ticker Register(Ticker t)
        {
            var b = AddElementSync(t, ApiTickers);
            if (b != null)
            {
                t.Id = b.Id;
            }
            return b;
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
                        element = y.Result.Content.ReadAsAsync<T>().Result;
                        var uri = y.Result.Headers.Location;
                        // Console.WriteLine("Response --- Insert Success Uri= {0}\n {1}", uri.ToString(), elementAdded);
                    }
                });
            await task;
        }
        private async void AddElementAsync2<T>(T element, string apiStr)
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

        private T AddElementSync<T>(T element, string apiStr)
        {
            T e = default(T);
            using (HttpResponseMessage response = _client.PostAsJsonAsync(apiStr, element).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    e = response.Content.ReadAsAsync<T>().Result;
                    //var evlUri = response.Headers.Location;
                    //Console.WriteLine("Insert(Add) Success EventLog Uri= {0}", evlUri.ToString());
                    // return 1;
                }
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return e;
        }

        private T ReadElementSync<T>(string board, string code)
        {
            var requestStr = ApiTickers + @"?board=" + board + @"&code=" + code;
            HttpResponseMessage response = _client.GetAsync(requestStr).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<T>().Result;
            }

            Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            return default(T);
        }
        
        private int PostElementSync<T>(int id, T t)
        {
            var requestStr = ApiTickers + id;
            HttpResponseMessage response = _client.PutAsJsonAsync(requestStr,t).Result;
            return response.IsSuccessStatusCode ? 1 : 0;
        }
        public int UpdateTicker(int id, Ticker t)
        {
            return PostElementSync<Ticker>(id, t);
        }

        public Ticker GetTicker(string board, string code)
        {
             return ReadElementSync<Ticker>(board, code);
        }
    }
}
