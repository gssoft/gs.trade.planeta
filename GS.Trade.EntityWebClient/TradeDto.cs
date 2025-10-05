using System;

namespace GS.Trade.EntityWebClient
{
    public class TradeDto //: IHaveKey<long>
    {
        public TradeDto()
        {
        }

        public long Id { get; set; }

        public DateTime Dt { get; set; }

        public long TradeId { get; set; }
        public long OrderId { get; set; }
        public long ClOrdId { get; set; }

        //public long PortfolioId { get; set; }
        //[ForeignKey("PortfolioId")]
        //public virtual Portfolio Portfolio { get; set; }

        public string Account { get; set; }
        public string Symbol { get; set; }
        public long AlgoID { get; set; }
        public string AlgoName { get; set; }

        public int Side { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public double Commission { get; set; }
        public double Commission2 { get; set; }

        public long Key
        {
            get { return Id; }
        }

        public override string ToString()
        {
            return string.Concat(
                "[Id:", Id,
                " TradeID:", TradeId,
                " OrderID:", OrderId,
                " ClOrdID:", ClOrdId,
                " Acc:", Account,
                " AlgoID:", AlgoID,
                " AlgoName:", AlgoName,
                " Symb:", Symbol,
                " Side:", Side > 0 ? "Long" : "Short",
                " Qty:", Qty,
                " Price:", Price,
                "]");
        }
    }
}
