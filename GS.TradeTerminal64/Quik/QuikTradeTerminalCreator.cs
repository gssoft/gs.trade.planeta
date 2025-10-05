using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed class QuikTradeTerminalCreator
    {
        private const int MaxTrans2QuikDllCount = 3;
        private readonly List<QuikTradeTerminal> _quikTradeTerminals;
        private readonly List<ITrans2Quik> _trans2QuikDlls;

        private QuikTradeTerminalCreator()
        {
            _quikTradeTerminals = new List<QuikTradeTerminal>();
            _trans2QuikDlls = new List<ITrans2Quik>();
        }
        public static QuikTradeTerminalCreator GetInstance { get; } = new QuikTradeTerminalCreator();
        public QuikTradeTerminal GetQuikTradeTerminalOrNew(string path2Quik)
        // public QuikTradeTerminal GetQuikTradeTerminalOrNew( string path2Quik )
        {
            path2Quik = path2Quik.Trim().ToUpper();
            QuikTradeTerminal q = _quikTradeTerminals.FirstOrDefault(t => t.Path2Quik == path2Quik);
            if (q != null) return q;
            //foreach (var q in _quikTradeTerminals.Where(q => q.Path2Quik == path2Quik))
            //{
            //    return q;
            //}
            var qtt = new QuikTradeTerminal(path2Quik, GetNewTrans2Quik());
            //qtt.SetTransactionReplyCallback(QuikTradeTerminal.TransactionReplyHandler);
            _quikTradeTerminals.Add(qtt);
            return qtt;
        }
        public IQuikTradeTerminal GetQuikTradeTerminalOrNull(string path2Quik)
        {
            return _quikTradeTerminals.FirstOrDefault(q => q.Path2Quik == path2Quik);
        }
        private ITrans2Quik GetNewTrans2Quik()
        {
            ITrans2Quik t2Q;
            switch (_trans2QuikDlls.Count)
            {
                case 0:
                    t2Q = new Trans2Quik01();
                    _trans2QuikDlls.Add(t2Q);
                    return t2Q;
                case 1:
                    // t2Q = new Trans2Quik02();
                    t2Q = new Trans2Quik01();
                    _trans2QuikDlls.Add(t2Q);
                     return t2Q;
                case 2:
                    // t2Q = new Trans2Quik03();
                    t2Q = new Trans2Quik01();
                    _trans2QuikDlls.Add(t2Q);
                      return t2Q;
                default:
                    throw new Exception($"Max DllName Count {MaxTrans2QuikDllCount} already in use");
            }
        }
        /*
        public string CheckConnection()
        {
            string ret = null;
            foreach (var qt in _quikTradeTerminals)
            {
                ret = qt.IsConnectedResult() + ' ' + qt.DllNamePath2QuikPair; 
            }
            return ret;
        }
        */
    }
}