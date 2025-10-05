using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpRequestParse_01
{
    public enum DirMemberEnum {Undefined = 0, File = 1, Dir = 2, CurrentDir = 3, ParentDir = 4 }


    public class FtpFileInfo
    {
        public string Name { get; set; }
        public string Permissions { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
        public int Node { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public DirMemberEnum DirMemberType { get; set; }
        public bool IsFile => DirMemberType == DirMemberEnum.File;
        public bool IsDirectory => DirMemberType == DirMemberEnum.Dir;
        public bool IsCurrentDirectory => DirMemberType == DirMemberEnum.CurrentDir;
        public bool IsParentDirectory => DirMemberType == DirMemberEnum.ParentDir;
        
        //public string DirStr => IsDirectory
        //                            ? (IsCurrentDirectory 
        //                                    ? "Current" 
        //                                    : (IsParentDirectory
        //                                            ? "Parent" 
        //                                            : "Dir"))
        //                            : "File";

        public override string ToString()
        {
            var size = IsFile ? Size : 0;
            return
                $"{DirMemberType}: {Name}, Prms: {Permissions}, Size: {size}, Modified: {Modified.ToString("yyyy-MM-dd HH:mm")}, Node: {Node}";
        }
    }
}
