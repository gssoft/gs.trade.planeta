using System;
using System.Runtime.Remoting.Contexts;
using GS.Web.Api.Service01;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest01
{
    [TestClass]
    public class UnitTest1
    {
        WebApiService _webApi;
        [TestInitialize]
        public void SetUp()
        {
            _webApi = new WebApiService();
        }
        [TestMethod]
        public void WebApi_Start_Test()
        {
            Assert.IsNotNull(_webApi);
            _webApi.Start();
        }
        [TestMethod]
        public void WebApi_Stop_Test()
        {
            Assert.IsNotNull(_webApi);
            _webApi.Stop();
        }

        [TestMethod] public void Uri_Buider_Test()
        {
            var sheme = "http";
            var host = "localhost";
            var port = 8082;
            var urib = new UriBuilder(sheme, host, 8082);
            var uri = urib.ToString();
            Console.WriteLine(uri);
            Assert.AreEqual("http://localhost:8082/", uri);
        }

    }
}
