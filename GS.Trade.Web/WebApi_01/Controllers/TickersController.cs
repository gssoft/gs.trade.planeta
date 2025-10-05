using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GS.Trade.DataBase.Model;
using GS.Trade.DataBase.Dal;

namespace WebApi_01.Controllers
{
    public class TickersController : ApiController
    {
        private DbTradeContext db = new DbTradeContext();

        // GET api/Tickers
        public IEnumerable<Ticker> GetTickers()
        {
            return db.Tickers.AsEnumerable();
        }
       

        // GET api/Tickers/5
        public Ticker GetTicker(int id)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return ticker;
        }

        public HttpResponseMessage GetTicker(string board, string code)
        {
            var ticker = db.Tickers
                .FirstOrDefault(t => t.TradeBoard.Trim().ToUpper() == board.Trim().ToUpper()  
                                  && t.Code.Trim().ToUpper() ==  code.Trim().ToUpper());
            if (ticker == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Found, ticker);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = ticker.Id }));
            return response;
        }

        // PUT api/Tickers/5
        public HttpResponseMessage PutTicker(int id, Ticker ticker)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            Ticker t = db.Tickers.Find(id);
            if (t == null || ticker == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            t.CopyFrom(ticker);

            t.Modified = DateTime.Now;

            db.Entry(t).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //public HttpResponseMessage PutTicker(int id, Ticker ticker)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != ticker.Id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(ticker).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        // POST api/Tickers
        public HttpResponseMessage PostTicker(Ticker ticker)
        {
            //var t = db.Tickers.FirstOrDefault(a => a.Code == ticker.Code);
            //if (t != null)
            //{
            //    if (ticker.Id == 0)
            //    {
            //        ticker.Id = t.Id;
            //    }
            //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Found, ticker);
            //    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = ticker.Id }));
            //    return response;
            //}
            if (ModelState.IsValid)
            {
                //ticker.LaunchDate = DateTime.Now.Date;
                //ticker.ExpireDate = DateTime.Now.Date;
                ticker.Modified = DateTime.Now;
                ticker.Created = DateTime.Now;

                db.Tickers.Add(ticker);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ticker);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = ticker.Id }));
                return response;
                // return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Tickers/5
        public HttpResponseMessage DeleteTicker(int id)
        {
            Ticker ticker = db.Tickers.Find(id);
            if (ticker == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Tickers.Remove(ticker);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ticker);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}