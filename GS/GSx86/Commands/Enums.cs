using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Commands
{
    public enum CommandEnum : int
    {
        Start, Finish, Stop, Suspend, Continue, Resume, Abort, Terminate, Complete,
        Refresh, Save, Commit,
        Create, Read, Update, Delete, Remove,
        AddOrUpdate, AddOrGet, Register,
        Init, Open, Close,
        Requery,
        Unknown
    }

}
