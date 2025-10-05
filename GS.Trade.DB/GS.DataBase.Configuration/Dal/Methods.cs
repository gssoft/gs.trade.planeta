using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.Extension;

namespace GS.DataBase.Configuration.Dal
{
    using Model;

    /// <summary>
    /// Add with DaeteTime ? Handling
    /// </summary>
    
    public partial class ConfigurationContext
    {
        public Configuration GetConfiguration(string key)
        {
            return key.HasNoValue() 
                ? null 
                : Configurations.FirstOrDefault(conf => conf.Code == key);
        }
        public Item GetItem(Configuration cnf, string key)
        {
            if (cnf == null || key.HasNoValue())
                return null;
            return Items.FirstOrDefault(i => i.ConfigurationId == cnf.Id && i.Code == key);
        }

        public IEnumerable<Item> GetItems(long? cnfId)
        {
            IQueryable<Item> items;
            if (cnfId == null)
                items = Items.Include(i => i.Configuration);
            else
                items = Items
                        .Where(i => i.ConfigurationId == cnfId)
                        .Include(i => i.Configuration);
            return items;
        }


        public Configuration Add(Configuration c)
        {
            //var cnf = Configurations.FirstOrDefault(conf => conf.Code == c.Code);
            var cnf = GetConfiguration(c.Code);
            return cnf ?? Configurations.Add(c);
        }
        public Item Add(Item i)
        {
            var item = Items.FirstOrDefault(it => it.Code == i.Code);
            return item ?? Items.Add(i);
        }
        //public Transaction Add(Transaction t)
        //{
        //    t.DT = DateTime.Now;
        //    return Transactions.Add(t);
        //}

        public Configuration IsConfigurationEnabled(string key)
        {
            var cnf = GetConfiguration(key);
            return cnf != null && cnf.Enabled == EnDisEnum.Enabled ? cnf : null;
        }
        public KeyValuePair<Configuration, bool> IsConfigurationEnabled2(string key)
        {
            var kvp = new KeyValuePair<Configuration, bool>();
            var cnf = GetConfiguration(key);
            
            return cnf == null 
                ? new KeyValuePair<Configuration, bool>(null,false)
                : new KeyValuePair<Configuration, bool>(cnf, cnf.IsEnabled);
        }

        public Item IsItemEnabled(Configuration c, string itemKey)
        {
            var items = Items.Where(i => i.ConfigurationId == c.Id);
            var item = items.FirstOrDefault(i => i.Code == itemKey);

            return item != null && item.Enabled == EnDisEnum.Enabled // && item.TrCount > 0
                    ? item
                    : null;
        }

        public Item IsItemEnabled(string confKey, string itemKey)
        {
            if (confKey.HasNoValue() || itemKey.HasNoValue())
                return null;
            var cnf = IsConfigurationEnabled(confKey);
            if (cnf == null)
                return null;
            var items = Items.Where(i => i.ConfigurationId == cnf.Id);
            var item = items.FirstOrDefault(i => i.Code == itemKey);

            return item != null && item.Enabled == EnDisEnum.Enabled // && item.TrCount > 0
                    ? item
                    : null;
        }

        public Item IsTransactionEnabled(string confKey, string itemKey)
        {
            var item = IsItemEnabled(confKey, itemKey);
            if (item == null)
                return null;

            if (--item.TrCount <= 0)
                item.Enabled = EnDisEnum.Disabled;

            SaveChanges();

            if (!Items.Where(i => i.ConfigurationId == item.ConfigurationId)
                .All(i => i.Enabled == EnDisEnum.Disabled))
                return item;
            
            item.Configuration.Enabled = EnDisEnum.Disabled;
            SaveChanges();

            return item;
        }

        public void TransactionProcess(string confKey, string itemKey, string oper, string obj)
        {
            var item = IsTransactionEnabled(confKey, itemKey);
            if (item == null)
                return;
            var t = new Transaction
            {
                Operation = oper,
                Object = obj
            };
            item.Transactions.Add(t);

            SaveChangesAsync();

        }

