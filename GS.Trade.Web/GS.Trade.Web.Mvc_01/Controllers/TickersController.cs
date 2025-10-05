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

namespace GS.Trade.Web.Mvc_01.Controllers
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

        // PUT api/Tickers/5
        public HttpResponseMessage PutTicker(int id, Ticker ticker)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != ticker.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(ticker).State = EntityState.Modified;

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

        // POST api/Tickers
        public HttpResponseMessage PostTicker(Ticker ticker)
        {
            if (ModelState.IsValid)
            {
                db.Tickers.Add(ticker);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ticker);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = ticker.Id }));
                return response;
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