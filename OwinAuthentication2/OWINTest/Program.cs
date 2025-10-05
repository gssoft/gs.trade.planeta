using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
// using GS.ConsoleAS;

namespace OWINTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // string baseAddress = "http://localhost:4312";
             string baseAddress = "http://localhost:8082";

            Token token = new Token();
            using (var client = new HttpClient())
            {
                // var form = new Dictionary<string, string>  
                //{  
                //    {"grant_type", "password"},  
                //    {"username", "jignesh"},  
                //    {"password", "user123456"} 
                //};
               var form1 = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "gs_trade@mail.ru"},
                   {"password", "8pjAKFrbNU+"}
               };
                //var form2 = new Dictionary<string, string>
                //{
                //    {"grant_type", "password"},
                //    {"username", "gs_order@mail.ru"},
                //    {"password", "x5Aiw3p2M+"}
                //};

                // var urlForm = new FormUrlEncodedContent(form);
                //  Console.WriteLine(urlForm.ToString());

                // ConsoleSync.WriteReadLineT($"Try to get token is Started");

                var tokenResponse = client.PostAsync(baseAddress + "/oauth/token", new FormUrlEncodedContent(form1)).Result;
                //var token = tokenResponse.Content.ReadAsStringAsync().Result;
                token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                if (string.IsNullOrEmpty(token.Error))
                {
                    Console.WriteLine("Token issued is: {0}", token.AccessToken);
                    Console.WriteLine("Token Type: {0}", token.TokenType);
                    Console.WriteLine("Token ExpiresIn: {0}", token.ExpiresIn);
                    Console.WriteLine("Token Refresh: {0}", token.RefreshToken);
                    Console.WriteLine("Token Error: {0}", token.Error);
                }
                else
                {
                    Console.WriteLine("Error : {0}", token.Error);
                }
            }
            Console.WriteLine("Token issued is: {0}", token.AccessToken);
            Console.WriteLine("Token Type: {0}", token.TokenType);
            Console.WriteLine("Token ExpiresIn: {0}", token.ExpiresIn);
            Console.WriteLine("Token Refresh: {0}", token.RefreshToken);
            Console.WriteLine("Token Error: {0}", token.Error);

            // Next Request 
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseAddress);
                //httpClient1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token.AccessToken));
                //HttpResponseMessage response = httpClient.GetAsync("api/TestMethod").Result;
                HttpResponseMessage response = httpClient.GetAsync("api/User").Result;
                if (response.IsSuccessStatusCode)
                {
                    System.Console.WriteLine("Success");
                }
                string message = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("URL responese: " + message);
            }

            Console.Read();
        }
    }
}
