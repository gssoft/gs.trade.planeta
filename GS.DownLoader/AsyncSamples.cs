using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GS.Trade.DownLoader
{
    class AsyncSamples
    {
        void DoWithResponse(WebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }

        private void Something()
        {
            //HttpWebRequest request;
            WebRequest request = WebRequest.Create("ssf");

            // init your request...then:
            DoWithResponse(request, (response) =>
            {
                var body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Console.Write(body);
            });
        }
    }
}
