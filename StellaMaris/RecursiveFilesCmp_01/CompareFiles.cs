using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Ftp;
using GS.Serialization;

namespace RecursiveFilesCmp_01
{
    public class CompareFiles : Element1<string>
    {
        public string EtalonDir { get; set; }
        public string ProductionDir { get; set; }
        public bool IsExtraDirectoryToDelete { get; set; }
        public List<string> TextFileExtensions { get; set; }
        public List<string> BinFileExtensions { get; set; }
        public List<string> DirExceptionList { get; set; }

        protected List<FtpReturn> WrongFtpReturnInfo { get; set; }
        protected List<string> WrongFiles { get; set; }

        private DirectoryInfo _etalonDirInfo;
        private DirectoryInfo _productionDirectoryInfo;

        protected FtpWebClient FtpWebClient { get; set; }

        [XmlIgnore]
        public int StackDepthCount { get; private set; }
        [XmlIgnore]
        public long FilesCount { get; private set; }

        [XmlIgnore]
        public long ErrorCount { get; private set; }
        [XmlIgnore]
        public long SuccessCount { get; private set; }
        [XmlIgnore]
        public long ExceptionError { get; private set; }

        public override void Init()
        {
            ErrorCount = 0;
            SuccessCount = 0;
            ExceptionError = 0;

            WrongFiles = new List<string>();
            WrongFtpReturnInfo = new List<FtpReturn>();

           // WhoAreYou();

            try
            {
                _etalonDirInfo = new DirectoryInfo(EtalonDir);
                _productionDirectoryInfo = new DirectoryInfo(ProductionDir);

                //FtpWebClient = Builder.Build<FtpWebClient>(@"Init\FtpWebClientTest.xml", "FtpWebClient");
                FtpWebClient = Builder.Build<FtpWebClient>(@"Init\FtpWebClient2.xml", "FtpWebClient");
                FtpWebClient.Parent = this;
                FtpWebClient.Init();
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }

        public void Compare()
        {
            if(_etalonDirInfo != null)
                WalkDirectoryTree(_etalonDirInfo);

            ConsoleSync.WriteLineT($"BadFilesCnt: {WrongFiles.Count}");
            foreach(var fi in WrongFiles)
                ConsoleSync.WriteLineT(fi);

            ConsoleSync.WriteLineT($"BadFilesInfoCnt: {WrongFtpReturnInfo.Count}");
            foreach (var fi in WrongFtpReturnInfo)
                ConsoleSync.WriteLineT(fi.ToString());
        }

        private void WalkDirectoryTree(DirectoryInfo dir)
        {

            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            // ConsoleSync.WriteLineT($"StackDepth: {_stackDepthCount}");

            // First, process all the files directly under this folder
            try
            {
                files = dir.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                // Log.Add(e.Message);
                SendException(e);
            }
            catch (DirectoryNotFoundException e)
            {
                SendException(e);
            }

            // Files operate at First
            if (files != null)
            {
                // try to Get Files and Dirs

                var remDirName = dir.FullName.Replace(EtalonDir, "");
                var dirDetails = FtpWebClient.GetListDirectoryDetails(remDirName);

                var localfiles = files.Select(di => di.Name).ToList();
                var remoteFiles = dirDetails.ListDirectoryDetails
                    .Where(di => di.IsFile)
                    .Select(di => di.Name).ToList();

                var filesToDelete = remoteFiles.Except(localfiles).ToList();
                foreach (var f in filesToDelete)
                {
                    var name = string.Join(@"\", remDirName, f);

                     ConsoleSync.WriteReadLineT($"DELETE EXTRA FILE: {name}");
                    
                    FtpWebClient.DeleteFile(name);
                }
                foreach (FileInfo fi in files)
                {
                    FilesCount++;

                    ConsoleSync.WriteLineT(Environment.NewLine + "{0} {1}", fi.FullName, fi.Length);

                    // Thread.Sleep(100);

                    try
                    {
                        //var hash12 = GetHashSha1(fileBytes);
                        //if(string.Compare(hash11, hash12, StringComparison.Ordinal) != 0)
                        //    ConsoleSync.WriteLineT("SHA11: {0} SHA12: {1}", hash11, hash12);

                        var remotePath = fi.FullName.Replace(EtalonDir, "");

                        //var fileExt = fi.Extension;
                        // var text = TextFileExtensions.Contains(fileExt);

                        var hash21 = Crypto.GetHashSha1FromFileLinesSplitedInBytes(fi.FullName);

                        var ftpReturn = FtpWebClient.DownLoadFile(remotePath);

                        if (ftpReturn.FtpOperationStatus != FtpOperationStatus.Ok)
                        {
                            if (ftpReturn.FtpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                            {
                                ConsoleSync.WriteLineT($"FILE NOT FOUND: {remotePath}, FtpReturn: {ftpReturn}" + Environment.NewLine);

                               // ftpReturn.FtpOperationStatus = FtpOperationStatus.FileNotFound;
                                WrongFtpReturnInfo.Add(ftpReturn);
                                WrongFiles.Add(fi.FullName);
                                ErrorCount++;

                                // UpLoad
                                var ftpReturnUpload = FtpWebClient.UpLoadFile(fi.FullName, remotePath);
                                if (ftpReturnUpload.FtpOperationStatus != FtpOperationStatus.Ok)
                                {
                                    WrongFtpReturnInfo.Add(ftpReturn);
                                    WrongFiles.Add(fi.FullName);
                                    ErrorCount++;
                                }

                                continue;
                            }

                            WrongFtpReturnInfo.Add(ftpReturn);
                            WrongFiles.Add(fi.FullName);
                            ErrorCount++;
                            continue;
                        }

                        var bs = ftpReturn.Content.ToLinesInBytes();
                        var hash22 = Crypto.GetHashSha1(bs);

                        if (hash21 != hash22)
                        {
                            ConsoleSync.WriteLineT($"Error: Hash1: {hash21}, Hash2: {hash22}" + Environment.NewLine);

                            ftpReturn.FtpOperationStatus = FtpOperationStatus.BadHash;
                            WrongFtpReturnInfo.Add(ftpReturn);
                            WrongFiles.Add(fi.FullName);
                            ErrorCount++;

                            // DELETE Wrong File
                            var ftpReturnDelete = FtpWebClient.DeleteFile(remotePath);
                            if (ftpReturnDelete.FtpOperationStatus != FtpOperationStatus.Ok)
                            {
                                WrongFtpReturnInfo.Add(ftpReturn);
                                WrongFiles.Add(fi.FullName);
                                ErrorCount++;
                                continue;
                            }
                            // UPLOAD Right File
                            var ftpReturnUpload = FtpWebClient.UpLoadFile(fi.FullName, remotePath);
                            if (ftpReturnUpload.FtpOperationStatus != FtpOperationStatus.Ok)
                            {
                                WrongFtpReturnInfo.Add(ftpReturn);
                                WrongFiles.Add(fi.FullName);
                                ErrorCount++;
                            }
                        }
                        else
                        {
                            // ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                            SuccessCount++;
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        // ex.ToConsoleSync();
                        ExceptionError++;
                        SendException(ex);
                    }
                    catch (Exception ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        SendException(ex);
                        ExceptionError++;
                    }

                    //  Console.ReadLine();
                }
            }

            try
            {
                // Now find all the subdirectories under this directory.
                subDirs = dir.GetDirectories();

                var remDirName = dir.FullName.Replace(EtalonDir, "");
                var remChildDirs = FtpWebClient.GetListDirectoryDetails(remDirName);

                var localDirs = subDirs.Select(di => di.Name).ToList();
                var remoteDirs = remChildDirs.ListDirectoryDetails
                    .Where(di => di.IsDirectory)
                    .Select(di => di.Name).ToList();

                var dirToCreate = localDirs.Except(remoteDirs).ToList();
                foreach (var di in dirToCreate)
                {
                    var dr = Path.Combine(dir.FullName, di);
                    dr = dr.Replace(EtalonDir, "");
                    FtpWebClient.MakeDirectory(dr);
                }

                var dirToWarnings = remoteDirs.Except(localDirs).ToList();
                foreach (var di in dirToWarnings)
                {
                    var d = string.Join(@"\", dir.FullName, di);
#if DEBUG
                    ConsoleSync.WriteReadLineT($"WARNING DIR: {d}" );
#else
                    ConsoleSync.WriteLineT($"WARNING DIR: {d}" );
#endif
                    if (!IsExtraDirectoryToDelete)
                        continue;

                    d = d.Replace(EtalonDir, "");
                    FtpWebClient.RemoveDirectoryTree(d);
                }

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    ConsoleSync.WriteLineT($"StackDepth: {++StackDepthCount}");

                    if (DirExceptionList.Contains(dirInfo.FullName))
                    {
                        ConsoleSync.WriteLineT($"DirName: {dirInfo.Name}" +
                                               Environment.NewLine +
                                               $"DirFullName: {dirInfo.FullName}");

                        ConsoleSync.WriteLineT($"Skip Dir: {dirInfo.FullName} ");

                        continue;
                    }
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"{ex.Message}"); 
                // ex.ToConsoleSync();
                SendException(ex);
                // ExceptionError++;
            }
        }
        private void WalkDirectoryTree3(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            // ConsoleSync.WriteLineT($"StackDepth: {_stackDepthCount}");

            // First, process all the files directly under this folder
            try
            {
                files = dir.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                // Log.Add(e.Message);
                SendException(e);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    FilesCount++;
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().

                    ConsoleSync.WriteLineT("{0} {1}", fi.FullName, fi.Length);

                    // Thread.Sleep(100);

                    try
                    {
                        //var hash12 = GetHashSha1(fileBytes);
                        //if(string.Compare(hash11, hash12, StringComparison.Ordinal) != 0)
                        //    ConsoleSync.WriteLineT("SHA11: {0} SHA12: {1}", hash11, hash12);

                        var remotePath = fi.FullName.Replace(EtalonDir, "");
                        var fileExt = fi.Extension;

                        var text = TextFileExtensions.Contains(fileExt);

                        // var hash211 = FileStringCalculateSha1_2(fi.FullName);
                        var hash21 = Crypto.GetHashSha1FromFileLinesSplitedInBytes(fi.FullName);
                        
                        // var hash22 = FtpFileStringCalculateSha1(remotePath);

                        // var hash22 = FtpWebClient.DownLoadFileAndCalcSha1(remotePath);

                        var ftpReturn = FtpWebClient.DownLoadFile(remotePath);
                        var bs = ftpReturn.Content.ToLinesInBytes();
                        var hash22 = Crypto.GetHashSha1(bs);

                        // if(hash22 == string.Empty)

                        if (hash21 != hash22)
                        {
                            //var fileBytes = File.ReadAllBytes(fi.FullName);
                            //var hash11 = GetHashSha1(fileBytes);
                            ErrorCount++;

                            var hash11 = Crypto.GetHashSha1FromFileAllBytes(fi.FullName);

                            var fileInBytes = FtpWebClient.DownLoadFileInBytes(remotePath);

                            var hash12 = Crypto.GetHashSha1(fileInBytes);
                            if (hash11 != hash12)
                            {
                                ConsoleSync.WriteLineT($"Error: Hash1: {hash11}, Hash2: {hash21}" + Environment.NewLine);
                                WrongFiles.Add(fi.FullName);
                                ErrorCount++;

                                // Stream + Md5
                                //var hash12 = CalculateMd5(fi.FullName);
                                //var fileStream = FtpWebClient.DownLoadFileInStream(remotePath);
                                //var hash22 = GetHashMd5(fileStream);
                            }
                            else
                            {
                                //  ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                                SuccessCount++;
                            }
                        }
                        else
                        {
                            // ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                            SuccessCount++;
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        ex.ToConsoleSync();
                        ExceptionError++;
                    }
                    catch (Exception ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        ex.ToConsoleSync();
                        ExceptionError++;
                    }

                    //  Console.ReadLine();
                }
            }

            try
            {
                // Now find all the subdirectories under this directory.
                subDirs = dir.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    ConsoleSync.WriteLineT($"StackDepth: {++StackDepthCount}");

                    // Resursive call for each subdirectory.
                    WalkDirectoryTree3(dirInfo);
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"{ex.Message}"); 
                ex.ToConsoleSync();

                ExceptionError++;
            }
        }
        private void WalkDirectoryTree1(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            // ConsoleSync.WriteLineT($"StackDepth: {_stackDepthCount}");

            // First, process all the files directly under this folder
            try
            {
                files = dir.GetFiles("*.*");
            }
                // This is thrown if even one of the files requires permissions greater
                // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                // Log.Add(e.Message);
                SendException(e);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    FilesCount++;
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().

                    ConsoleSync.WriteLineT("{0} {1}", fi.FullName, fi.Length);

                   // Thread.Sleep(100);

                    try
                    {
                        var fileBytes = File.ReadAllBytes(fi.FullName);
                        var hash11 = GetHashSha1(fileBytes);
                        //var hash12 = GetHashSha1(fileBytes);
                        //if(string.Compare(hash11, hash12, StringComparison.Ordinal) != 0)
                        //    ConsoleSync.WriteLineT("SHA11: {0} SHA12: {1}", hash11, hash12);

                        var remotePath = fi.FullName.Replace(EtalonDir, "");

                        var fileInBytes = FtpWebClient.DownLoadFileInBytes(remotePath);
                        if (fileInBytes != null)
                        {
                            var hash21 = GetHashSha1(fileInBytes);
                            if (hash11 != hash21)
                            {
                                //ConsoleSync.WriteLineT($"Error: Hash1: {hash11}, Hash2: {hash21}" + Environment.NewLine);
                                //ErrorCount++;

                                // Stream + Md5
                                //var hash12 = CalculateMd5(fi.FullName);
                                //var fileStream = FtpWebClient.DownLoadFileInStream(remotePath);
                                //var hash22 = GetHashMd5(fileStream);

                                var hash12 = FileStringCalculateSha1(fi.FullName);
                                // var hash22 = FtpFileStringCalculateSha1(remotePath);

                               // Thread.Sleep(1000);

                                var hash22 = FtpWebClient.DownLoadFileAndCalcSha1(remotePath);
                                // if(hash22 == string.Empty)

                                if (hash12 != hash22)
                                {
                                    ConsoleSync.WriteLineT($"Error2: Hash1: {hash11}, Hash2: {hash21}" +
                                                           Environment.NewLine);
                                    ErrorCount++;
                                }
                                else
                                {
                                   // ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                                    SuccessCount++;
                                }
                            }
                            else
                            {
                              //  ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                                SuccessCount++;
                            }
                        }
                        else
                        {
                            ErrorCount++;
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        foreach (var e in ex.ToList())
                            ConsoleSync.WriteLineT(e);
                        ExceptionError++;
                    }
                    catch (Exception ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        foreach (var e in ex.ToList())
                            ConsoleSync.WriteLineT(e);
                        ExceptionError++;
                    }
                    
                    //  Console.ReadLine();
                }
            }

            try
            {
                // Now find all the subdirectories under this directory.
                subDirs = dir.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    ConsoleSync.WriteLineT($"StackDepth: {++StackDepthCount}");

                    // Resursive call for each subdirectory.
                    WalkDirectoryTree1(dirInfo);

                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"{ex.Message}"); 
                foreach (var e in ex.ToList())
                    ConsoleSync.WriteLineT(e);

                ExceptionError++;
            }
        }

        private void WalkDirectoryTree2(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            // ConsoleSync.WriteLineT($"StackDepth: {_stackDepthCount}");

            // First, process all the files directly under this folder
            try
            {
                files = dir.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                // Log.Add(e.Message);
                SendException(e);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    FilesCount++;
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().

                    ConsoleSync.WriteLineT("{0} {1}", fi.FullName, fi.Length);

                    // Thread.Sleep(100);

                    try
                    {
                        
                        //var hash12 = GetHashSha1(fileBytes);
                        //if(string.Compare(hash11, hash12, StringComparison.Ordinal) != 0)
                        //    ConsoleSync.WriteLineT("SHA11: {0} SHA12: {1}", hash11, hash12);

                        var remotePath = fi.FullName.Replace(EtalonDir, "");

                        var hash21 = FileStringCalculateSha1_2(fi.FullName);
                        // var hash22 = FtpFileStringCalculateSha1(remotePath);

                        // Thread.Sleep(1000);

                        var hash22 = FtpWebClient.DownLoadFileAndCalcSha1(remotePath);
                        // if(hash22 == string.Empty)

                        if (hash21 != hash22)
                        {

                            var fileBytes = File.ReadAllBytes(fi.FullName);
                            var hash11 = GetHashSha1(fileBytes);

                            var fileInBytes = FtpWebClient.DownLoadFileInBytes(remotePath);
                            if (fileInBytes != null)
                            {
                                var hash12 = GetHashSha1(fileInBytes);
                                if (hash11 != hash12)
                                {
                                    ConsoleSync.WriteLineT($"Error: Hash1: {hash11}, Hash2: {hash21}" + Environment.NewLine);
                                    ErrorCount++;

                                    // Stream + Md5
                                    //var hash12 = CalculateMd5(fi.FullName);
                                    //var fileStream = FtpWebClient.DownLoadFileInStream(remotePath);
                                    //var hash22 = GetHashMd5(fileStream);


                                }
                                else
                                {
                                    //  ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                                    SuccessCount++;
                                }
                            }
                            else
                            {
                                ErrorCount++;
                            }

                            //ConsoleSync.WriteLineT($"Error2: Hash1: {hash11}, Hash2: {hash21}" +
                            //                       Environment.NewLine);
                            ErrorCount++;
                        }
                        else
                        {
                            // ConsoleSync.WriteLineT("Ok {0} {1}" + Environment.NewLine, fi.FullName, fi.Length);
                            SuccessCount++;
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        foreach (var e in ex.ToList())
                            ConsoleSync.WriteLineT(e);
                        ExceptionError++;
                    }
                    catch (Exception ex)
                    {
                        // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                        foreach (var e in ex.ToList())
                            ConsoleSync.WriteLineT(e);
                        ExceptionError++;
                    }

                    //  Console.ReadLine();
                }
            }

            try
            {
                // Now find all the subdirectories under this directory.
                subDirs = dir.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    ConsoleSync.WriteLineT($"StackDepth: {++StackDepthCount}");

                    // Resursive call for each subdirectory.
                    WalkDirectoryTree2(dirInfo);

                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"{ex.Message}"); 
                ex.ToConsoleSync();

                ExceptionError++;
            }
        }
        private string GetHashSha1(byte[] bytes)
        {
            string hash;
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
            return hash;
        }

