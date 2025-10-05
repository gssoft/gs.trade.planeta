using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.TradeContext
{
    public class TradeContextInit
    {
        /*
        public string DdeServerName { get; set; }
        public string ClassCodeToRemoveLogin { get; set; }
        public string LoginToRemove { get; set; }
        */
        public int PositionLastPriceRefreshTimeSec { get; set; }

        public DdeInit DdeInit { get; set; }
        public OrderInit OrderInit { get; set; }
    }
    /*
    public class TradeContextInit2
    {
        public string DdeServerName { get; set; }
        public string ClassCodeToRemoveLogin { get; set; }
        public string LoginToRemove { get; set; }
        public int PositionLastPriceRefreshTimeSec { get; set; }

        public DdeInit DdeInit { get; set; }
        public OrderInit OrderInit { get; set; }
        public TradeContextInit2()
        {
            DdeInit = new DdeInit();
            OrderInit = new OrderInit();

            DdeInit.ServerName = "NewServer";
            OrderInit.ClassCodeToRemoveLogin = "QJSIM";
            OrderInit.LoginToRemove = "OPEN45/";
        }
    }
     * */
    public class DdeInit
    {
        public string ServerName { get; set; }
    }
    public class OrderInit
    {
        public string ClassCodeToRemoveLogin { get; set; }
        public string LoginToRemove { get; set; }
    }
}
