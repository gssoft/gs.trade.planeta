using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TradeTerminals64.Quik
{
    class Trans2QuikList
    {
        private const string Trans2QuikDllName = "trans2quik.dll";
        
        #region Connect / Disconnect
        #region Connect
        [DllImport(Trans2QuikDllName, 
                EntryPoint = "TRANS2QUIK_CONNECT",
                CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 connect(
                string lpcstrConnectionParamsString,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
        #endregion
        #region Disconnect
        [DllImport(Trans2QuikDllName, 
                EntryPoint = "TRANS2QUIK_DISCONNECT",
                CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 disconnect(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
        #endregion

        #region Is_Quik_Connected
        [DllImport(Trans2QuikDllName, 
                EntryPoint = "TRANS2QUIK_IS_QUIK_CONNECTED",
                CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 is_quik_connected(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
        #endregion
        #region Is_dll_Connected
        [DllImport(Trans2QuikDllName,
                EntryPoint = "TRANS2QUIK_IS_DLL_CONNECTED",
                CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 is_dll_connected(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
        #endregion
        #endregion
    }
}
