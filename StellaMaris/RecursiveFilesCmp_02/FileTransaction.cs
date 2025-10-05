using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Ftp;

namespace RecursiveFilesCmp_02
{
    public class FileOperations
    {
        public string DirName { get; set; }
        public string Name { get; set; }
        public string RelativeName { get; set; }
        public string FullName { get; set; }

        private readonly List<FileTransaction> _fileTransactions;

        public IEnumerable<FileTransaction> Transactions => _fileTransactions.ToList();

        public bool IsSuccess => Transactions.Count(i => i.Status != FtpOperationStatus.Ok) <= 0;

        public override string ToString()
        {
            return $"File: {RelativeName}, {FileOperationsStr}";
        }

        public void Clear()
        {
            _fileTransactions.Clear();
        }

        public FileOperations(string relativepath)
        {
            _fileTransactions = new List<FileTransaction>();

            DirName = Path.GetDirectoryName(relativepath);
            Name = Path.GetFileName(relativepath);
            RelativeName = relativepath;
            FullName = relativepath;
        }

        public FileOperations(FtpReturn ftpret)
        {
            _fileTransactions = new List<FileTransaction>();

            var relativepath = ftpret.FileShortPath;
            DirName = Path.GetDirectoryName(relativepath);
            Name = Path.GetFileName(relativepath);
            RelativeName = relativepath;
            FullName = relativepath;

            Add(ftpret.FtpOperation, ftpret.FtpOperationStatus);
        }

        public string FileOperationsStr
        {
            get
            {
                var str = Transactions.Select(i => i.ToString());
                return string.Join(", ", str);
            }
        }

        public void Add(FtpReturn ftpret)
        {
            var rs = ftpret.Flatten();
            foreach (var t in rs.Select(r => new FileTransaction
            {
                Operation = r.FtpOperation,
                Status = r.FtpOperationStatus
            }
            ))
            {
                _fileTransactions.Add(t);
            }
        }
        public void Add(FileTransaction ft)
        {
            _fileTransactions.Add(ft);
        }

        public void Add(FtpOperation foperation, FtpOperationStatus status)
        {
            Add(new FileTransaction {Operation = foperation, Status = status});
        }
    }
    public class FileTransaction
    {
        public FtpOperation Operation { get; set; }
        public FtpOperationStatus Status { get; set; }

        public override string ToString()
        {
            return $"{Operation}: {Status}";
        }
    }

}
