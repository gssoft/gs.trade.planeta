using System;

namespace GS.Ftp
{
    
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
        public bool IsFileOrDirectory => IsFile || IsDirectory;
        public bool IsCurrentDirectory => DirMemberType == DirMemberEnum.CurrentDir;
        public bool IsParentDirectory => DirMemberType == DirMemberEnum.ParentDir;
       
        public override string ToString()
        {
            var size = IsFile ? Size : 0;
            return
                $"{DirMemberType}: {Name}, Prms: {Permissions}, Size: {size}, Modified: {Modified.ToString("yyyy-MM-dd HH:mm")}, Node: {Node}";
        }
    }
}
