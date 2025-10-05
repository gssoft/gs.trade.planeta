namespace GS.Trade.EntityWebClient.Dto
{
    public enum ESecurityType : byte
    {
        /// <summary>
        /// Фьючерз
        /// </summary>
        FUT = 0,
        /// <summary>
        /// Опцион
        /// </summary>
        OPT = 1,
        /// <summary>
        /// Акция
        /// </summary>
        STK = 2,
        /// <summary>
        /// Облигация
        /// </summary>
        BOND = 3,
        /// <summary>
        /// Значение индекса
        /// </summary>
        IDX = 4,
        /// <summary>
        /// И-Ти-Эф
        /// </summary>
        ETF = 5,
        /// <summary>
        /// Форекс
        /// </summary>
        FX = 6,
        /// <summary>
        /// Соммодитиз
        /// </summary>
        CMD = 7,
        /// <summary>
        /// Мельтилег
        /// </summary>
        MLEG = 8,
        /// <summary>
        /// РЕПО
        /// </summary>
        REPO = 9


    }

    public enum EOrderType : byte
    {
        /// <summary>
        /// Лимитный
        /// </summary>
        Limit = 0,
        /// <summary>
        /// Рыночный
        /// </summary>
        Market = 1,
        /// <summary>
        /// Стоп
        /// </summary>
        Stop = 2
    }

    public enum EOrdStatus : byte
    {

        /// <summary>
        /// Ордер стоит в торговой системе
        /// </summary>
        New = 0,
        /// <summary>
        /// Ордер частично исполнен
        /// </summary>
        PartiallyFilled = 1,
        /// <summary>
        /// Ордер исполнен
        /// </summary>
        Filled = 2,
        /// <summary>
        /// 
        /// </summary>
        DoneForDay = 3,
        /// <summary>
        /// Ордер отменен
        /// </summary>
        Cancelled = 4,
        /// <summary>
        /// Пытаемся отменить ордер
        /// </summary>
        PendingCancel = 6,
        /// <summary>
        /// Исполнение оредера остановлено
        /// </summary>
        Stopped = 7,
        /// <summary>
        /// Ордер отвергнут
        /// </summary>
        Rejected = 8,
        /// <summary>
        /// 
        /// </summary>
        Suspended = 9,
        /// <summary>
        /// Оредр отправляется
        /// </summary>
        PendingNew = 10,
        /// <summary>
        /// 
        /// </summary>
        Calculated = 11,
        /// <summary>
        /// Срок действия ордера истек
        /// </summary>
        Expired = 12,
        /// <summary>
        /// 
        /// </summary>
        AcceptedForBidding = 13,
        /// <summary>
        /// Пытаемся заменить ордер
        /// </summary>
        PendingReplace = 14,
        /// <summary>
        /// Order зааменен
        /// </summary>
        Replaced = 15,
        /// <summary>
        /// Статус оредра неопределен
        /// </summary>
        Undefined = 20
    }

    public enum ETimeInForce : byte
    {
        Day = 0,
        GTD = 1,
        IOC = 2,
        FOK = 3
    }
    public enum ESide : byte
    {
        /// <summary>
        /// Покупка
        /// </summary>
        Buy = 0,
        /// <summary>
        /// Продажа
        /// </summary>
        Sell = 1,
        /// <summary>
        /// Неизвестно
        /// </summary>
        Unknown = 2
    }
}
