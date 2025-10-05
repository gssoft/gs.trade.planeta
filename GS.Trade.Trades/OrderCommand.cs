using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GS.Interfaces;

//namespace sg_Trades
namespace GS.Trade.Trades
{
        public class OrderCommand
        {
            public const string TagAction = "ACTION";        
            public const string TagTransId = "TRANS_ID";
            public const string TagAccount = "ACCOUNT";
            public const string TagClientCode = "CLIENT_CODE";
            public const string TagType = "TYPE";
            public const string TagLimitOrderKey = "ORDER_KEY";
            public const string TagStopOrderKey = "STOP_ORDER_KEY";

            public const string TagClassCode = "CLASSCODE";
            public const string TagTicker = "SECCODE";

            public const string TagOperation = "OPERATION";
            public const string TagPrice = "PRICE";
            public const string TagLimitPrice = "LIMITPRICE";
            public const string TagStopPrice = "STOPPRICE";
            public const string TagQuantity = "QUANTITY";
            public const string TagBaseContract = "BASE_CONTRACT";

            public const string TagStrategy = "Strategy";

            public const string NewOrder = "NEW_ORDER";
            public const string NewMarketOrder = "NEW_MARKET_ORDER";
            public const string NewLimitOrder = "NEW_LIMIT_ORDER";
            public const string NewStopOrder = "NEW_STOP_ORDER";

            public const string KillOrder = "KILL_ORDER";
            public const string KillLimitOrder = "KILL_LIMIT_ORDER";
            public const string KillStopOrder = "KILL_STOP_ORDER";

            public const string KillAllOrders = "KILL_ALL_ORDERS";
            public const string KillAllLimitOrders = "KILL_ALL_LIMIT_ORDERS";
            public const string KillAllStopOrders = "KILL_ALL_STOP_ORDERS";

            public const string KillAllFuturesOrders = "KILL_ALL_FUTURES_ORDERS";
            public const string KillAllFuturesLimitOrders = "KILL_ALL_ORDERS";
            public const string KillAllFuturesStopOrders = "KILL_ALL_STOP_ORDERS";

            public const string MoveOrders = "MOVE_ORDERS";
        

        public enum OrderActionEnum : short 
        {   
            ActionNotSupported,
            NewMarketOrder, NewLimitOrder, NewStopOrder,
            KillLimitOrder, KillStopOrder, KillAllLimitOrders, KillAllStopOrders, KillAllOrders,
            KillAllFuturesLimitOrders, KillAllFuturesStopOrders, KillAllFuturesOrders, 
            MoveOrders
        }

            private IEventLog _eventLog;

            public OrderActionEnum OrderAction { get; set; }
            public ulong TransId { get; set; }
            public string ClassCode { get; set; }
            public string Account { get; set; }
            public string Strategy { get; set; }
            public string ClientCode { get; set; }

            public string Ticker { get; set; }
            public string BaseContract { get; set; }

            public OrderOperationEnum Operation { get; set; }
            public OrderTypeEnum OrderType { get; set; }
            public double Price { get; set; }
            public double LimitPrice { get; set; }
            public double StopPrice { get; set; }
            public long Quantity { get; set; }

            public long LimitOrderNumber { get; set; }
            public long StopOrderNumber { get; set; }
    
            //public string OperationString { get { return  (Operation == +1 ? "B" : Operation == -1 ? "S" : "Unknown"); } }

            public override string ToString()
            {
                return String.Format(
                    "OrdAct={0},TransId={1},ClassCode={2},Acc={3},Strat={4},ClCode={5},Sec={6},BaseCntr={7},Oper={8},OrdType={9},Qnty={10},Pr={11},PrLim={12},PrStop={13},LimNumb={14},StopNumb={15}",
                    OrderAction, TransId, ClassCode, Account, Strategy, ClientCode, Ticker, BaseContract, Operation, OrderType, Quantity, Price, LimitPrice, StopPrice, LimitOrderNumber, StopOrderNumber);
            }

