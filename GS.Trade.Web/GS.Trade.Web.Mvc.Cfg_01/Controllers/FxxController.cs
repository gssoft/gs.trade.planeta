using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml.Linq;
using GS.DataBase.Configuration.Dal;
using GS.DataBase.Configuration.Model;

namespace GS.Trade.Web.Mvc.Cfg_01.Controllers
{

    /// <summary>
    /// F = files, x = xml, x = XContent, Controller
    /// Only for Xml files.
    /// Return XContent
    /// Xml to Stream and Return aapplication/xml 
    /// </summary>
    public class FxxController : ApiController
    {
        private readonly ConfigurationContext _db = new ConfigurationContext();

        [HttpGet]
        public HttpResponseMessage Index()
        {
            //var xml =
            //    XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            return new HttpResponseMessage()
            {
                RequestMessage = Request,
                // Content = new XContent(xml)
            };
        }

        //[HttpGet]
        //public HttpResponseMessage Get(string cnf, string item, string obj)
        //{
        //    var i = _db.IsItemEnabled(cnf, item);
        //    if(i == null)
        //        return null;
        //    var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
        //    return new HttpResponseMessage()
        //    {
        //        RequestMessage = Request,
        //        Content = new XContent(xml)
        //    };
        //}
        [HttpGet]
        //public HttpResponseMessage Get(string cnf, string item, string obj)
        //{
        //    var ip = Request.GetClientIpAddress();

        //    var c = _db.IsConfigurationEnabled(cnf);
        //    if (c == null)
        //    {
        //        //_db.AddTransaction(Request.RequestUri.ToString(), ip, i, OperResultEnum.Failure, "Get", "Cnf UnAvailable: " + cnf, 1);
        //        return null;
        //    }
        //    var i = _db.IsItemEnabled(c, item);
        //    if (i == null)
        //    {
        //        //_db.AddTransaction(Request.RequestUri.ToString(), ip, i, OperResultEnum.Failure, "Get", "Item UnAvailable: " + item , 1);
        //        return null;
        //    }
        //    var path = c.Catalog + i.Catalog + @"/" + obj; 
        //    //var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
        //    var xml = XDocument.Load(path);

        //   _db.AddTransaction(Request.RequestUri.ToString(), ip,  i, OperResultEnum.Success, "Get", path, 1);

        //    return new HttpResponseMessage()
        //    {
        //        RequestMessage = Request,
        //        Content = new XContent(xml)
        //    };
        //}

        #region Methods WithOut Token
        /*
        public HttpResponseMessage Get(string cnf, string item)
        {
            var ip = Request.GetClientIpAddress();
            Item i;
            var c = _db.GetConfiguration(cnf);
            if (c == null)
            {
                _db.AddTransaction(null, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Wrong Configuration: " + cnf, cnf, 1);
                return null;
            }
            if (!c.IsEnabled)
            {
                i = _db.GetItem(c, item);
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Configuration Disabled: " + cnf, cnf + " " + item, 1);
                return null;
            }
            i = _db.GetItem(c, item);
            if (i == null)
            {
                _db.AddTransaction(c, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Wrong Item: " + item, cnf + " " + item, 1);
                return null;
            }
            if (!i.IsEnabled)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Item Disabled: " + item, cnf + " " + item, 1);
                return null;
            }
            var path = c.Catalog + @"\" + i.Catalog + @"\" + i.Obj;
            var path2 = c.Catalog + " " + i.Catalog + " " + i.Obj;
            //var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            if (!File.Exists(path))
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: File Is NOT Found", path2, 1);
                return null;
            }
            // var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            var xml = XDocument.Load(path);

            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

            return new HttpResponseMessage()
            {
                RequestMessage = Request,
                Content = new XContent(xml)
            };
        }

        public HttpResponseMessage Get(string cnf, string item, string obj)
        {
            var ip = Request.GetClientIpAddress();
            Item i;
            var c = _db.GetConfiguration(cnf);
            if (c == null)
            {
                _db.AddTransaction(null, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Wrong Configuration: " + cnf, cnf, 1);
                return null;
            }
            if (!c.IsEnabled)
            {
                i = _db.GetItem(c, item);
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Configuration Disabled: " + cnf, cnf + " " + item, 1);
                return null;
            }
            i = _db.GetItem(c, item);
            if (i == null)
            {
                _db.AddTransaction(c, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Wrong Item: " + item, cnf + " " + item, 1);
                return null;
            }
            if (!i.IsEnabled)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: Item Disabled: " + item, cnf + " " + item, 1);
                return null;
            }
            var path = c.Catalog + @"\" + i.Catalog + @"\" + obj;
            var path2 = c.Catalog + " " + i.Catalog + " " + obj;
            //var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            if (!File.Exists(path))
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: File Is NOT Found", path2, 1);
                return null;
            }
            //var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            var xml = XDocument.Load(path);

            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

            return new HttpResponseMessage()
            {
                RequestMessage = Request,
                Content = new XContent(xml)
            };
        }
        */
        #endregion



        //    public HttpResponseMessage Get(string cnf, string item, string obj, int rt)
        //    {
        //        var ip = Request.GetClientIpAddress();
        //        Item i;
        //        var c = _db.GetConfiguration(cnf);
        //        if (c == null)
        //        {
        //            _db.AddTransaction(null, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                "Get: Wrong Configuration: " + cnf, cnf, 1);
        //            return null;
        //        }
        //        if (!c.IsEnabled)
        //        {
        //            i = _db.GetItem(c, item);
        //            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                "Get: Configuration Disabled: " + cnf, cnf + " " + item, 1);
        //            return null;
        //        }
        //        i = _db.GetItem(c, item);
        //        if (i == null)
        //        {
        //            _db.AddTransaction(c, null, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                "Get: Wrong Item: " + item, cnf + " " + item, 1);
        //            return null;
        //        }
        //        if (!i.IsEnabled)
        //        {
        //            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                "Get: Item Disabled: " + item, cnf + " " + item, 1);
        //            return null;
        //        }
        //        if (rt == 1)
        //        {
        //            var path = c.Catalog + @"\" + i.Catalog + @"\" + obj;
        //            var path2 = c.Catalog + " " + i.Catalog + " " + obj;
        //            //var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
        //            if (!File.Exists(path))
        //            {
        //                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                    "Get: File Is NOT Found", path2, 1);
        //                return null;
        //            }
        //            //var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");
        //            var xml = XDocument.Load(path);

        //            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

        //            return new HttpResponseMessage()
        //            {
        //                RequestMessage = Request,
        //                Content = new XContent(xml)
        //            };
        //        }
        //        if (rt == 2)
        //        {
        //            var path = obj;
        //            var path2 = path;

        //            if (!File.Exists(path))
        //            {
        //                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
        //                    "Get: File Is NOT Found", path2, 1);
        //                return null;
        //            }
        //            var fileInBytes = System.IO.File.ReadAllBytes(path);

        //            var result = new HttpResponseMessage(HttpStatusCode.OK)
        //            {
        //                Content = new ByteArrayContent(fileInBytes)
        //            };
        //            result.Content.Headers.ContentType =
        //                new MediaTypeHeaderValue("application/octet-stream");

        //            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

        //            return result;
        //        }
        //        return null;
        //    }        
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
