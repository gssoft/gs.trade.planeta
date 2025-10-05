using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpRequestParse_02
{
    public class FtpFileInfo
    {
        public string Name { get; set; }
        public string Permissions { get; set; }
        public bool IsDirectory { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }

        public override string ToString()
        {
            var lineType = IsDirectory ? "Dir" : "File";
            var size = IsDirectory ? 0 : Size;
            return $"{lineType}: {Name}, Prms: {Permissions}, Size: {size}, Modified: {Modified.ToString("yyyy-MM-dd HH:mm")}";
        }
    }
}
