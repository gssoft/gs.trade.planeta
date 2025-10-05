using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TradeTerminals64._64
{
    public interface IDllName
    {
        string DllName { get; }
    }

    public class Trans2QuikDllName01 : IDllName
    {
        public string DllName => Trans2QuikDllName;
        protected const string Trans2QuikDllName = "trans2quik.dll";
    }

    public class Trans2Quik64<T> where T : IDllName
    {
        T _dllname;
        static string s;

        public Trans2Quik64(T dllname)
        {
            this._dllname = dllname;
            s = dllname.DllName;
        }

       // [DllImport("Adv", EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16",CallingConvention = CallingConvention.StdCall)]   
    }
}
