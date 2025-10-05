using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Containers1;
using GS.Containers5;
using GS.Interfaces;

namespace GS.Exceptions
{
    public interface IGSExceptionBase : IHaveKey<string>, IHaveId<long>
    {
         DateTime DateTime { get; set; }
         string Source { get; set; }
         string ObjType { get; set; }
         string Operation { get; set; }
         string ObjStr { get; set; }
         string Message { get; set; }
         string SourceExc { get; set; }
         string ExcType { get; set; }
         string TargetSite { get; set; }

         string DateTimeString { get; }
         string TimeDateString { get; }
    }

    public interface IGSException : IGSExceptionBase
    {
        void SaveInFile(string directory);
    }
    public interface IGSExceptionDb : IGSExceptionBase
    {
    }

}
