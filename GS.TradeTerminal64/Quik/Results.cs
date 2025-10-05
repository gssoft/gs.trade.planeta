using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        #region Results and ResultsConversation Functions

        #region Константы возвращаемых значений
        public const Int32 TRANS2QUIK_SUCCESS = 0;
        public const Int32 TRANS2QUIK_FAILED = 1;
        public const Int32 TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2;
        public const Int32 TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3;
        public const Int32 TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4;
        public const Int32 TRANS2QUIK_WRONG_SYNTAX = 5;
        public const Int32 TRANS2QUIK_QUIK_NOT_CONNECTED = 6;
        public const Int32 TRANS2QUIK_DLL_NOT_CONNECTED = 7;
        public const Int32 TRANS2QUIK_QUIK_CONNECTED = 8;
        public const Int32 TRANS2QUIK_QUIK_DISCONNECTED = 9;
        public const Int32 TRANS2QUIK_DLL_CONNECTED = 10;
        public const Int32 TRANS2QUIK_DLL_DISCONNECTED = 11;
        public const Int32 TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12;
        public const Int32 TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13;
        public const Int32 TRANS2QUIK_WRONG_INPUT_PARAMS = 14;
        #endregion

        public static string ResultToString(long result)
        {
            switch (result & 255)
            {
                case TRANS2QUIK_SUCCESS:                                //0
                    return "TRANS2QUIK_SUCCESS";
                case TRANS2QUIK_FAILED:                                 //1
                    return "TRANS2QUIK_FAILED";
                case TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND:                //2
                    return "TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND";
                case TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED:              //3
                    return "TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED";
                case TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK:              //4
                    return "TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK";
                case TRANS2QUIK_WRONG_SYNTAX:                           //5
                    return "TRANS2QUIK_WRONG_SYNTAX";
                case TRANS2QUIK_QUIK_NOT_CONNECTED:                     //6
                    return "TRANS2QUIK_QUIK_NOT_CONNECTED";
                case TRANS2QUIK_DLL_NOT_CONNECTED:                      //7
                    return "TRANS2QUIK_DLL_NOT_CONNECTED";
                case TRANS2QUIK_QUIK_CONNECTED:                         //8
                    return "TRANS2QUIK_QUIK_CONNECTED";
                case TRANS2QUIK_QUIK_DISCONNECTED:                      //9
                    return "TRANS2QUIK_QUIK_DISCONNECTED";
                case TRANS2QUIK_DLL_CONNECTED:                          //10
                    return "TRANS2QUIK_DLL_CONNECTED";
                case TRANS2QUIK_DLL_DISCONNECTED:                       //11
                    return "TRANS2QUIK_DLL_DISCONNECTED";
                case TRANS2QUIK_MEMORY_ALLOCATION_ERROR:                //12
                    return "TRANS2QUIK_MEMORY_ALLOCATION_ERROR";
                case TRANS2QUIK_WRONG_CONNECTION_HANDLE:                //13
                    return "TRANS2QUIK_WRONG_CONNECTION_HANDLE";
                case TRANS2QUIK_WRONG_INPUT_PARAMS:                     //14
                    return "TRANS2QUIK_WRONG_INPUT_PARAMS";
                default:
                    return "UNKNOWN_VALUE";
            }
        }
        public static string ResultToString(Int32 result)
        {
            switch (result & 255)
            {
                case TRANS2QUIK_SUCCESS:                                //0
                    return "TRANS2QUIK_SUCCESS";
                case TRANS2QUIK_FAILED:                                 //1
                    return "TRANS2QUIK_FAILED";
                case TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND:                //2
                    return "TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND";
                case TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED:              //3
                    return "TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED";
                case TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK:              //4
                    return "TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK";
                case TRANS2QUIK_WRONG_SYNTAX:                           //5
                    return "TRANS2QUIK_WRONG_SYNTAX";
                case TRANS2QUIK_QUIK_NOT_CONNECTED:                     //6
                    return "TRANS2QUIK_QUIK_NOT_CONNECTED";
                case TRANS2QUIK_DLL_NOT_CONNECTED:                      //7
                    return "TRANS2QUIK_DLL_NOT_CONNECTED";
                case TRANS2QUIK_QUIK_CONNECTED:                         //8
                    return "TRANS2QUIK_QUIK_CONNECTED";
                case TRANS2QUIK_QUIK_DISCONNECTED:                      //9
                    return "TRANS2QUIK_QUIK_DISCONNECTED";
                case TRANS2QUIK_DLL_CONNECTED:                          //10
                    return "TRANS2QUIK_DLL_CONNECTED";
                case TRANS2QUIK_DLL_DISCONNECTED:                       //11
                    return "TRANS2QUIK_DLL_DISCONNECTED";
                case TRANS2QUIK_MEMORY_ALLOCATION_ERROR:                //12
                    return "TRANS2QUIK_MEMORY_ALLOCATION_ERROR";
                case TRANS2QUIK_WRONG_CONNECTION_HANDLE:                //13
                    return "TRANS2QUIK_WRONG_CONNECTION_HANDLE";
                case TRANS2QUIK_WRONG_INPUT_PARAMS:                     //14
                    return "TRANS2QUIK_WRONG_INPUT_PARAMS";
                default:
                    return "UNKNOWN_VALUE";
            }
        }
        public static string ByteToString(byte[] str)
        {
            if (str == null) return "";
            var count = 0;
            for (var i = 0; i < str.Length; ++i)
            {
                if (0 != str[i]) continue;
                count = i;
                break;
            }
            return System.Text.Encoding.Default.GetString(str, 0, count);
        }
        #endregion      
    }
}