        private string GetHashMd5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private string CalculateMd5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private string CalculateMd5_1(string filename)
        {
                using (var stream = File.OpenRead(filename))
                {
                    return Crypto.GetHashMd5(stream);           
                }
        }

        private string FileStringCalculateSha1(string filename)
        {
            var lst = new List<string>();
            
                using (var stream = File.OpenRead(filename))
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lst.Add(line);
                    }
                }

                byte[] dataAsBytes = lst.SelectMany(s =>
                            Encoding.UTF8.GetBytes(s
                            // + Environment.NewLine
                            )).ToArray();

                return GetHashSha1(dataAsBytes);
        }

        private string FileStringCalculateSha1_2(string filename)
        {
            try
            {
                string content;

                using (var stream = File.OpenRead(filename))
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }

                var dataAsBytes = content.ToLinesInBytes();

                return GetHashSha1(dataAsBytes);
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
                return string.Empty;
            }          
        }

        private string FtpFileStringCalculateSha1(string remotepath)
        {
            var lst = new List<string>();
            try
            {
                var stream = FtpWebClient.DownLoadFileInStream(remotepath);

                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lst.Add(line);
                    }
                }
                byte[] dataAsBytes = lst.SelectMany(s =>
                            Encoding.UTF8.GetBytes(s
                            // + Environment.NewLine
                            )).ToArray();

                return GetHashSha1(dataAsBytes);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.ToList())
                    ConsoleSync.WriteLineT(e);

                return string.Empty;
            }
            catch (Exception ex)
            {
                foreach (var e in ex.ToList())
                    ConsoleSync.WriteLineT(e);

                return string.Empty;
            }
        }

        private byte[] ContentToLinesInBytes(string content)
        {
            try
            {
                return content.ToLinesInBytes();
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
                return null;
            }           
        }

        public override string Key => Code;
    }
}
