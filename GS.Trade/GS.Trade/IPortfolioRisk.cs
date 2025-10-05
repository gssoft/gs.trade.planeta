using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Interfaces;

namespace GS.Trade
{
    //public interface IPortfolioRisk : IElement2<string, IStrategy>, IHaveInit
    //{
    //    //IEnumerable<IStrategy> Collection { get; }
    //    //IEnumerable<IStrategy> Collection { get; }
    //    void Init();
    //    void Refresh();
    //    IPosition2 Position { get; }

    //    bool IsLongEnabled { get; }
    //    bool IsShortEnabled { get; }

    //    long MaxOneSidePositionSize { get; }
    //    long MaxSideCount { get; }

    //    long LongsCount { get; }
    //    long ShortsCount { get; }
    //}
    public interface IPortfolios : IElement2<string, IPortfolioRisk>
    {
        void Refresh();
        void SetMaxSideSize(int maxsidesize);
        void Register(IStrategy s);
    }

    public interface IPortfolioRisk : IElement2<string, IStrategy>, IHaveInit
    {
        //IEnumerable<IStrategy> Collection { get; }
        //IEnumerable<IStrategy> Collection { get; }
        void Init();
        void Refresh();
        IPosition2 Position { get; }

        long Count { get; }

        bool IsLongEnabled { get; }
        bool IsShortEnabled { get; }

       // long MaxOneSidePositionSize { get; }
        float MaxSideInitValue { get; set; }
        int MaxSideCount { get; }

        long LongsCount { get; }
        long ShortsCount { get; }

        //int LongRequests { get; set; }
        //int ShortRequests { get; set; }
        bool IsSellRequestEnabled(IStrategy strategy);
        bool IsBuyRequestEnabled(IStrategy strategy);
        bool IsBuySellRequestEnabled(IStrategy strategy);

        bool IsSellRequestEnabled();
        bool IsBuyRequestEnabled();

        int BuyRequestCount { get; }
        int SellRequestCount { get; }

        void ClearBuyRequest(IStrategy s);
        void ClearSellRequest(IStrategy s);
        void ClearBuySellRequest(IStrategy s);
        void SkipTheTickToOthers(string key);
        void SkipTheTickToOthers(IStrategy str);
        void SkipTheTickToOthers(IStrategy str, int skipCount);
        void SkipTheTickToOthers(IStrategy str, int skipCount, bool isOpened, bool isSameSide);
        string ShortDescription { get; }
        string BuyRequestShortInfo { get; }
        string SellRequestShortInfo { get; }
        void SetMaxSideSize(int maxsidesize);
    }
}
