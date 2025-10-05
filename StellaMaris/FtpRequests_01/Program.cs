using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace FtpRequests_01
{
    public class FtpWebRequestGet
    {
        public struct MyStruct
        {
            public string StringData;
            public MyStruct(string stringData)
            {
                this.StringData = stringData;
            }
        }

        static void Main(string[] args)
        {
            // Get the object used to communicate with the server.
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.contoso.com/");

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@"ftp://62.173.145.180/www/stellamaris.su");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            // request.Method = WebRequestMethods.Ftp.ListDirectory;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("user41381", "kPywCImxAZ2z");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            // Stream newS = new MemoryStream();
            // responseStream?.WriteTo1(newS);
             StreamReader reader = new StreamReader(responseStream);


            // StreamReader reader = new StreamReader(newS);

            while (! reader.EndOfStream)
            {
                var str = reader.ReadLine();
                Console.WriteLine(str);
            }

            //Console.WriteLine(reader.ReadToEnd());

            //var lst = responseStream.Deserialize<List<MyStruct>>();
           

            Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);

            // reader.Close();
            response.Close();

            Console.ReadLine();
        }
    }
}
