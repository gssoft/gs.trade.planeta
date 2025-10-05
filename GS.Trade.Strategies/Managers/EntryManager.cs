using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Containers;

namespace GS.Trade.Strategies.Managers
{
    //public class EntryManagers : SetContainer<string, IEntryManager>
    //{
    //    public void Init()
    //    {
    //        foreach(var e in Items)
    //            e.Init();
    //    }

    //    public IEntryManager Register(IEntryManager p)
    //    {
    //        return AddNew(p);
    //    }
    //}
    //public class EntryManager : ListContainer<string, IStrategy>, IEntryManager
    //{
    //    public string Code { get; set; }
    //    public string Name { get; set; }

    //    public string Key
    //    {
    //        get { return Code; }
    //    }

    //    public IStrategy Register(IStrategy strategy)
    //    {
    //        return AddNew(strategy);
    //    }
    //    public void PositionChanged(IPosition oldp, IPosition newp, PositionChangedEnum e)
    //    {
    //        Policy01(newp);
    //    }

    //    public void Init()
    //    {
    //        foreach (var st in Items) //.Where(s => s.Position.IsNeutral))
    //        {
    //            st.LongEnabled = false;
    //            st.ShortEnabled = false;
    //        }

    //        var sta = Items.Where(s=>s.Position.IsNeutral).OrderBy(a=>a.Position.PositionTotal.LastTradeDT.ToString("u")).FirstOrDefault();
    //        if (sta == null) return;

    //      //  var strDt = sta.Position.PositionTotal.LastTradeDT.ToString("u");

    //        sta.ShortEnabled = true;
    //        sta.LongEnabled = true;

    //        sta.ShortEntryLevel = float.MinValue;
    //        sta.LongEntryLevel = float.MaxValue;
    //    }

    //    private void Policy01(IPosition newp)
    //    {
    //        Init();
    //        var stra = Items.Where(s => s.Position.IsNeutral).OrderBy(a => a.Position.PositionTotal.LastTradeDT.ToString("u")).FirstOrDefault();
    //        if (stra == null)
    //            return;
    //        if (newp.IsLong)
    //        {
    //            stra.LongEntryLevel = (float) newp.Price1 - stra.Volatility*2;
    //            stra.ShortEntryLevel = float.MinValue;
    //            stra.SkipTheTick(0);
    //        }
    //        else if (newp.IsShort)
    //        {
    //            stra.LongEntryLevel = float.MaxValue;
    //            stra.ShortEntryLevel = (float)newp.Price1 + stra.Volatility * 2;
    //            stra.SkipTheTick(0);
    //        }
    //        else if (newp.IsNeutral)
    //        {
    //            stra.ShortEntryLevel = float.MinValue;
    //            stra.LongEntryLevel = float.MaxValue;
    //            stra.SkipTheTick(0);
    //        }
    //    }

    //}
}
