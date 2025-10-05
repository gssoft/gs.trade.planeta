using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Contexts;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Ftp;
using GS.Interfaces;
using GS.Serialization;
using GS.Works;
using GS.WorkTasks;

namespace RecursiveFilesCmp_02
{
    public class FtpFilesComparer : Context0
    {
        public string EtalonDir { get; set; }
        public string ProductionDir { get; set; }
        public int TimeIntervalToRunInHours { get; set; }
        private int _timeIntToRun;
        public bool IsExtraDirectoryToDelete { get; set; }
        public List<string> TextFileExtensions { get; set; }
        public List<string> BinFileExtensions { get; set; }
        public List<string> DirExceptionList { get; set; }

        public List<string> FilesToDelete { get; set; }
        public List<string> FilesToUpload { get; set; }
        public List<string> FilesToChange { get; set; }

        [XmlIgnore]
        public List<FileOperations> FileOperationsTotal { get; set; }

        private DirectoryInfo _etalonDirInfo;

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

        // protected List<FtpReturn> FtpFileReturns { get; set; }

        protected DateTime DateTimeStart { get; set; }
        protected DateTime DateTimeComplete { get; set; }

        public override string ToString()
        {
            return $"Type: {TypeFullName}, EtalonDir: {EtalonDir}, TimeIntInHours: {_timeIntToRun}";
            //, ErrorCount: {ErrorCount}, SuccessCount: {SuccessCount}";
        }
        public override void  Init()
        {
            base.Init();

            ErrorCount = 0;
            SuccessCount = 0;
            ExceptionError = 0;

           // FtpFileReturns = new List<FtpReturn>();
            FileOperationsTotal = new List<FileOperations>();

           // WhoAreYou();

            try
            {
                 _etalonDirInfo = new DirectoryInfo(EtalonDir);
                
                var cnf = Builder.Build<Config>(@"Init\Config.xml", "Config");

                FtpWebClient = Builder.Build<FtpWebClient>(cnf.FtpWebClient, "FtpWebClient");

                FtpWebClient.Parent = this;
                FtpWebClient.Init();

                _timeIntToRun = TimeIntervalToRunInHours <= 0 
                    ? 3
                    : TimeIntervalToRunInHours;

                var work  = new Work1<IEventArgs>
                {
                    Code = "Work." + Code,
                    TimeInterval = 1000 * 60 * 60 * _timeIntToRun,
                    MainFunc = () =>
                    {
                        // VerifyConcreteFiles();

                        var ret = Compare();
                        //if (ret > 0)
                        //    Compare();

                        // Test_Compare();
                        return true;
                    }
                };
                var task = new WorkTask4
                {
                    Code = GetType().FullName + "@CompareFilesWork",
                    TimeInterval = 1000 * 60 * 60 * _timeIntToRun
                    //TimeInterval = 1000 * 30
                };
                task.Works.Register(work);
                WorkTasks.Register(task);

                //var work2 = new Work1<IEventArgs>
                //{
                //    Code = "Work2." + Code,
                //    TimeInterval = 1000 * 60 * 5 * 1,
                //    MainFunc = () =>
                //    {
                //        // VerifyConcreteFiles();
                //        // Compare();
                //        Test_Compare2();
                //        return true;
                //    }
                //};
                //var task2 = new WorkTask4
                //{
                //    Code = GetType().FullName + "@CompareFilesWork2",
                //    TimeInterval = 1000 * 60 * 2 //* 60
                //};
                //task2.Works.Register(work2);
                //WorkTasks.Register(task2);

                WorkTasks.Parent = this;

                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,  "Init()", "Complete", ToString() );
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }
        private void Clear()
        {
            ErrorCount = 0;
            SuccessCount = 0;
            ExceptionError = 0;

            // FtpFileReturns?.Clear();

           // Clear(FtpFileReturns);
            Clear(FileOperationsTotal);
#if DEBUG
            Evlm3(EvlResult.INFO, EvlSubject.INIT, 
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Complete", "");
#endif
        }
        private void Clear(ICollection<FtpReturn> lst)
        {
            if (lst == null)
                return;
            foreach (var i in lst)
                i.Clear();
            lst.Clear();
#if DEBUG
            Evlm2(EvlResult.INFO, EvlSubject.INIT,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                "Complete", " Clear FtpFileTransactions");
#endif
        }
        private void Clear(ICollection<FileOperations> lst)
        {
            if (lst == null)
                return;
            foreach (var i in lst)
                i.Clear();
            lst.Clear();
#if DEBUG
            Evlm3(EvlResult.INFO, EvlSubject.INIT,
                System.Reflection.MethodBase.GetCurrentMethod().Name,
                "Complete", " Clear FtpFileTransactions");
#endif
        }
        public int Compare()
        {
            int badFilesCnt = 0;
            try
            {
                DateTimeStart = DateTime.Now;

                Evlm2(EvlResult.INFO, EvlSubject.INIT, TypeFullName, GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"Start at {DateTimeStart.ToString("G")}", ToString());

                Clear();

                if (_etalonDirInfo != null)
                    WalkDirectoryTree(_etalonDirInfo);

                badFilesCnt = TotalBadFilesReport();

                Clear();

                DateTimeComplete = DateTime.Now;
                var elapsed = (DateTimeComplete - DateTimeStart).ToString("g");

                var result = badFilesCnt > 0 ? EvlResult.WARNING : EvlResult.SUCCESS; 

                Evlm2(EvlResult.INFO, EvlSubject.INIT,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"Start at { DateTimeStart.ToString("G")}, " + 
                    $"Complete at {DateTimeComplete.ToString("G")}, Elpased: {elapsed}, " +
                    $"Next Run at {DateTimeComplete.AddHours(_timeIntToRun)}",
                    $"Comparer {ToString()}, FtpClient {FtpWebClient?.ToString()}" );
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
            return badFilesCnt;
        }
        public void Test_Compare()
        {
            Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, GetType().FullName, GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", "");
            foreach (var i in Enumerable.Range(1,3))
            {
                ConsoleSync.WriteLineT($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Working ...");
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }
            Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, GetType().FullName, GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Complete", "");
        }
        public void Test_Compare2()
        {
            Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, GetType().FullName, GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", "");
            foreach (var i in Enumerable.Range(1, 2))
            {
                ConsoleSync.WriteLineT($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Working ...");
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }
            Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, GetType().FullName, GetType().Name,
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Complete", "");
        }

//        public void VerifyConcreteFiles()
//        {
//#if DEBUG
//            Evlm3(EvlResult.INFO, EvlSubject.PROGRAMMING, 
//                    System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", "");
//#endif
//            var fileReturns = new List<FtpReturn>();    
//            try
//            {
//                if (FilesToDelete?.Count > 0)
//                {
//                    Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, TypeFullName, TypeName,
//                        System.Reflection.MethodBase.GetCurrentMethod().Name,
//                        $"DELETE EXTRA FILES. COUNT: {FilesToDelete?.Count}", "");

//                    //foreach (var fi in FilesToDelete)
//                    //    DeleteIfExist(fi);

//                fileReturns.AddRange(FilesToDelete.Select(DeleteIfExist).Where(r => r!=null));

//                }
//                if (FilesToChange?.Count > 0)
//                {
//                    Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, System.Reflection.MethodBase.GetCurrentMethod().Name,
//                        $"FILES TO CHANGE. COUNT: {FilesToChange?.Count}", "");

//                    fileReturns.AddRange(FilesToChange.Select(FileProcessing).Where(r => r != null));

//                }
//                if (FilesToUpload?.Count > 0)
//                {
//                    {
//                        Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, System.Reflection.MethodBase.GetCurrentMethod().Name,
//                            $"FILES TO UPLOAD. COUNT: {FilesToUpload?.Count}", "");

//                        //foreach (var fi in FilesToUpload)
//                        //    FileProcessing(fi);

//                        fileReturns.AddRange(FilesToUpload.Select(FileProcessing).Where(r => r != null));
//                    }
//                }

//                Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, System.Reflection.MethodBase.GetCurrentMethod().Name,
//                            $"FILES PROCESSED COUNT: {fileReturns.Count}", "" );

//                foreach (var fr in fileReturns)
//                    Evlm2(EvlResult.INFO, EvlSubject.PROGRAMMING, System.Reflection.MethodBase.GetCurrentMethod().Name,
//                            "FILE PROCESSED:", fr.ToString());
//            }
//            catch (Exception ex)
//            {
//                SendException(ex);

//                Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING, 
//                        System.Reflection.MethodBase.GetCurrentMethod().Name,  $"Exception: {ex.Message}", "");
//            }
//        }
        private FtpReturn DeleteIfExist(string filepath)
        {
#if DEBUG
            Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY,
                System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", $"{filepath}");
#endif
            try
            {
                var dirpath = Path.GetDirectoryName(filepath);
                var filename = Path.GetFileName(filepath);

                var dirDetails = FtpWebClient.GetListDirectoryDetails(dirpath);
                var remoteFiles = dirDetails.ListDirectoryDetails
                        .Where(di => di.IsFile)
                        .Select(di => di.Name).ToList();

                if (remoteFiles.Contains(filename))
                {
#if DEBUG
                    Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY, 
                        System.Reflection.MethodBase.GetCurrentMethod().Name, @"TRY TO DELETE FILE", $"{filepath}");
#endif
                    return FtpWebClient.DeleteFile(filepath);
                }

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, @"FILE IS NOT FOUND", $"{filepath}");
            }
            catch (Exception ex)
            {
                SendException(ex);

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"TRY TO DELETE: {filepath}", $"Exception: {ex.Message}");
            }
            return null;
        }
        private void WalkDirectoryTree(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            var dirFullName = dir.FullName;
            var relativeDirPath = dir.FullName.Replace(EtalonDir, "");
            if (relativeDirPath.HasNoValue())
                relativeDirPath = @"\";
            try
            {
#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"NEW DIR TO SCAN: {relativeDirPath}", "");
#endif
                var dirFileReturns = new List<FtpReturn>();
                var dirFileOpers = new List<FileOperations>();
           
                // First, process all the files directly under this folder
                try
                {
                    files = dir.GetFiles("*.*");
                }
                // This is thrown if even one of the files requires permissions greater
                // than the application provides.
                catch (UnauthorizedAccessException e)
                {
                    Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING, "GET FILES", $"DIR: {dir.FullName}", e.Message);
                    SendException(e);
                }
                catch (DirectoryNotFoundException e)
                {
                    Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                                            System.Reflection.MethodBase.GetCurrentMethod().Name + ", GET FILES",
                                            $"DIR: {dir.FullName}", e.Message);
                    SendException(e);
                }
                // Files operate at First DELETE EXTRA FILES
                if (files != null)
                {
                    // DIR: START EXTRA FILE DETECTING
                    DirExtraFilesDetecting(dir,files,relativeDirPath, dirFileReturns, dirFileOpers);

                    // DIR: FILES PROCESING
                    // DirFilesProcessing(relativeDirPath, files, dirFileReturns);
                    DirFilesProcessing(relativeDirPath, files, dirFileOpers);
                }
                // DIR: START SUBDIRS PROCESSING

                subDirs = dir.GetDirectories();

                SubDirsProcessing(dir, subDirs, relativeDirPath, dirFileReturns, dirFileOpers);

                // DirBadFilesReport(relativeDirPath, dirFileReturns);
                DirBadFilesReport(relativeDirPath, dirFileOpers);
                // PUSH TO NEXT DIR EXCEPT SKIP DIRECTORIES
                foreach (var dirInfo in subDirs)
                {
                    if (DirExceptionList.Contains(dirInfo.FullName))
                    {
                        var skipDirName = dirInfo.FullName.Replace(EtalonDir, "");

                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}", $"SKIP SUBDIR: {skipDirName}");

                        continue;
                    }
                    WalkDirectoryTree(dirInfo);
                }             
            }
            catch (Exception ex)
            {
                SendException(ex);
                ExceptionError++;
            }
        }
        private void WalkDirectoryTreeExtraFileOnly(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            var dirFullName = dir.FullName;
            var relativeDirPath = dir.FullName.Replace(EtalonDir, "");
            if (relativeDirPath.HasNoValue())
                relativeDirPath = @"\";
            try
            {
#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"NEW DIR TO SCAN: {relativeDirPath}", "");
#endif
                var dirFileReturns = new List<FtpReturn>();
                var dirFileOpers = new List<FileOperations>();

                // First, process all the files directly under this folder
                try
                {
                    files = dir.GetFiles("*.*");
                }
                // This is thrown if even one of the files requires permissions greater
                // than the application provides.
                catch (UnauthorizedAccessException e)
                {
                    Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING, "GET FILES", $"DIR: {dir.FullName}", e.Message);
                    SendException(e);
                }
                catch (DirectoryNotFoundException e)
                {
                    Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                                            System.Reflection.MethodBase.GetCurrentMethod().Name + ", GET FILES",
                                            $"DIR: {dir.FullName}", e.Message);
                    SendException(e);
                }
                // Files operate at First DELETE EXTRA FILES
                if (files != null)
                {
                    // DIR: START EXTRA FILE DETECTING
                    DirExtraFilesDetecting(dir, files, relativeDirPath, dirFileReturns, dirFileOpers);

                    // DIR: FILES PROCESING
                    DirFilesProcessing(relativeDirPath, files, dirFileOpers);
                }
                // DIR: START SUBDIRS PROCESSING

                subDirs = dir.GetDirectories();

                SubDirsProcessing(dir, subDirs, relativeDirPath, dirFileReturns, dirFileOpers);

                DirBadFilesReport(relativeDirPath, dirFileOpers);
                // PUSH TO NEXT DIR EXCEPT SKIP DIRECTORIES
                foreach (var dirInfo in subDirs)
                {
                    if (DirExceptionList.Contains(dirInfo.FullName))
                    {
                        var skipDirName = dirInfo.FullName.Replace(EtalonDir, "");

                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}", $"SKIP SUBDIR: {skipDirName}");

                        continue;
                    }

                    WalkDirectoryTreeExtraFileOnly(dirInfo);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                ExceptionError++;
            }
        }

        private void SubDirsProcessing(FileSystemInfo dir,
            IEnumerable<DirectoryInfo> subDirs, string relativeDirPath,
            ICollection<FtpReturn> dirFileReturns,
            List<FileOperations> dirFileOperations)
        {
#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}, START SUBDIRS PROCESSING", "");
#endif
            try
            {
                var remDirName = dir.FullName.Replace(EtalonDir, "");
                var ftpResp = FtpWebClient.GetListDirectoryDetails(remDirName);
                if (ftpResp.FtpOperationStatus != FtpOperationStatus.Ok)
                {
                    var f = new FileOperations(ftpResp.FileShortPath);
                    f.Add(ftpResp);
                    FileOperationsTotal.Add(f);
                    // return ftpResp;
                    return;
                }
                var localDirs = subDirs.Select(di => di.Name).ToList();

                var remoteDirs = ftpResp.ListDirectoryDetails
                    .Where(di => di.IsDirectory)
                    .Select(di => di.Name).ToList();

                ftpResp.ListDirectoryDetails.Clear();
                ftpResp.ListDirectoryDetails = null;

                // CREATE DIRS IN REMOTE TARGET
                var dirToCreate = localDirs.Except(remoteDirs).ToList();

                if (dirToCreate.Count > 0)
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}", $"SUBDIRS TO CREATE COUNT: {dirToCreate.Count}");

                foreach (var di in dirToCreate)
                {
                    var dr = Path.Combine(dir.FullName, di);
                    dr = dr.Replace(EtalonDir, "");

                    var fop = new FileOperations(dr);
                    var t = new FileTransaction { Operation = FtpOperation.GetDirectory, Status = FtpOperationStatus.DirNotFound};
                    fop.Add(t);

                    var r = FtpWebClient.MakeDirectory(dr);

                    // r.FtpOperationStatus = FtpOperationStatus.DirNotFound;
                    
                    fop.Add(r);

                    dirFileOperations.Add(fop);
                    FileOperationsTotal.Add(fop);
                }
                // REMOVE EXTRA DIRS IN REMOTE TARGET
                var dirToDelete = remoteDirs.Except(localDirs).ToList();
                if (dirToDelete.Count > 0)
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}", $"SUBDIRS TO DELETE COUNT: {dirToDelete.Count}");

                foreach (var di in dirToDelete)
                {
                    var d = string.Join(@"\", dir.FullName, di);

                    //if (!IsExtraDirectoryToDelete)
                    //    continue;

                    d = d.Replace(EtalonDir, "");

                    RemoveDirectoryTree(d);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                      $"DIR: {relativeDirPath}",
                      $"Exception: {ex.Message}");
            }
        }
        private void DirExtraFilesDetecting(DirectoryInfo dir, FileInfo[] files,  string relativeDirPath,
            List<FtpReturn> dirFileReturns, List<FileOperations> dirFileOperations)
        {

#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {relativeDirPath}",
                    "START EXTRA FILE DETECTING","");