            public OrderCommand()
            {
            }
            public void Init(IEventLog evl)
            {
                _eventLog = evl;
            }          
            public void SetOrderCommand(XElement xe)
            {
                foreach (var xAttribute in xe.Attributes())
                {
                    switch (xAttribute.Name.ToString())
                    {
                        case TagAction:
                            OrderActionToEnum(xAttribute.Value);
                            break;
                        case TagTransId:
                            TransId = ulong.Parse(xAttribute.Value);
                            break;
                        case TagAccount:
                            Account = xAttribute.Value;
                            break;
                        case TagStrategy:
                        case TagClientCode:
                            ClientCode = xAttribute.Value;
                            Strategy = ClientCode;
                            break;
                        case TagLimitOrderKey:
                            LimitOrderNumber = long.Parse(xAttribute.Value);
                            break;
                        case TagStopOrderKey:
                            StopOrderNumber = long.Parse(xAttribute.Value);
                            break;
                        case TagClassCode:
                            ClassCode = xAttribute.Value;
                            break;
                        case TagTicker:
                            Ticker = xAttribute.Value;
                            break;
                        case TagOperation:
                            Operation = (xAttribute.Value == "B" ? OrderOperationEnum.Buy : xAttribute.Value == "S" ? OrderOperationEnum.Sell : OrderOperationEnum.Unknown);
                            break;
                        case TagPrice: 
                        case TagLimitPrice:
                            LimitPrice = double.Parse(xAttribute.Value);
                            break;
                        case TagStopPrice:
                            StopPrice = double.Parse(xAttribute.Value);
                            break;
                        case TagQuantity:
                            Quantity = long.Parse(xAttribute.Value);
                            break;
                        case TagBaseContract:
                            BaseContract = xAttribute.Value;
                            break;
                    }
                }
            }
            private void OrderActionToEnum(string orderAction )
            {
                switch( orderAction )
                {
                    case NewMarketOrder:
                        OrderAction = OrderActionEnum.NewMarketOrder;
                        OrderType = OrderTypeEnum.Market;
                        break;
                    case NewOrder:
                    case NewLimitOrder:
                        OrderAction = OrderActionEnum.NewLimitOrder;
                        OrderType = OrderTypeEnum.Limit;
                        break; 
                    case NewStopOrder:
                        OrderAction = OrderActionEnum.NewStopOrder;
                        OrderType = OrderTypeEnum.StopLimit;
                        break;
                    case KillOrder:
                    case KillLimitOrder:
                        OrderAction = OrderActionEnum.KillLimitOrder; 
                        break;
                    case KillStopOrder:
                        OrderAction = OrderActionEnum.KillStopOrder; 
                        break; 
                    case KillAllOrders:
                        OrderAction = OrderActionEnum.KillAllOrders; 
                        break;
                    case KillAllLimitOrders:
                        OrderAction = OrderActionEnum.KillAllLimitOrders; 
                        break;
                    case KillAllStopOrders:
                        OrderAction = OrderActionEnum.KillAllStopOrders; 
                        break;
                    case KillAllFuturesOrders:
                        OrderAction = OrderActionEnum.KillAllFuturesOrders; 
                        break;
                    case MoveOrders:
                        OrderAction = OrderActionEnum.MoveOrders; 
                        break;
                    default:
                        OrderAction = OrderActionEnum.ActionNotSupported; 
                        break;
                }
            }
            public bool CheckOrder()
            {
                if (
                    OrderAction <= 0 ||
                    TransId <= 0 ||
                    string.IsNullOrWhiteSpace(Account)  ||
                    string.IsNullOrWhiteSpace(ClassCode) ||
                    string.IsNullOrWhiteSpace(Strategy)
                    )
                {
                    _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "OrderCommand Check", "OrderCommand Check",
                                "Invalid Action,TransID,Account,Strategy", this.ToString(),"");
                    return false;
                }
                else
                {
                    switch (OrderAction)
                    {
                        case OrderActionEnum.NewLimitOrder:
                            if (
                                (Operation == OrderOperationEnum.Unknown) ||
                                ( String.IsNullOrWhiteSpace(Ticker)) ||
                                LimitPrice <= 0 ||
                                Quantity <= 0
                                )
                            {
                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "OrderCommand Check", "OrderCommand Check",
                                                  "Invalid Operation,LimitPrice,Quantity", this.ToString(), "");
                                return false;
                            }
                            break;

                        case OrderActionEnum.NewStopOrder:
                            if (
                                (Operation == OrderOperationEnum.Unknown) ||
                                (String.IsNullOrWhiteSpace(Ticker)) ||
                                StopPrice <= 0 ||
                                Quantity <= 0
                                )
                            {
                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "OrderCommand Check", "OrderCommand Check",
                                                  "Invalid Operation,StopPrice,Quantity", this.ToString(), "");
                                return false;
                            }
                            break;

