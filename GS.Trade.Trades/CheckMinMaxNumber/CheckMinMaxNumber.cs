using System.Collections.Generic;
using GS.Containers3;
using GS.Extension;
using GS.Trade.Trades.CheckMinMaxNumber;

namespace GS.Trade.Trades.CheckMinMaxNumber
{
    public class CheckMember : Containers3.IContainerItem<string>
    {
        public string Account { get; set; }
        public ulong Number { get; set; }
        public string Key => Account.TrimUpper();
    }
    public class CheckMinMaxNumber
    {
        // private readonly Dictionary<string, long> _membersToCheck;

        private readonly ListContainer<string> _membersToCheck;

        public CheckMinMaxNumber()
        {
            //_membersToCheck = new Dictionary<string, long>();
            _membersToCheck = new ListContainer<string>();
        }

        public bool SetIfGreaterThanMe(string key, ulong value)  // Simulate Max
        {
            var v = _membersToCheck.GetByKey(key.TrimUpper()) as CheckMember;
            if (v == null)
            {
                var i = new CheckMember { Account = key, Number = value};
                _membersToCheck.Add(i);
                return true;
            }
            if (value > v.Number)
            {
                v.Number = value;
                return true;
            }
            return false;
        }
        public bool SetIfLessThanMe(string key, ulong value) // Simulate Min
        {
            var v = _membersToCheck.GetByKey(key.TrimUpper()) as CheckMember;
            if (v == null)
            {
                var i = new CheckMember { Account = key, Number = value};
                _membersToCheck.Add(i);
                return true;
            }
            if (value < v.Number)
            {
                v.Number = value;
                return true;
            }
            return false;
        }

        //public void Clear()
        //{
        //    lock (_membersToCheck)
        //    {
        //        _membersToCheck.Clear();
        //    }
        //}

        public CheckMember GetByKey(string key)
        {
            return _membersToCheck.GetByKey(key.TrimUpper()) as CheckMember;
        }
    }
}
