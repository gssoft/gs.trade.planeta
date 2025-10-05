using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;

namespace OwinTest_02
{
    class Program
    {
        // private static string _baseAddress = @"http://localhost:8082";
        private static string _baseAddress = @"http://185.31.161.159:8083";
        
        private static Token _token;
        private static  HttpClient _httpClient;

        public struct ReturnStatus
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public bool ReturnCode { get; set; }
            public ReturnStatus(HttpStatusCode httpStatus, bool retSt)
            {
                HttpStatusCode = httpStatus;
                ReturnCode = retSt;
            }

            public override string ToString()
            {
                return $"HttpStatusCode: {HttpStatusCode}, ReturnCode: {ReturnCode}";
            }
        }

        static void Main(string[] args)
        {
            ConsoleSync.WriteLineT("Start Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            using (var c = new HttpClient())
            {
                c.BaseAddress = new Uri(_baseAddress);
                
                ClearHttpClient();
                ReturnStatus ret;
                try
                {
                    while (true)
                    {
                        ret = GetValue(c);
                        if (ret.HttpStatusCode == HttpStatusCode.Unauthorized)
                        {
                            ClearHttpClient();

                            ret = GetToken(c);
                            if (ret.ReturnCode)
                            {
                                ret = GetValue(c);
                                if (!ret.ReturnCode)
                                {
                                    ConsoleSync.WriteLineT($"Error in GetValue(): {ret}");
                                    // Exit
                                }
                            }
                            else
                            {
                                ConsoleSync.WriteLineT($"Error in GetToken(): {ret}");
                            }
                        }
                        
                        if (_token != null)
                            ConsoleSync.WriteLineT
                                ($"ExpIn: {_token.ExpireDateTime}," +
                                 $" Now: {DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss")}," +
                                 $" Min: {(_token.ExpireDateTime - DateTime.Now).Minutes}" +
                                 $" Sec: {(_token.ExpireDateTime - DateTime.Now).Seconds}" 
                                 + Environment.NewLine);
                        else
                        {
                            ConsoleSync.WriteLineT(
                                $"Token is Null, Now: {DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss")}");
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(60));
                    }
                }
                catch (Exception ex)
                {
                    ConsoleSync.WriteLineT("Exception: {0}", ex.Message);
                }

                //{
                //    ConsoleSync.WriteLineT("Error in GetToken()");
                //}
            }
            ConsoleSync.WriteReadLineT("Finish Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        private static ReturnStatus GetToken(HttpClient client)
        {
            bool ret;
            
            var returnStatus = new ReturnStatus();

            ConsoleSync.WriteLineT("Start Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            var form1 = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", "gs_trade@mail.ru"},
                {"password", "8pjAKFrbNU+"}
            };
            // using (_httpClient = new HttpClient())
            // {

            try
            {
                _httpClient = new HttpClient { BaseAddress = new Uri(_baseAddress) };

                var tokenResponse =
                       _httpClient.PostAsync(_baseAddress + "/oauth/token", new FormUrlEncodedContent(form1)).Result;
                _token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                if (string.IsNullOrEmpty(_token.Error))
                {
                    _token.SetExpireDateTime();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");

                    Console.WriteLine("Token: {0}", _token.AccessToken);
                    Console.WriteLine("Token Type: {0}", _token.TokenType);
                    Console.WriteLine("Token ExpiresIn: {0}", _token.ExpiresIn);
                    Console.WriteLine("Token Refresh: {0}", _token.RefreshToken);
                    Console.WriteLine("Token Expires in: {0}", _token.ExpireDateTime);
                    // Console.WriteLine("Token Error: {0}", _token.Error);

                    // ConsoleSync.WriteLineT($"HttpStatus: {tokenResponse.StatusCode}");

                    returnStatus.HttpStatusCode = tokenResponse.StatusCode;
                    returnStatus.ReturnCode = true;
                }
                else
                {
                    //ConsoleSync.WriteLineT("Token Error : {0}, HttpStatus: {1}",
                    //                            _token.Error, tokenResponse.StatusCode);

                    returnStatus.HttpStatusCode = tokenResponse.StatusCode;
                    returnStatus.ReturnCode = false;

                    ClearHttpClient();
                }
            }
            catch (Exception ex)
            {
                var exs = ex.ExceptionMessageAgg();
                ConsoleSync.WriteLineT($"Exception: {exs}");

                ClearHttpClient();

                returnStatus.HttpStatusCode = HttpStatusCode.ServiceUnavailable;
                returnStatus.ReturnCode = false;
            }
           
            ConsoleSync.WriteLineT("Finish Method {0}, {1}" + Environment.NewLine,
                                System.Reflection.MethodBase.GetCurrentMethod().Name, returnStatus );
            
            return returnStatus;
            //}
        }

        private static ReturnStatus GetValue(HttpClient client)
        {
            ConsoleSync.WriteLineT("Start Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            var ret = new ReturnStatus();
            //if (_token == null)
            if( ! IsAuthorised)
            {
                ClearHttpClient();

                ret.ReturnCode = false;
                ret.HttpStatusCode = HttpStatusCode.Unauthorized;

                ConsoleSync.WriteLineT("Token is Null");
                ConsoleSync.WriteLineT("Finish Method {0} Status: {1}" + Environment.NewLine,
                            System.Reflection.MethodBase.GetCurrentMethod().Name, ret);

                return ret;

            }
            
           //  client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
            //HttpResponseMessage response = httpClient.GetAsync("api/TestMethod").Result;

            try
            {
                HttpResponseMessage response = _httpClient.GetAsync("api/User").Result;

                if (response.IsSuccessStatusCode)
                {
                    var message = response.Content.ReadAsStringAsync().Result;

                    ret.HttpStatusCode = response.StatusCode;
                    ret.ReturnCode = true;

                    ConsoleSync.WriteLineT($"URL responese: {message}");
                    //ConsoleSync.WriteLineT($"StatusCode {ret}");
                    // ConsoleSync.WriteLineT(ret.ToString());
                }
                else
                {

                    var m = response.Content.ReadAsStringAsync().Result;

                    ret.HttpStatusCode = response.StatusCode;
                    ret.ReturnCode = false;

                    ConsoleSync.WriteLineT($"URL responese: {m}");
                    // ConsoleSync.WriteLineT(ret.ToString());
                }
            }
            catch (Exception ex)
            {
                var exs = ex.ExceptionMessageAgg();
                ConsoleSync.WriteLineT($"Exception: {exs}");
                
                ClearHttpClient();

                ret.HttpStatusCode = HttpStatusCode.ServiceUnavailable;
                ret.ReturnCode = false;
                
            }

            ConsoleSync.WriteLineT("Finish Method {0} {1}" + Environment.NewLine,
                            System.Reflection.MethodBase.GetCurrentMethod().Name, ret);

            return ret;
        }

        private static void ClearHttpClient()
        {
            _httpClient?.Dispose();
            _httpClient = null;
            _token = null;
        }

        protected static bool IsReadyToGetData => _httpClient != null && IsAuthorised;
        protected static bool IsAuthorised => _token != null;
    }
}
