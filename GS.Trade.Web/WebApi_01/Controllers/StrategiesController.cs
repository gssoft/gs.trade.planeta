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
using Newtonsoft.Json;

namespace WebApi_01.Controllers
{
    public class StrategiesController : ApiController
    {
        private DbTradeContext db = new DbTradeContext();

        // GET api/Strategies
        public IEnumerable<Strategy> GetStrategies()
        {
          // var str = db.Strategies.Include(s => s.Account).Include(s => s.Ticker);
          // return str.AsEnumerable();
          return  db.Strategies.AsEnumerable();
        }

        // GET api/Strategies/5
        public Strategy GetStrategy(int id)
        {
          //  Strategy strategy = db.Strategies.Find(id);
            Strategy strategy = db.Strategies.FirstOrDefault();
            if (strategy == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return strategy;
        }

        // PUT api/Strategies/5
        public HttpResponseMessage PutStrategy(int id, Strategy strategy)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != strategy.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(strategy).State = EntityState.Modified;

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

        // POST api/Strategies
        public HttpResponseMessage PostStrategy(Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                db.Strategies.Add(strategy);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, strategy);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = strategy.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Strategies/5
        public HttpResponseMessage DeleteStrategy(int id)
        {
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Strategies.Remove(strategy);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, strategy);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}