#endif
            try
            {
                var remDirName = dir.FullName.Replace(EtalonDir, "");
                var ftpResp = FtpWebClient.GetListDirectoryDetails(remDirName);
                if (ftpResp.FtpOperationStatus != FtpOperationStatus.Ok)
                {
                    var f = new FileOperations(ftpResp.FileShortPath);
                    f.Add(ftpResp);
                    FileOperationsTotal.Add(f);
                    return;
                }

                var localfiles = files.Select(di => di.Name).ToList();
                var remoteFiles = ftpResp.ListDirectoryDetails
                    .Where(di => di.IsFile)
                    .Select(di => di.Name).ToList();

                var filesToDelete = remoteFiles.Except(localfiles).ToList();
                if (filesToDelete.Count <= 0)
                    return;

                Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"DIR: {relativeDirPath}", $"EXTRA FILES TO DELETE COUNT: {filesToDelete.Count}");

                foreach (var f in filesToDelete)
                {
                    var name = string.Join(@"\", remDirName, f);
//#if DEBUG
//                    Evlm2(EvlResult.WARNING,
//                        EvlSubject.TECHNOLOGY,
//                        System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {relativeDirPath}",
//                        "EXTRA FILE DETECTED", name);
//#endif
                    var rdel = FtpWebClient.DeleteFile(name);
                    if (rdel == null)
                        continue;

                    var r = new FtpReturn
                    {
                        FileShortPath = name,
                        FtpOperation = FtpOperation.ExtraFileScan,
                        FtpOperationStatus = FtpOperationStatus.ExtraFileDetected
                    };

                    var fo = new FileOperations(r.FileShortPath);
                    fo.Add(r);
                    fo.Add(rdel);

                    dirFileOperations.Add(fo);
                    FileOperationsTotal.Add(fo);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                    System.Reflection.MethodBase.GetCurrentMethod().Name + ", EXTRA FILES DETECTING",
                     $"DIR: {relativeDirPath}", ex.Message);
            }
        }
        private void DirBadFilesReport(string relativeDirPath, List<FileOperations> dirFileReturns)
        {
            try
            {
                var dirFilesCnt = dirFileReturns.Count;
                var badfiles = dirFileReturns.Where(r => !r.IsSuccess).ToList();
                var badFilesCnt = badfiles.Count;
                if (badFilesCnt <= 0)
                {
                    Evlm2(EvlResult.SUCCESS,
                       EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                       $"{relativeDirPath}",
                       $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");
                    return;
                }
                Evlm2(EvlResult.WARNING, EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                       //$"DIR: {relativeDirPath}",
                       $"{relativeDirPath}",
                       $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");

                foreach (var f in badfiles)
                {
                    Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                             System.Reflection.MethodBase.GetCurrentMethod().Name, // + $", {f.DirName}",
                             $"{f.DirName}\\ {f.Name}", $"{f.FileOperationsStr}, {f.RelativeName}");
                }               
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.INIT,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          $"{relativeDirPath}",
                          $"Exception: {ex.Message}");
            }
            dirFileReturns.Clear();
        }
        private void DirBadFilesReport(string relativeDirPath, List<FtpReturn> dirFileReturns)
        {
            try
            {
                var dirFilesCnt = dirFileReturns.Count; 
                var badfiles = dirFileReturns.Where(r => !r.IsSuccess).ToList();
                var badFilesCnt = badfiles.Count;
                if (badFilesCnt <= 0)
                {
                    Evlm2(EvlResult.INFO,
                       EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                       $"DIR: {relativeDirPath}", $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");
                    return;
                }
                Evlm2(EvlResult.WARNING,
                       EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                       $"DIR: {relativeDirPath}", $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");

                //Evlm2(EvlResult.WARNING,
                //       EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name + ", Report Start",
                //       $"DIR: {relativeDirPath}", $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");

                foreach (var r in badfiles)
                {
                    var rlst = r.Flatten();
                    foreach (var i in rlst)
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name +
                            $", DIR: {relativeDirPath}",
                            $"{i.FtpOperation}: {i.FileShortPath}, {i.FtpOperationStatus}", i.ToString());
                }
                //Evlm2(EvlResult.WARNING,
                //       EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name + ", Report Complete",
                //       $"DIR: {relativeDirPath}", $"DIR BAD FILES: {badFilesCnt}, DIR TOTALS FILES: {dirFilesCnt}");
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          $"DIR: {relativeDirPath}",
                          $"Exception: {ex.Message}");
            }

            // Clear(dirFileReturns);
            dirFileReturns.Clear();
        }
        private int TotalBadFilesReport()
        {
            var badFilesCnt = 0;
            try
            {
                var filesCnt = FileOperationsTotal.Count;
                var badfiles = FileOperationsTotal.Where(i => !i.IsSuccess).ToList();
                badFilesCnt = badfiles.Count;

                if (badFilesCnt <= 0)
                {
                    Evlm2(EvlResult.SUCCESS,
                       EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "TOTAL REPORT", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");
                    return badFilesCnt;
                }
                //Evlm2(EvlResult.WARNING,
                //       EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                //        "TOTAL REPORT", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");

                foreach (var f in badfiles)
                {
                    Evlm2(EvlResult.WARNING, EvlSubject.INIT, f.Name,
                            System.Reflection.MethodBase.GetCurrentMethod().Name, // + $", {f.DirName}",
                            $"{f.DirName}\\ {f.Name}", $"{f.FileOperationsStr}, {f.RelativeName}");
                }

                Evlm2(EvlResult.WARNING,
                       EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "TOTAL REPORT", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.INIT,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "TOTAL REPORT",
                          $"Exception: {ex.Message}");
            }

            // Clear(fileReturns);
            Clear(FileOperationsTotal);

            return badFilesCnt;
        }

        private int TotalBadFilesReport(ICollection<FileOperations> fileReturns)
        {
            int badFilesCnt = 0;
            try
            {
                var filesCnt = fileReturns.Count;
                var badfiles = fileReturns.Where(r => !r.IsSuccess).ToList();
                badFilesCnt = badfiles.Count;
                if (badFilesCnt <= 0)
                {
                    Evlm2(EvlResult.INFO, EvlSubject.INIT, System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "TOTAL REPORT", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");
                    return badFilesCnt;
                }
                Evlm2(EvlResult.WARNING,EvlSubject.INIT,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                       "TOTAL REPORT START", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");

                foreach (var f in badfiles)
                {
                    Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                        System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {f.DirName}",
                        $"{f.Name}", $"{f.FileOperationsStr}");
                }

                Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "TOTAL REPORT", $"BAD FILES: {badFilesCnt}, TOTALS FILES: {filesCnt}");

                //foreach (var f in FileOperationsTotal.Where(i => !i.IsSuccess))
                //{
                //    Evlm2(EvlResult.WARNING, EvlSubject.INIT,
                //            System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {f.DirName}",
                //            $"{f.Name}", $"{f.FileOperationsStr}");
                //}
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "TOTAL REPORT",
                          $"Exception: {ex.Message}");
            }

            Clear(fileReturns);

            return badFilesCnt;
        }
        private void DirFilesProcessing(string relativeDirPath, FileInfo[] files, List<FileOperations> dirFileOpers )
        {
            try
            {
#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"DIR: {relativeDirPath}, START FILE PROCESSING", "");
#endif
                foreach (FileInfo fi in files)
                {
                    FilesCount++;

                    var remotePath = fi.FullName.Replace(EtalonDir, "");
                    remotePath = remotePath.Substring(1);

                    var ret = FileProcessing2(remotePath);
                    if (ret == null)
                        continue;

                    var foprs = new FileOperations(remotePath);
                    foprs.Add(ret);

                    FileOperationsTotal.Add(foprs);
                    dirFileOpers.Add(foprs);

                    // FtpFileReturns?.Add(ret);
                   // dirFileReturns.Add(ret);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                ExceptionError++;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"DIR: {relativeDirPath}",
                    $"Exception: {ex.Message}");
            }
        }
        private void DirFilesProcessing(string relativeDirPath, FileInfo[] files, List<FtpReturn> dirFileReturns)
        {
            try
            {
#if DEBUG
                Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY, System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"DIR: {relativeDirPath}, START FILE PROCESSING", "");
#endif
                foreach (FileInfo fi in files)
                {
                    FilesCount++;

                    var remotePath = fi.FullName.Replace(EtalonDir, "");
                    remotePath = remotePath.Substring(1);

                    var ret = FileProcessing2(remotePath);
                    if (ret == null)
                        continue;

                    var foprs = new FileOperations(remotePath);
                    foprs.Add(ret);
                    FileOperationsTotal.Add(foprs);

                   // FtpFileReturns?.Add(ret);
                    dirFileReturns.Add(ret);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                ExceptionError++;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    $"DIR: {relativeDirPath}",
                    $"Exception: {ex.Message}");
            }
        }

        //        private FtpReturn FileProcessing(string remotePath)
        //        {
        //            FtpReturn ftpReturn = null;
        //            try
        //            {
        //                var fileFullName = Path.Combine(EtalonDir, remotePath);
        //                remotePath = fileFullName.Replace(EtalonDir, "");

        //                var hash21 = fileFullName.GetHashSha1FromFileLinesBytes();

        //                ftpReturn = FtpWebClient.DownLoadFile(remotePath);

        //                if (ftpReturn.FtpOperationStatus != FtpOperationStatus.Ok)
        //                {
        //                    // if (ftpReturn.FtpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        //                    if (ftpReturn.FtpOperationStatus == FtpOperationStatus.FileNotFound)
        //                    {

        //                        ErrorCount++;

        //                        // UpLoad
        //                        var ftpReturnUpload = FtpWebClient.UpLoadFile(fileFullName, remotePath);

        //                        if (ftpReturnUpload.FtpOperationStatus == FtpOperationStatus.Ok)
        //                            return ftpReturnUpload;

        //                        ErrorCount++;
        //                        return ftpReturnUpload;
        //                    }


        //                    ErrorCount++;
        //                    return ftpReturn;
        //                }

        //                var bs = ftpReturn.Content.ToLinesInBytes();
        //                var hash22 = bs.GetHashSha1();

        //                ftpReturn.Content = string.Empty; // for GS

        //                if (hash21 != hash22)
        //                {
        //                    ftpReturn.HashStatus = HashStatus.Bad;

        //                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
        //                                    System.Reflection.MethodBase.GetCurrentMethod().Name,
        //                                                    "INVALID HASH", ftpReturn.ToString());


        //                    ErrorCount++;

        //                    // DELETE Wrong File
        //                    var ftpReturnDelete = FtpWebClient.DeleteFile(remotePath);
        //                    if (ftpReturnDelete.FtpOperationStatus != FtpOperationStatus.Ok)
        //                    {
        //                        ErrorCount++;
        //                        return ftpReturnDelete;
        //                    }
        //                    // UPLOAD Right File
        //                    var ftpReturnUpload = FtpWebClient.UpLoadFile(fileFullName, remotePath);
        //                    if (ftpReturnUpload.FtpOperationStatus != FtpOperationStatus.Ok)
        //                    {
        //                        ErrorCount++;
        //                    }
        //                    return ftpReturnUpload;
        //                }

        //                // Hash is OK
        //                SuccessCount++;
        //                ftpReturn.HashStatus = HashStatus.Ok;
        //#if DEBUG
        //                    Evlm3(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, 
        //                            System.Reflection.MethodBase.GetCurrentMethod().Name,
        //                                    "HASH IS OK", ftpReturn.ToString());
        //#endif

        //            }
        //            catch (Exception ex)
        //            {
        //                // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
        //                SendException(ex);
        //                ExceptionError++;

        //                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
        //                       System.Reflection.MethodBase.GetCurrentMethod().Name,
        //                                $"Exception: {ex.Message}", remotePath);
        //            }
        //            return ftpReturn;
        //        }
        private FtpReturn FileProcessing2(string remotePath)
        {
           // var fileOperations = new FileOperations(remotePath);

            FtpReturn ftpReturn = null;
            var dirname = Path.GetDirectoryName(remotePath);
            var filename = Path.GetFileName(remotePath);
            try
            {
                var fileFullName = Path.Combine(EtalonDir, remotePath);
                remotePath = fileFullName.Replace(EtalonDir, "");

                // var hash21 = fileFullName.GetHashSha1FromFileLinesBytes();

                var hash1 = Crypto.GetHashSha1FromFileBytesWithoutrn(fileFullName);

                ftpReturn = FtpWebClient.DownLoadFile(remotePath);

                if (ftpReturn.FtpOperationStatus != FtpOperationStatus.Ok)
                {
                    // NOT FOUND
                    if (ftpReturn.FtpOperationStatus == FtpOperationStatus.FileNotFound)
                    {
                        // UpLoad
                        var ftpReturnUpload = FtpWebClient.UpLoadFile(fileFullName, remotePath);
                       
                        ftpReturn.FtpReturn2 = ftpReturnUpload;
                        return ftpReturn;
                    }
                    return ftpReturn;
                }

                var bs = GSstring.ToBytesWithoutrn(ftpReturn.Content);
                var hash2 = bs.GetHashSha1();

                ftpReturn.Content = string.Empty; // for GS

                if (hash1 != hash2)
                {
                    ftpReturn.HashStatus = HashStatus.Bad;
                    ftpReturn.FtpOperationStatus = FtpOperationStatus.BadHash;

                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                    System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                                                    $"INVALID HASH. FILE: {filename}", ftpReturn.ToString());                
                    ErrorCount++;

                    // DELETE Wrong File
                    var ftpReturnDelete = FtpWebClient.DeleteFile(remotePath);
                    ftpReturn.FtpReturn2 = ftpReturnDelete;
                    if (ftpReturnDelete.FtpOperationStatus != FtpOperationStatus.Ok)
                    {
                       ErrorCount++;
                       return ftpReturn;
                    }
                    // UPLOAD Right File
                    var ftpReturnUpload = FtpWebClient.UpLoadFile(fileFullName, remotePath);
                    ftpReturn.FtpReturn2.FtpReturn2 = ftpReturnUpload;
                    if (ftpReturnUpload.FtpOperationStatus != FtpOperationStatus.Ok)
                    {
                        ErrorCount++;
                    }
                    return ftpReturn;
                }

                // Hash is OK
                SuccessCount++;
                ftpReturn.HashStatus = HashStatus.Ok;
#if DEBUG
                Evlm3(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                                $"HASH IS OK, FILE: {filename}", ftpReturn.ToString());
#endif
            }
            catch (Exception ex)
            {
                // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                SendException(ex);
                ExceptionError++;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                       System.Reflection.MethodBase.GetCurrentMethod().Name,
                                $"Exception: {ex.Message}", remotePath);
            }
            return ftpReturn;
        }
        public FtpReturn RemoveDirectoryTree(string path)
        {
            var ftpReturn = new FtpReturn
            {
                FileShortPath = path,
                FileFullName = path,
                FtpOperation = FtpOperation.RemoveDirectoryTree
            };
            try
            {
#if DEBUG
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeFullName, TypeFullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", path);
#endif
                var ftpResp = FtpWebClient.GetListDirectoryDetails(path);
                if (ftpResp.FtpOperationStatus != FtpOperationStatus.Ok)
                {
                    var f = new FileOperations(ftpResp.FileShortPath);
                    f.Add(ftpResp);
                    FileOperationsTotal.Add(f);
                    return ftpResp;
                }
                foreach (var i in ftpResp.ListDirectoryDetails.Where(i => i.IsFile))
                {
                    var name = string.Join(@"\", path, i.Name);
                    var re = FtpWebClient.DeleteFile(name);

                    var fo = new FileOperations(name);
                    fo.Add(FtpOperation.ExtraFileScan, FtpOperationStatus.ExtraFileDetected );
                    fo.Add(re);
                    FileOperationsTotal.Add(fo);
                }
                foreach (var i in ftpResp.ListDirectoryDetails.Where(i => i.IsDirectory))
                {
                    var name = string.Join(@"\", path, i.Name);
                    var re = RemoveDirectoryTree(name);
                    //var f = new FileOperations(re.FileShortPath);
                    //f.Add(re);
                    //FileOperationsTotal.Add(f);
                }
                var r = FtpWebClient.RemoveDirectory(path);
                var fop = new FileOperations(path);
                fop.Add(FtpOperation.ExtraDirScan, FtpOperationStatus.ExtraDirDetected);
                fop.Add(r);
                FileOperationsTotal.Add(fop);

                return r;
            }
            catch (Exception ex)
            {
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.RemoveDirectoryTreeFailure;
                ftpReturn.Exception = ex;

                FileOperationsTotal.Add(new FileOperations(ftpReturn));

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                           System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {path}",
                                    $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }  
    
        public override string Key => Code;
        public override void DeQueueProcess()
        {
            throw new NotImplementedException();
        }
    }
}
