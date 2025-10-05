using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GS.Extension;

namespace GS.Ftp
{
    public class FtpReturn
    {
        public string FileShortPath { get; set; }
        public string FileFullName { get; set; }

        public string DirName =>
            FileShortPath.HasValue()
                ? Path.GetDirectoryName(FileShortPath)
                : string.Empty;
        public string FileName =>
            FileShortPath.HasValue()
                ? Path.GetFileName(FileShortPath)
                : string.Empty;

        public string DirFileNameStr1 => $"Dir: {DirName}, File: {FileName}";
        public string DirFileNameStr => $"{DirName}\\ {FileName}";

        public string FtpOperStr => $"{FtpOperation}, {FtpOperationStatus}";

        public FtpStatusCode FtpStatusCode { get; set; }
        public string FtpStatusDescription { get; set; }
        public string Content { get; set; }
        public object Object { get; set; }
  //      public string Encoding { get; set; }
        public FtpOperation FtpOperation { get; set; }
        public FtpOperationStatus FtpOperationStatus { get; set; }
        public HashStatus HashStatus { get; set; }
        public Exception Exception { get; set; }
        public List<FtpFileInfo> ListDirectoryDetails { get; set; }

        public FtpReturn FtpReturn2 { get; set; }
        public bool IsSuccess => FtpOperationStatus == FtpOperationStatus.Ok;

        public bool IsFileOperationSuccess => 
                        FtpOperationStatus == FtpOperationStatus.Ok && HashStatus != HashStatus.Bad;

        public string FileOperationStatusStr => 
            HashStatus == HashStatus.Bad ? HashStatus.Bad.ToString() : FtpStatusCode.ToString();

        public FtpReturn()
        {
            Clear();
        }

        public void Clear()
        {
            FileShortPath = string.Empty;
            FileFullName = string.Empty;
            FtpStatusCode = FtpStatusCode.Undefined;
            FtpStatusDescription = string.Empty;
            Content = string.Empty;
            FtpOperationStatus = FtpOperationStatus.UnDefined;
            Exception = null;
            ListDirectoryDetails?.Clear();
            ListDirectoryDetails = null;

            if (FtpReturn2 == null)
                return;
            if (FtpReturn2.FtpReturn2 != null)
            {
                FtpReturn2.FtpReturn2.Clear();
                FtpReturn2.FtpReturn2 = null;
            }
            else
            {
                FtpReturn2.Clear();
                FtpReturn2 = null;
            }
        }

        public FtpReturn Clone()
        {
            return new FtpReturn
            {
                FileFullName = FileFullName,
                FtpStatusCode = FtpStatusCode,
                FtpStatusDescription = FtpStatusDescription,
                Content = Content,
                FtpOperationStatus = FtpOperationStatus
            };
        }

        public List<FtpReturn> Flatten()
        {
            var lst = new List<FtpReturn> {this};

            if (FtpReturn2 == null)
                return lst;

            lst.Add(FtpReturn2);
            if(FtpReturn2.FtpReturn2 != null)
                lst.Add(FtpReturn2.FtpReturn2);
            return lst;
        }
        public override string ToString()
        {
            return
                   // $"FullName: {FileFullName}," + Environment.NewLine +
                   // $"ShortPath: {FileShortPath}," + Environment.NewLine +
                   //  $"Dir: {DirName}," + Environment.NewLine +
                   //  $"File: {FileName}," + Environment.NewLine +
                   //  $"{DirFileNameStr},"  + Environment.NewLine +
                   $"FtpStatus: {FtpStatusCode}, {FtpStatusDescription.Trim()}"
                  // $" {FtpOperation}, {FtpOperationStatus}"
                   ;
        }
        public  string ToString1()
        {
            return
                  // $"FullName: {FileFullName}," + Environment.NewLine +
                  // $"ShortPath: {FileShortPath}," + Environment.NewLine +
                 //  $"Dir: {DirName}," + Environment.NewLine +
                 //  $"File: {FileName}," + Environment.NewLine +
                 //  $"{DirFileNameStr},"  + Environment.NewLine +
                   $"FtpStatus: {FtpStatusCode}," + Environment.NewLine +
                   $"FtpStatusDescr: {FtpStatusDescription.Trim()}," + Environment.NewLine +
                   $"Oper: {FtpOperation}," + Environment.NewLine +
                   $"OperStatus: {FtpOperationStatus}" // ," + Environment.NewLine +
                  // (FtpOperation == FtpOperation.UploadFile ? $"Encoding: {Encoding}," : "")
                    // $"HashStatus: {HashStatus}";
                    ;
        }
    }
}