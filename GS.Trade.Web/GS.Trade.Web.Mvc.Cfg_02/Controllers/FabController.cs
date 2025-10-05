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

namespace GS.Trade.Web.Mvc.Cfg_02.Controllers
{
    /// <summary>
    /// F = files, a = all file type, b = Binary (octed-stream), Controller
    /// Only for Xml Files. 
    /// Xml to Byte Array and Return aapplication/xml
    /// </summary>

    public class FabController : ApiController
    {
        private readonly ConfigurationContext _db = new ConfigurationContext();

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

            var fileInBytes = System.IO.File.ReadAllBytes(path);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = Request,
                Content = new ByteArrayContent(fileInBytes)
            };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

            return result;
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
            //var path = c.Catalog + @"\" + i.Catalog + @"\" + obj;
            var path = obj;
            var path2 = path;
            //var path2 = c.Catalog + " " + i.Catalog + " " + obj;
            //var xml = XDocument.Load(@"D:/VC/1303/Tests/BackTest01/Strategies/Real/R_BkStandard_Fin_Z735P_SIU5_151014.xml");
            if (!File.Exists(path))
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                    "Get: File Is NOT Found", path2, 1);
                return null;
            }
            //var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");

            var fileInBytes = System.IO.File.ReadAllBytes(path);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = Request,
                Content = new ByteArrayContent(fileInBytes)
            };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

            return result;
        }
        */
        #endregion
        #region Token Support & witout User
        /*
        public HttpResponseMessage Get(long? tkn, string cnf, string item)
        {
            Configuration c;
            Item i;

            var ip = Request.GetClientIpAddress();
            var requestUri = Request.RequestUri.ToString();

            if (!_db.CheckRequest(tkn, cnf, item, requestUri, ip, out c, out i))
                return null;

            string path, path2 = string.Empty;
            try
            {
                path2 = c.Catalog + " " + i.Catalog + " " + i.Obj;

                path = Path.Combine(c.Catalog, i.Catalog);
                path = Path.Combine(path, i.Obj);

                if (!File.Exists(path))
                {
                    _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                        "Get: File Is NOT Found", path2, 1);
                    return null;
                }
                // var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");

                var fileInBytes = System.IO.File.ReadAllBytes(path);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = Request,
                    Content = new ByteArrayContent(fileInBytes)
                };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get", path2, 1);

                return result;
            }
            catch (Exception ex)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                                                      "GetObject Exception: " + ex.Message, path2, 1);
                return null;
            }
        }
        public HttpResponseMessage Get(long? tkn, string cnf, string item, string obj)
        {
            Configuration c;
            Item i;

            var ip = Request.GetClientIpAddress();
            var requestUri = Request.RequestUri.ToString();

            if (!_db.CheckRequest(tkn, cnf, item, requestUri, ip, out c, out i))
                return null;

            string path, path2 = string.Empty;
            try
            {
                path2 = c.Catalog + " " + i.Catalog + " " + obj;

                path = Path.Combine(c.Catalog, i.Catalog);
                path = Path.Combine(path, obj);

                if (!File.Exists(path))
                {
                    _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                        "Get: File Is NOT Found", path2, 1);
                    return null;
                }
                //var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");

                var fileInBytes = System.IO.File.ReadAllBytes(path);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = Request,
                    Content = new ByteArrayContent(fileInBytes)
                };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Success, "Get:", path2, 1);

                return result;
            }
            catch (Exception ex)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, OperResultEnum.Failure,
                                                       "GetObject Exception: " + ex.Message, path2, 1);
                return null;
            }
        }
        */
        #endregion
        #region Token & User Support
        public HttpResponseMessage Get(long? tkn, string cnf, string item, string dm, string u)
        {
            Configuration c;
            Item i;

            var ip = Request.GetClientIpAddress();
            var requestUri = Request.RequestUri.ToString();

            if (!_db.CheckRequest(tkn, cnf, item, requestUri, ip, dm, u, out c, out i))
                return null;

            string path, path2 = string.Empty;
            try
            {
                path2 = c.Catalog + " " + i.Catalog + " " + i.Obj;

                path = Path.Combine(c.Catalog, i.Catalog);
                path = Path.Combine(path, i.Obj);

                if (!File.Exists(path))
                {
                    _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Failure,
                        "Get: File Is NOT Found", path2, 1);
                    return null;
                }
                // var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");

                var fileInBytes = System.IO.File.ReadAllBytes(path);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = Request,
                    Content = new ByteArrayContent(fileInBytes)
                };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Success, "Get", path2, 1);

                return result;
            }
            catch (Exception ex)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Failure,
                                                      "GetObject Exception: " + ex.Message, path2, 1);
                return null;
            }
        }
        public HttpResponseMessage Get(long? tkn, string cnf, string item, string obj, string dm, string u)
        {
            Configuration c;
            Item i;

            var ip = Request.GetClientIpAddress();
            var requestUri = Request.RequestUri.ToString();

            if (!_db.CheckRequest(tkn, cnf, item, requestUri, ip, dm, u, out c, out i))
                return null;

            string path, path2 = string.Empty;
            try
            {
                path2 = c.Catalog + " " + i.Catalog + " " + obj;

                path = Path.Combine(c.Catalog, i.Catalog);
                path = Path.Combine(path, obj);

                if (!File.Exists(path))
                {
                    _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Failure,
                        "Get: File Is NOT Found", path2, 1);
                    return null;
                }
                //var xml = XDocument.Load(@"D:\VC\1303\Tests\BackTest01\Strategies\Real\R_BkStandard_Fin_Z735P_SIU5_151014.xml");

                var fileInBytes = System.IO.File.ReadAllBytes(path);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = Request,
                    Content = new ByteArrayContent(fileInBytes)
                };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Success, "Get:", path2, 1);

                return result;
            }
            catch (Exception ex)
            {
                _db.AddTransaction(c, i, Request.RequestUri.ToString(), ip, dm, u, OperResultEnum.Failure,
                                                       "GetObject Exception: " + ex.Message, path2, 1);
                return null;
            }
        }
        #endregion
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