        public void AddTransaction(string request, string ip, Item i, OperResultEnum res, string oper, string obj, int save)
        {
            var tr = new Transaction
            {
                DT = DateTime.Now,
                IpAddress = ip,
                Request = request,
                ItemId = i.Id,
                Operation = oper,
                Result = res,
                Object = obj
            };
            Transactions.Add(tr);
            if (save > 0)
                SaveChanges();
        }
        public void AddTransaction(Configuration cnf, Item item, string request, string ip, OperResultEnum res, string oper, string obj, int save)
        {
            Configuration c;
            Item i;
            if (cnf == null && item == null)
            {
                c = GetConfiguration("Unknown");
                if (c == null)
                    return;
                i = GetItem(c, "Unknown");
                if (i == null)
                    return; 
            }
            else if (cnf != null && item == null)
            {
                i = GetItem(cnf, "Unknown");
                if (i == null)
                    return;
            }
            else if (cnf != null)
            {
                c = cnf;
                i = item;
            }
            else
            {
                i = item;
            }
            var tr = new Transaction
            {
                DT = DateTime.Now,
                IpAddress = ip,
                Request = request,
                ItemId = i.Id,
                Operation = oper,
                Result = res,
                Object = obj
            };
            Transactions.Add(tr);
            if (save > 0)
                SaveChanges();
        }

        public void AddTransaction(Configuration cnf, Item item, string request, string ip, string dm, string user, OperResultEnum res, string oper, string obj, int save)
        {
            Configuration c;
            Item i;
            if (cnf == null && item == null)
            {
                c = GetConfiguration("Unknown");
                if (c == null)
                    return;
                i = GetItem(c, "Unknown");
                if (i == null)
                    return;
            }
            else if (cnf != null && item == null)
            {
                i = GetItem(cnf, "Unknown");
                if (i == null)
                    return;
            }
            else if (cnf != null)
            {
                c = cnf;
                i = item;
            }
            else
            {
                i = item;
            }
            var tr = new Transaction
            {
                DT = DateTime.Now,
                IpAddress = ip,
                Request = request,
                Domain = dm,
                User = user,
                ItemId = i.Id,
                Operation = oper,
                Result = res,
                Object = obj
            };
            Transactions.Add(tr);
            if (save > 0)
                SaveChanges();
        }

        /// <summary>
        /// Updates Entity Fileds Methods
        /// </summary> 
        #region Updates Entity Fileds Methods

        public void UpdateCnfItemEnables(long? cnfId, long? itemId, EnDisEnum ed)
        {
            if ( cnfId == null && itemId == null)
            {
                foreach (var item in Items.ToList())
                {
                    item.Enabled = ed;
                    item.ModifiedDT = DateTime.Now;
                }
            }
            else if (cnfId != null && itemId == null)
            {
                var cnf = Configurations.Find(cnfId);
                if (cnf == null)
                    return;
                var items = Items.Where(i => i.ConfigurationId == cnfId).ToList();
                foreach (var i in items)
                {
                    i.Enabled = ed;
                    i.ModifiedDT = DateTime.Now;
                }
            }
            else // if (cnfId == null && itemId != null)
            {
                var item = Items.Find(itemId);
                if (item == null)
                    return;
                item.Enabled = ed;
                item.ModifiedDT = DateTime.Now;
            }
            SaveChanges();
        }
        public void UpdateCnfEnables(long? cnfId, EnDisEnum ed)
        {
            if (cnfId == null)
            {
                foreach (var cnf in Configurations.ToList())
                {
                    cnf.Enabled = ed;
                    cnf.ModifiedDT = DateTime.Now;
                }
            }
            else
            {
                var cnf = Configurations.Find(cnfId);
                if (cnf == null)
                    return;
                cnf.Enabled = ed;
                cnf.ModifiedDT = DateTime.Now;
            }
            SaveChanges();
        }
        public void UpdateCnfItemEnables(long? itemId, EnDisEnum ed)
        {
            if (itemId == null)
            {
                foreach (var item in Items.ToList())
                {
                    item.Enabled = ed;
                }
            }
            else
            {
                var item = Items.Find(itemId);
                if (item == null)
                    return;
                item.Enabled = ed;
            }
            SaveChanges();
        }

