using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.DataBase.Entities;

namespace GS.DataBase.Configuration.Model
{
    public enum EnDisEnum : short { Enabled = 1, Disabled = 0 }

    public enum OperResultEnum : short { Success = 1, Failure = -1 }
    
    public class Configuration : SimpleEntity2
    {
        public EnDisEnum Enabled { get; set; }
        public string Catalog { get; set; }

        //15.11.21
        public int? Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? IncDaysExpire { get; set; }

        public override string Key
        {
            get { return Code; }
        }
        public bool IsEnabled {
            get { return Enabled == EnDisEnum.Enabled; }
        }

        public Configuration() : base()
        {
            var dt = DateTime.Now;
            //CreatedDT = dt;
            //ModifiedDT = dt;
            ExpireDate = dt.Date.AddDays(1);
            IncDaysExpire = 1;
            Token = 0;
            Items = new List<Item>();
        }
        public IList<Item> Items { get; set; }
        public Item Add(Item i)
        {
            
            var item = Items.FirstOrDefault(it => it.Code == i.Code);
            if (item != null)
                return item;
            Items.Add(i);
            return i;
        }
    }

    public class Item : SimpleEntity2
    {
        [ForeignKey("ConfigurationId")]
        public virtual Configuration Configuration { get; set; }
        public long ConfigurationId { get; set; }
        public EnDisEnum Enabled { get; set; }
        public string Catalog { get; set; }

        // 15.11.21
        public string CatalogExt { get; set; }
        public string Obj { get; set; }

        public int TrCount { get; set; }
        public override string Key
        {
            get { return Code; }
        }
        public bool IsEnabled {
            get { return Enabled == EnDisEnum.Enabled; }
        }
        public IList<Transaction> Transactions { get; set; }

        public Item() : base()
        {
            Transactions = new List<Transaction>();
        }
    }

    public class Transaction : SimpleTimeEntity
    {
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
        public long ItemId { get; set; }

        public OperResultEnum Result { get; set; }

        public string IpAddress { get; set; }
        public string Domain { get; set; }
        public string User { get; set; }
        public string Request { get; set; }
        public string Operation { get; set; }
        public string Object { get; set; }

    }
}
