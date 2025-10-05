using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Web.Service01
{
    public class DataTransferObj
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"Code:{Code}, Name:{Name}";
        }
    }

    [Serializable]
    public class DataDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"Code:{Code}, Name:{Name}";
        }
    }
}
