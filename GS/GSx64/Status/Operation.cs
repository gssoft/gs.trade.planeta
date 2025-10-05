using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Status
{
    public enum OperationEnum : int
    {
        Ok,
        Empty, NoOperation, Clear, Init,
        Add, AddNew, AddOrUpdate, AddOrGet, Register,
        Delete, Remove, Erase, Kill,
        Update, Move,
        Get, Put, Push, Pull,
        Start, Stop, Finish, Complete, Continue, Pause, Terminate,
        Open, Close, 
        Exception, Error, Failure,
        Send, Receive,
        Refresh,
        Read, Write,
        Print, Show,
        NoConnection, NotConnected, Connected 
    }
}
