using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade;

namespace GS.Accounts
{
    public class Accounts : Element1<string>, IAccounts
    {
        private readonly Dictionary<string,IAccount> _accounts;
        private readonly object _addLocker;

        public override string Key => Code;

        public Accounts()
        {
            Code = "Accounts.Core";
            Name = "Account Core Storage";

            _addLocker = new object();
            _accounts = new Dictionary<string, IAccount>();
        } 
        private void AddAccount(IAccount acc)
        {
            if (GetAccount(acc.Key) != null)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, Code, "Account",
                "RegisterAccount(): " + acc.Key.WithSqBrackets0(),
                "Add new Account to Core Storage - Failure. Account already Registered",
                acc.ToString());
                return;
            }
            lock( _addLocker)
            {
                acc.Accounts = this;
                acc.Parent = this;
                _accounts.Add(acc.Key.TrimUpper(), acc);
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Code, "Account",
                "RegisterAccount(): " + acc.Key.WithSqBrackets0(),
                "RegisterAccount(): " + acc.Key.WithSqBrackets0() + ". Add new Account to Core Storage",
                acc.ToString());
        }
        public IAccount Register(IAccount acc)
        {
            IAccount a;
            if( _accounts.TryGetValue(acc.Key.Trim().ToUpper(), out a))
                return a;

            AddAccount(acc);
            return acc;
        }
        public IAccount GetAccount1(string key)
        {
            IAccount a;
            //return _accounts.TryGetValue(key.Trim().ToUpper(), out a)
            //           ? a
            //           : null;
            if( _accounts.TryGetValue(key.Trim().ToUpper(), out a))
                return a;

            a = _accounts.Values.ToList().FirstOrDefault(ac => ac.Code.HasValue() && ac.Code.Trim() == key.Trim()) ??
                _accounts.Values.ToList().FirstOrDefault(ac => ac.Alias.HasValue() && ac.Alias.Trim() == key.Trim());

            return a ?? new Account {Alias = key, Name = key, Code = key};
        }
        public IAccount GetAccount(string key)
        {
            IAccount a;
            //return _accounts.TryGetValue(key.Trim().ToUpper(), out a)
            //           ? a
            //           : null;
            if (_accounts.TryGetValue(key.Trim().ToUpper(), out a))
                return a;

            a = _accounts.Values.ToList().FirstOrDefault(ac => ac.Code.HasValue() && ac.Code.Trim() == key.Trim()) ??
                _accounts.Values.ToList().FirstOrDefault(ac => ac.Alias.HasValue() && ac.Alias.Trim() == key.Trim());
            return a;
        }

        public IAccount CreateInstance(IAccountBase a)
        {
            return new Account
            {
                Id = a.Id,
                //Key = a.Key,
                Code = a.Code,
                Name = a.Name,
                Alias = a.Alias,
                TradePlace = a.TradePlace,

                Balance = a.Balance
            };
        }
        public IAccount CreateInstanceSimple(string key)
        {
            return new Account
            {
                //Id = ,
                //Key = a.Key,
                Code = key,
                Name = key,
                Alias = key
                // TradePlace = a.TradePlace,
                // Balance = a.Balance
            };
        }


        public IAccount this[string index]
        {
            get
            {
                return _accounts.Keys.Contains(index.Trim().ToUpper()) ? _accounts[index] : null;
            }
        }

        public IEnumerable<IAccount> GetAccounts {
            get { return _accounts.Values; }
        }

        public void Load()
        {
            DeserializeAccounts();
        }
        private void DeserializeAccounts()
        {
            string xmlfname = null;
            try
            {
                var xDoc = XDocument.Load(@"SpsInit.xml");

                var xe = xDoc.Descendants("Accounts_XmlFileName").First();
                xmlfname = xe.Value;

                var x = XElement.Load(xmlfname);

                var al = Serialization.Do.DeSerialize<List<Account>>(x, null);
                    //(s1, s2) =>
                    //          _evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    //                "Tickers", s1, String.Format("FileName={0}", xmlfname), s2));
               
                _accounts.Clear();
                foreach (var a in al)
                {
                    AddAccount(a);
                }
                //_evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Tickers", "DeSerialization",
                //String.Format("FileName={0}", xmlfname), "Count=" + _tickerDictionary.Count);
                
            }
            catch (Exception e)
            {
                //_evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Tickers", "DeSerialization",
                //String.Format("FileName={0}", xmlfname), e.ToString());

                throw new SerializationException("Accountss.Deserialization Failure " + xmlfname);
            }
        }
    }
    public class Account : Element1<string>, IAccount
    {
        public int Id { get; set; }
        //public string Code { get; set; }
        //public string Name { get; set; }
        //public string Alias { get; set; }
        public string TradePlace { get; set; }

        //public string Key { get; set; }
        public override string Key => 
            (TradePlace.HasValue() ? TradePlace + "@" : "") + Code;

        //[XmlIgnore]
        public Decimal Balance { get; set; }

        public decimal DailyProfitLimitPrcnt { get; set; }
        public decimal DailyLossLimitPrcnt { get; set; }

        public decimal Limit { get; set; }

        public decimal DailyProfitLimit => 
            Limit.IsGreaterThan(0m)
            ? Limit * DailyProfitLimitPrcnt/100m
            : decimal.MaxValue;

        public decimal DailyLossLimit => 
            Limit.IsGreaterThan(0m)
            ? -Limit * DailyLossLimitPrcnt / 100m
            : decimal.MinValue;

        [XmlIgnore]
        public IAccounts Accounts { get; set; }

        public Account()
        {
        }

        
        public override string ToString()
        {
            return
                $"Id:{Id} Code:{Code} Name:{Name} Alias:{Alias} TradePlace:{TradePlace} " +
                $"Key={Key} Balance{Balance} " +
                $"DailyProfitLimitPrcnt:{DailyProfitLimitPrcnt} DailyLossLimitPrcnt:{DailyLossLimitPrcnt}";
        }
    }
}
