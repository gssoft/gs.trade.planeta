using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Exceptions;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.DataBase.Dal;
using GS.Trade.DataBase.EFRepositary;
using GS.Trade.DataBase.Model;
using GS.Trade.Queues;
using GS.Trade.Storage2;

namespace GS.Trade.DataBase.Storage
{
    public class ExceptionDbRepository : 
            TradeBaseRepository3<string, IGSException, Model.GSException, IGSExceptionDb>,  
            IEntityRepository<IGSExceptionDb, IGSException> //IExceptionRepository
    {
        public override string Key { get { return Code; } }

        //public ExceptionDbRepository()
        //{
        //    TradeEntityQueue = new TradeEntityQueue<IGSException>();
        //}

        public override void Init(IEventLog eventLog)
        {
            base.Init(eventLog);

            DbTradeContext = new DbTradeContext(DataBaseName);
            Repository = new ExceptionEFRepository(DbTradeContext);
        }
        
        protected override bool AddVal(IGSException ex)
        {
            //try
            //{
                var em = new Model.GSException
                {
                    Key = ex.Key,

                    DateTime = ex.DateTime.MinValueToSql(),
                    Source = ex.Source,
                    ObjType = ex.ObjType,
                    Operation = ex.Operation,
                    ObjStr = ex.ObjStr,

                    Message = ex.Message,
                    SourceExc = ex.SourceExc,
                    ExcType = ex.ExcType,
                    TargetSite = ex.TargetSite
                };

                Repository.Add(em);
                DbTradeContext.SaveChanges();

                ex.Id = em.Id;
                return true;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ex.GetType().ToString(), "Add(Exception):", ex.ToString(), e);
            //                                    // e.Message, e.Source, e.GetType().ToString(), e.TargetSite.ToString());
            //    throw;
            //}
        }
        protected override IGSException Update(IGSException ve, Model.GSException vi) // AddOrGet
        {
            ve.Id = vi.Id;

            ve.DateTime = vi.DateTime;
            ve.Source = vi.Source;
            ve.ObjType = vi.ObjType;
            ve.Operation = vi.Operation;

            ve.Message = vi.Message;
            ve.SourceExc = vi.SourceExc;
            ve.ExcType = vi.ExcType;
            ve.TargetSite = vi.TargetSite;

            return ve;
        }

        protected override bool Update(Model.GSException vi, IGSException ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve","ExceptionRepository.Update(vi,ve);" +"GSException==Null");

                vi.DateTime = ve.DateTime.MinValueToSql();
                vi.Source = ve.Source;
                vi.ObjType = ve.ObjType;
                ve.Operation = ve.Operation;

                vi.Message = ve.Message;
                vi.SourceExc = ve.SourceExc;
                vi.ExcType = ve.ExcType;
                vi.TargetSite = ve.TargetSite;

                Repository.Update(vi);
                DbTradeContext.SaveChanges();

                //if (UIEnabled)
                //    FireStorageChangedEvent("UI.Accounts", "Account", "Update", ve);

                if (EvlEnabled)
                    Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                            FullName, Code, "Update(Model.GSException)", ve.Message, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "GSException" : ve.GetType().ToString(), "Update(GSException)",
                    ve == null ? "GSException" : ve.ToString(), e);
                                      
                throw;
            }
            return true;
        }

    }
}
