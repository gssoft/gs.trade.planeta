using System;

namespace GS.Trade.EntityWebClient.Dto
{
    class OrderDto
    {
        public long TimeStamp { get; set; }
        /// <summary>
        /// Получить время ордера
        /// </summary>
        /// <returns>время ордера</returns>
        public DateTime GetDateTime()
        {
            return new DateTime(TimeStamp);
        }
        /// <summary>
        /// Идентификатор
        /// </summary>

        public string ClOrdID { get; set; }
        /// <summary>
        /// Номер в торговой системе
        /// </summary>

        public string OrderID { get; set; }
        /// <summary>
        /// Дополнительный номер в торговой системе
        /// </summary>

        public long SubOrderID { get; set; }
        /// <summary>
        /// Символ инструмента
        /// </summary>

        public string Symbol { get; set; }
        /// <summary>
        /// Биржевой код инструмента, используемый в системе исполнения
        /// </summary>

        public string SecurityID { get; set; }
        /// <summary>
        /// Биржевой код класса инструмента
        /// </summary>

        public string SecurityIDSource { get; set; }
        /// <summary>
        /// Тип инструмента - фьючерз/опцион/акция/облигация
        /// </summary>

        public ESecurityType SecurityType { get; set; }
        /// <summary>
        /// CFI код инструмента
        /// </summary>

        public string CFICode { get; set; }
        /// <summary>
        /// Направление
        /// </summary>

        public ESide Side { get; set; }
        /// <summary>
        /// Цена
        /// </summary>

        public double Price { get; set; }
        /// <summary>
        /// Количество
        /// </summary>

        public double Qty { get; set; }
        /// <summary>
        /// Счет
        /// </summary>

        public string Account { get; set; }
        /// <summary>
        /// Код клиента
        /// </summary>

        public string ClientID { get; set; }
        /// <summary>
        /// Тип оредра
        /// </summary>

        public EOrderType OrderType { get; set; }
        /// <summary>
        /// Состояние ордера
        /// </summary>

        public EOrdStatus OrdStatus { get; set; }
        /// <summary>
        /// Срок действия заявки
        /// </summary>

        public ETimeInForce TimeInForce { get; set; }
        /// <summary>
        /// Метка даты срока действия заявки при TimeInForce = GTD(GOOD_TILL_DATE) (в тиках)
        /// </summary>
        public long GoodTillDateStamp { get; set; }
        /// <summary>
        /// Метка даты срока действия заявки при TimeInForce = GTD(GOOD_TILL_DATE)
        /// </summary>
        /// <returns></returns>
        public DateTime GetGoodTillDate()
        {
            return new DateTime(GoodTillDateStamp);
        }


        public string GoodTillDateStr { get { return GoodTillDateStamp == 0 ? "-" : GetGoodTillDate().ToString("yyMMdd"); } }
        /// <summary>
        /// Идентификатор адаптера исполнения
        /// </summary>

        public int AdapterID { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>

        public string Text { get; set; }
        /// <summary>
        /// Идентификатор торгового алгоритма, породившего этот ордер
        /// </summary>

        public long AlgoID { get; set; }

        //public TimeMetric TimeMetric { get; set; }
    }
}