                        case OrderActionEnum.KillAllFuturesOrders:
                        case OrderActionEnum.KillAllFuturesLimitOrders:
                        case OrderActionEnum.KillAllFuturesStopOrders:
                            if (string.IsNullOrWhiteSpace(BaseContract))
                            {

                                _eventLog.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "OrderCommand Check", "OrderCommand Check",
                                                  "Invalid or Empty BaseContract", this.ToString(), "");
                                return false;
                            }
                            break;
                    }
                }
                return true;
            }

            public string ToQuikOrderString()
            {
                switch (OrderAction)
                {
                    //ACCOUNT=SPBFUT00S98;CLIENT_CODE=135;TYPE=L;TRANS_ID=420120901;CLASSCODE=SPBFUT;SECCODE=RIM1;ACTION=NEW_ORDER;OPERATION=B;PRICE=199310;QUANTITY=1;
                    case OrderActionEnum.NewLimitOrder:
                        return String.Format("{0}={1};{2}={3};{4}={5};{6}={7};{8}={9};{10}={11};{12}={13};{14}={15};{16}={17:N0};{18}={19};",
                                              TagTransId, TransId, TagAccount, Account, TagClientCode, Strategy, TagType, OrderType,
                                              TagClassCode, ClassCode, TagTicker, Ticker, TagAction, OrderAction,
                                              TagOperation, Operation, TagLimitPrice, Price, TagQuantity, Quantity);

                    //CLASSCODE=EQBR; SECCODE=RU0009024277; TRANS_ID=5; ACTION=KILL_ORDER; ORDER_KEY=503983;
                    case OrderActionEnum.KillLimitOrder:
                        return String.Format(
                            "{0}={1};{2}={3};{4}={5};{6}={7};",
                                              TagTransId, TransId, TagClassCode, ClassCode, TagAction, OrderAction, TagLimitOrderKey, LimitOrderNumber);

                    // ACTION» = «KILL_STOP_ORDER»
                    case OrderActionEnum.KillStopOrder:
                        return String.Format(
                            "{0}={1};{2}={3};{4}={5};{6}={7};",
                                              TagTransId, TransId, TagClassCode, ClassCode, TagAction, OrderAction, TagStopOrderKey, StopOrderNumber);

                    // ACCOUNT=SPBFUT00S98;TRANS_ID=420120900;CLASSCODE=SPBFUT;SECCODE=RIM1;BASE_CONTRACT=RTS;ACTION=KILL_ALL_FUTURES_ORDERS;
                    case OrderActionEnum.KillAllFuturesLimitOrders:
                        return String.Format("{0}={1};{2}={3};{4}={5};{6}={7};{8}={9};",
                                              TagTransId, TransId, TagAccount, Account, TagClassCode, ClassCode,
                                              TagBaseContract, BaseContract, TagAction, OrderAction);

                    default:
                        return "";
                }
            }
            
        }
}
