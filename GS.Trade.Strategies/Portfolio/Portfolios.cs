using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Interfaces;

namespace GS.Trade.Strategies.Portfolio
{
    //public interface IPortfolios : IElement2<string, IPortfolioRisk>
    //{
    //    void Refresh();
    //}

    public class Portfolios : Element2<string, IPortfolioRisk,
        Containers5.ListContainer<string, IPortfolioRisk>> , IPortfolios
    {
        public override string Key => Code;

        public Portfolios()
        {
            Name = "Portfolio";
            Code = "PortfolioPosition";

            Collection = new ListContainer<string, IPortfolioRisk>();
        }

        public override IPortfolioRisk Register(IPortfolioRisk p)
        {
            var rp = base.Register(p);
            rp.Parent = this;
            FireChangedEvent("UI.Portfolio","Position","AddOrUpdate", rp.Position);
            return rp;
        }
        public void Refresh()
        {
            foreach (var p in Collection.Items)
            {
                p.Refresh();
            }
        }
        public void SetMaxSideSize(int maxsidesize)
        {
            foreach (var p in Collection.Items)
            {
                p.SetMaxSideSize(maxsidesize);
            }
        }
        public void Register(IStrategy s)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var k = s.RealAccountKey + "@" + s.RealTickerKey;
                var p = Collection.GetByKey(k);
                if (p != null)
                {
                    p.Register(s);
                    s.PortfolioRisk = p;
                    s.EntryPortfolioEnabled = true;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                            m, "Strategy Register Ok", s.Key);
                }
                else
                    Evlm2(EvlResult.FATAL, EvlSubject.INIT, ParentTypeName, TypeName,
                            m, "Strategy Register Failure", s.Key);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
    }
}