        public void UpdateToken(long? cnfId)
        {
            var rnd = new Random();
            var seed = rnd.Next();
            rnd = new Random(seed);
            if (cnfId == null)
            {
                foreach (var cnf in Configurations.ToList())
                {
                    cnf.Token = rnd.Next();
                    cnf.ModifiedDT = DateTime.Now;
                }
            }
            else
            {
                var cnf = Configurations.Find(cnfId);
                if (cnf == null)
                    return;
                cnf.Token = rnd.Next();
                cnf.ModifiedDT = DateTime.Now;
            }
            SaveChanges();
        }
        public void UpdateExpireDate(long? cnfId)
        {
            if (cnfId == null)
            {
                foreach (var cnf in Configurations.ToList())
                {
                    if (cnf.IncDaysExpire == null)
                        continue;
                    cnf.ExpireDate = DateTime.Now.Date.AddDays((double) cnf.IncDaysExpire);
                    cnf.ModifiedDT = DateTime.Now;
                }
            }
            else
            {
                var cnf = Configurations.Find(cnfId);
                if (cnf == null)
                    return;
                if (cnf.IncDaysExpire == null)
                    return;
                cnf.ExpireDate = DateTime.Now.Date.AddDays((double)cnf.IncDaysExpire);
                cnf.ModifiedDT = DateTime.Now;
            }
            SaveChanges();
        }
        #endregion


        public bool CheckRequest(long? tkn, string cnf, string item, string request, string ip,
                                    out Configuration configuration, out Item it)
        {
            Configuration c = null;
            Item i = null;
            try
            {
                c = GetConfiguration(cnf);
                if (c == null)
                {
                    AddTransaction(null, null, request, ip, OperResultEnum.Failure,
                        "Get: Wrong Configuration: " + cnf, cnf, 1);
                    configuration = null;
                    it = null;
                    return false;
                }
                if (!c.IsEnabled)
                {
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, OperResultEnum.Failure,
                        "Get: Configuration Disabled: " + cnf, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (c.ExpireDate == null || DateTime.Now.Date >= c.ExpireDate)
                {
                    DateTime dt = DateTime.Now.Date;
                    if (c.ExpireDate != null)
                        dt = (DateTime) c.ExpireDate;
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, OperResultEnum.Failure,
                        "Get: Configuration Expired in: " + dt.ToString("dd-MM-yyyy") + " " + cnf, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (tkn == null || (tkn != null && c.Token != tkn))
                {
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, OperResultEnum.Failure,
                        "Get: Wrong Token: " + tkn, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                i = GetItem(c, item);
                if (i == null)
                {
                    AddTransaction(c, null, request, ip, OperResultEnum.Failure,
                        "Get: Wrong Item: " + item, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (!i.IsEnabled)
                {
                    AddTransaction(c, i, request, ip, OperResultEnum.Failure,
                        "Get: Item Disabled: " + item, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddTransaction(c, i, request, ip, OperResultEnum.Failure, "CheckObject Exception: " + ex.Message, "", 1);
                configuration = c;
                it = i;
                return false;
            }
            configuration = c;
            it = i;
            return true;
        }

        public bool CheckRequest(long? tkn, string cnf, string item, string request, string ip, string domain, string user,
                                   out Configuration configuration, out Item it)
        {
            Configuration c = null;
            Item i = null;
            try
            {
                c = GetConfiguration(cnf);
                if (c == null)
                {
                    AddTransaction(null, null, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Wrong Configuration: " + cnf, cnf, 1);
                    configuration = null;
                    it = null;
                    return false;
                }
                if (!c.IsEnabled)
                {
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Configuration Disabled: " + cnf, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (c.ExpireDate == null || DateTime.Now.Date >= c.ExpireDate)
                {
                    DateTime dt = DateTime.Now.Date;
                    if (c.ExpireDate != null)
                        dt = (DateTime)c.ExpireDate;
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Configuration Expired in: " + dt.ToString("dd-MM-yyyy") + " " + cnf, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (tkn == null || (tkn != null && c.Token != tkn))
                {
                    i = GetItem(c, item);
                    AddTransaction(c, i, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Wrong Token: " + tkn, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                i = GetItem(c, item);
                if (i == null)
                {
                    AddTransaction(c, null, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Wrong Item: " + item, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
                if (!i.IsEnabled)
                {
                    AddTransaction(c, i, request, ip, domain, user, OperResultEnum.Failure,
                        "Get: Item Disabled: " + item, cnf + " " + item, 1);
                    configuration = c;
                    it = i;
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddTransaction(c, i, request, ip, domain, user, 
                    OperResultEnum.Failure, "CheckObject Exception: " + ex.Message, "", 1);
                configuration = c;
                it = i;
                return false;
            }
            configuration = c;
            it = i;
            return true;
        }
    }   
}
