using System;
using GS.Web.Api.Services.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest01
{
    [TestClass]
    public class WebApiServiceBaseUnitTest
    {
        WebApiServiceBase webapi;

        [TestInitialize]
        public void Init()
        {
            webapi = new WebApiServiceBase();
        }
        [TestMethod]
        public void Serialization_TestMethod1()
        {
            Assert.IsNotNull(webapi);
            webapi.Serialize();
        }
    }
}
