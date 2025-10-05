using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Ftp;
using GS.Interfaces;

namespace GS.Ftp
{
    public class FtpWebClient : Element1<string>
    {
        protected FtpWebRequest FtpWebRequest { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string BaseAddress { get; set; }

        public bool IsUnixFtpServer { get; set; }

        public string FtpServerType => IsUnixFtpServer ? "Unix" : "Windows"; 

        // public FtpReturn FtpReturn { get; set; }

        public override string ToString()
        {
            return $"Type: {TypeFullName}, BaseAddress: {BaseAddress}, FtpServerType: {FtpServerType}"; //, User: {User}";
        }

        public override void Init()
        {
            try
            {
               // Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeFullName, TypeFullName, "Init()", "Start", ToString());

                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT,  "Init()", "Complete", ToString());
                // return true;
            }
            catch (Exception ex)
            {
                SendException(ex);
                // return false;
            }
        }
        public Stream GetListDirectoryDetailsStream(string dir)
        {
            try
            {
                FtpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                return FtpWebRequest.GetResponse().GetResponseStream();
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
                return null;
            }
        }
        public FtpReturn GetListDirectoryDetails(string dir)
        {
            var ftpReturn = new FtpReturn
            {
                FileShortPath = dir,
                FileFullName = dir,
                FtpOperation = FtpOperation.GetDirectoryList
            };

            var baseAdr = BaseAddress;
            if (IsUnixFtpServer)
                baseAdr = baseAdr.Replace("/", "\\");

            var sourcePath = string.Concat(baseAdr, dir);

            if (IsUnixFtpServer)
                sourcePath = sourcePath.Replace("\\", "/");

//#if DEBUG
//            Evlm3(EvlResult.INFO, EvlSubject.TECHNOLOGY, 
//                System.Reflection.MethodBase.GetCurrentMethod().Name, "Start", sourcePath);
//#endif
            //var ftpReturn = new FtpReturn
            //{
            //    FileShortPath = dir,
            //    FileFullName = dir,
            //    FtpOperation = FtpOperation.GetDirectoryList
            //};
            try
            {
                var request = (FtpWebRequest) WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse)request.GetResponse())
                using (var respStream = response.GetResponseStream())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;

                    var listDirectoryDeails = UnixFtpStreamParse(respStream);

                    respStream?.Close();
                    response.Close();

                    if (listDirectoryDeails != null)
                    {
                        ftpReturn.ListDirectoryDetails = listDirectoryDeails.ToList();
                        ftpReturn.FtpOperationStatus = FtpOperationStatus.Ok;
#if DEBUG
                        Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                            ftpReturn.FtpOperStr, $"{dir}", ftpReturn.ToString());
#endif
                    }
                    else
                    {
                        ftpReturn.FtpOperationStatus = FtpOperationStatus.ListDirectoryDetailsFailure;

                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                            ftpReturn.FtpOperStr, $"{dir}", ftpReturn.ToString());
                    }
                }
                return ftpReturn;
            }
            catch (WebException ex)
            {
                 SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus = FtpOperationStatus.ListDirectoryDetailsFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ftpReturn.FileShortPath,
                        ftpReturn.FtpOperStr,
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                 SendException(ex);

                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus = FtpOperationStatus.ListDirectoryDetailsFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.FileShortPath,
                          ftpReturn.FtpOperStr,
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        public byte[] DownLoadFileInBytes(string path)
        {
            // path = "configuration.php";
            try
            {
                // ConsoleSync.WriteLineT($"{path}");

                //if (IsUnixFtpServer)
                //    path = path.Replace("\\", ""); // .Replace("/","");

                string baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                // var sourcePath = Path.Combine(baseAdr, path);
                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                //ConsoleSync.WriteLineT($"Path: {sourcePath}");
                ConsoleSync.WriteLineT($"{sourcePath}");
                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    return responseStream.CopyToBytes();
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                ex.ToConsoleSync();
            }
            return new byte[0];
        }
        public Stream DownLoadFileInStream(string path)
        {
            try
            {
                string baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                // var sourcePath = Path.Combine(baseAdr, path);
                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                //ConsoleSync.WriteLineT($"Path: {sourcePath}");
                ConsoleSync.WriteLineT($"{sourcePath}");
                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    var ms = new MemoryStream();
                    responseStream?.CopyTo(ms);
                    return ms;
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
                //ex.ToConsoleSync();
            }
            return null;
        }
        public FtpReturn DownLoadFile(string path)
        {
            var ftpReturn = new FtpReturn {FileShortPath = path};

            var dirname = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);

            try
            {
                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                // var sourcePath = Path.Combine(baseAdr, path);
                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                // ftpReturn.FileShortPath = path;
                ftpReturn.FileFullName = sourcePath;
                ftpReturn.FtpOperation = FtpOperation.DownloadFile;

                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse)request.GetResponse())
                using(var stream = response.GetResponseStream())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;

                    if (stream != null)
                    {
                        string content;
                        using (var reader = new StreamReader(stream))
                        {
                            content = reader.ReadToEnd();
                            reader.Close();
                        }

                        ftpReturn.Content = content;
                        ftpReturn.FtpOperationStatus = FtpOperationStatus.Ok;
                    }
                    else
                        ftpReturn.FtpOperationStatus = FtpOperationStatus.NullContent;

                    stream?.Close();
                    response.Close();

                    if (ftpReturn.IsSuccess)
                    {
// #if DEBUG
                        Evlm2(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                            // System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                            ftpReturn.FtpOperStr,
                            $"{ftpReturn.DirFileNameStr}",
                            ftpReturn.ToString());
// #endif
                    }
                    else
                        Evlm2(EvlResult.FATAL,EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                            ftpReturn.FtpOperStr,
                            $"{ftpReturn.DirFileNameStr}",
                            ftpReturn.ToString());

                    return ftpReturn;
                }
            }
            catch (WebException ex)
            {
                SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse) ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse) ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus =
                    ftpReturn.FtpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || ftpReturn.FtpStatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy
                        ? FtpOperationStatus.FileNotFound
                        : FtpOperationStatus.Exception;

                ftpReturn.Exception = ex;
                ex.Response.Close();

                if(ftpReturn.FtpOperationStatus == FtpOperationStatus.FileNotFound)
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                            ftpReturn.FtpOperStr,
                            $"{ftpReturn.DirFileNameStr}",
                            ftpReturn.ToString());
                else
                    Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                                    ftpReturn.FtpOperStr,
                                    $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.Exception;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                           ftpReturn.FtpOperStr,
                           $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        public FtpReturn MakeDirectory(string path)
        {
            var ftpReturn = new FtpReturn {FileShortPath = path};
            try
            {
                // ConsoleSync.WriteLineT($"FILE TO DELETE: {path}");

                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                ftpReturn.FileFullName = sourcePath;
                ftpReturn.FtpOperation = FtpOperation.MakeDirectory;

                var request = (FtpWebRequest)WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse) request.GetResponse())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;
                    ftpReturn.FtpOperationStatus =
                        ftpReturn.FtpStatusCode == FtpStatusCode.PathnameCreated
                            ? FtpOperationStatus.Ok
                            : FtpOperationStatus.MakeDirectoryFailure;

                    response.Close();
                }
                Evlm2(ftpReturn.IsSuccess ? EvlResult.WARNING : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        ftpReturn.FtpOperStr,
                        $"{path}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());
            }
            catch (WebException ex)
            {
                // SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus = FtpOperationStatus.MakeDirectoryFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, path,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                // ex.ToConsoleSync();
//                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.MakeDirectoryFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, path,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        public FtpReturn RemoveDirectoryTree(string path)
        {
            var ftpReturn = new FtpReturn {FileShortPath = path};
            try
            {
                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                ftpReturn.FileFullName = sourcePath;
                ftpReturn.FtpOperation = FtpOperation.RemoveDirectory;

#if DEBUG
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeFullName, TypeFullName,
                    ftpReturn.FtpOperStr, "Start", path);
#endif
                var ftpResp = GetListDirectoryDetails(path);
                if(ftpResp.FtpOperationStatus != FtpOperationStatus.Ok)
                    return ftpResp;

                foreach (var i in ftpResp.ListDirectoryDetails.Where(i => i.IsFile))
                {
                    var name = string.Join(@"\", path, i.Name);
                    //ftpReturn = DeleteFile(name);
                    DeleteFile(name);
                }
                foreach (var i in ftpResp.ListDirectoryDetails.Where(i => i.IsDirectory))
                {
                    var name = string.Join(@"\", path, i.Name);
                    // ftpReturn = RemoveDirectoryTree(name);
                    RemoveDirectoryTree(name);
                }
                return RemoveDirectory(path);
            }
            //catch (WebException ex)
            //{
            //    SendException(ex);

            //    ftpReturn.FtpStatusCode = ((FtpWebResponse) ex.Response).StatusCode;
            //    ftpReturn.FtpStatusDescription = ((FtpWebResponse) ex.Response).StatusDescription;
            //    ftpReturn.FtpOperationStatus = FtpOperationStatus.RemoveDirectoryFailure;
            //    ftpReturn.Exception = ex;
            //}
            catch (Exception ex)
            {
                SendException(ex);

                ftpReturn.FileShortPath = path;
                ftpReturn.FileFullName = path;
                ftpReturn.FtpOperation = FtpOperation.RemoveDirectory;
                ftpReturn.FtpOperationStatus = FtpOperationStatus.RemoveDirectoryFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, path,
                           ftpReturn.FtpOperStr,
                           $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        public FtpReturn RemoveDirectory(string path)
        {
            var ftpReturn = new FtpReturn
            {
                FileShortPath = path,
                FtpOperation = FtpOperation.RemoveDirectory
            };
            try
            {
                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                ftpReturn.FileFullName = sourcePath;

                var request = (FtpWebRequest)WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse) request.GetResponse())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;

                    ftpReturn.FtpOperationStatus =
                        ftpReturn.FtpStatusCode == FtpStatusCode.FileActionOK
                            ? FtpOperationStatus.Ok
                            : FtpOperationStatus.RemoveDirectoryFailure;

                    response.Close();
                }

                Evlm2(ftpReturn.IsSuccess ? EvlResult.WARNING : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                        ftpReturn.FtpOperStr,
                                        $"{path}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());
            }
            catch (WebException ex)
            {
                SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus = FtpOperationStatus.RemoveDirectoryFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, path,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.RemoveDirectoryFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, path,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        public FtpReturn DeleteFile(string path)
        {
            var ftpReturn = new FtpReturn
            {
                FileShortPath = path,
                FtpOperation = FtpOperation.DeleteFile
            };

            var dirname = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            try
            {
                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");
            
                ftpReturn.FileFullName = sourcePath;              
                
                var request = (FtpWebRequest)WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(User, Password);

                using (var response = (FtpWebResponse) request.GetResponse())
                {

                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;
                    ftpReturn.FtpOperationStatus =
                        ftpReturn.FtpStatusCode == FtpStatusCode.FileActionOK
                            ? FtpOperationStatus.Ok
                            : FtpOperationStatus.DeleteFileFailure;

                    response.Close();
                }
                if (ftpReturn.IsSuccess)
                {
                    //Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, 
                    //    System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                    //    $"DELETE FILE: {fileName}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());

                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                        ftpReturn.FtpOperStr,
                        $"{ftpReturn.DirFileNameStr}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());
                }
                else
                {
                    //Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    //    System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                    //    $"DELETE FILE FAILURE: {filename}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());

                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                       ftpReturn.FtpOperStr,
                       $"{ftpReturn.DirFileNameStr}, {ftpReturn.FtpOperationStatus}",
                       ftpReturn.ToString());
                }
                return ftpReturn;
            }
            catch (WebException ex)
            {
                SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ftpReturn.FtpOperationStatus = FtpOperationStatus.DeleteFileFailure;
                ftpReturn.Exception = ex;

                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());

            }
            catch (Exception ex)
            {
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.DeleteFileFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                            ftpReturn.FtpOperStr,
                            $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        // https://stackoverflow.com/questions/3576436/upload-a-file-with-encoding-using-ftp-in-c-sharp
        public FtpReturn UpLoadFile(string sourcePath, string targetPath)
        {
            var ftpReturn = new FtpReturn
            {
                FtpOperation = FtpOperation.UploadFile,
                FileShortPath = sourcePath
            };
            //var dirname = Path.GetDirectoryName(targetPath);
            //var filename = Path.GetFileName(targetPath);
            try
            {
                var baseAdr = BaseAddress;

                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                var targetFullPath = string.Concat(baseAdr, targetPath);

                if (IsUnixFtpServer)
                    targetFullPath = targetFullPath.Replace("\\", "/");

                // ftpReturn.FileShortPath = targetPath;
                // ftpReturn.FileFullName = targetFullPath;

                // ftpReturn.FileShortPath = sourcePath;
                // ftpReturn.FileFullName = targetFullPath;


                var request = (FtpWebRequest)WebRequest.Create(targetFullPath);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(User, Password);

                byte[] bs = File.ReadAllBytes(sourcePath);

                request.ContentLength = bs.Length;
                using (var s = request.GetRequestStream())
                {
                    s.Write(bs, 0, bs.Length);
                }
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;
                    ftpReturn.FtpOperationStatus = response.StatusCode == FtpStatusCode.ClosingData
                        ? FtpOperationStatus.Ok
                        : FtpOperationStatus.UpLoadFailure;
                    response.Close();
                }

                if (ftpReturn.IsSuccess)
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                        ftpReturn.FtpOperStr,
                        $"{ftpReturn.DirFileNameStr}", ftpReturn.ToString());
                else
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.FileName,
                                ftpReturn.FtpOperStr,
                                $"{ftpReturn.DirFileNameStr}", ftpReturn.ToString());

                return ftpReturn;
            }
            catch (WebException ex)
            {
                SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ex.Response?.Close();
                ex.Response?.Dispose();

                ftpReturn.FtpOperationStatus = FtpOperationStatus.UpLoadFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                          ftpReturn.FtpOperStr,
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.UpLoadFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ftpReturn.DirFileNameStr,
                          ftpReturn.FtpOperStr,
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }

        // this is does not work
        public FtpReturn UpLoadFile1(string sourcePath, string targetPath)
        {
            var ftpReturn = new FtpReturn();
            var dirname = Path.GetDirectoryName(targetPath);
            var filename = Path.GetFileName(targetPath);
            try
            {
                var baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");
                
                var targetFullPath = string.Concat(baseAdr, targetPath);

                if (IsUnixFtpServer)
                    targetFullPath = targetFullPath.Replace("\\", "/");

                ftpReturn.FileShortPath = targetPath;
                ftpReturn.FileFullName = targetFullPath;
                ftpReturn.FtpOperation = FtpOperation.UploadFile;

                var request = (FtpWebRequest)WebRequest.Create(targetFullPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(User, Password);

                using (var sourceStream = new StreamReader(sourcePath))
                using (var requestStream = request.GetRequestStream())
                {
                   // ftpReturn.Encoding = sourceStream.CurrentEncoding.ToString();
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();

                    //byte[] fileContents = Encoding.Default.GetBytes(sourceStream.ReadToEnd());
                    //sourceStream.Close();

                    request.ContentLength = fileContents.Length;

                    // using (var requestStream = request.GetRequestStream()) {}

                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                }
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    ftpReturn.FtpStatusCode = response.StatusCode;
                    ftpReturn.FtpStatusDescription = response.StatusDescription;
                    ftpReturn.FtpOperationStatus = FtpOperationStatus.Ok;

                    response.Close();
                }

                if(ftpReturn.IsSuccess)
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                            System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                            $"UPLOAD FILE: {filename}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());
                else
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                System.Reflection.MethodBase.GetCurrentMethod().Name + $", DIR: {dirname}",
                                $"UPLOAD FILE FAILURE: {filename}, {ftpReturn.FtpOperationStatus}", ftpReturn.ToString());

                return ftpReturn;
            }
            catch (WebException ex)
            {
                SendException(ex);

                ftpReturn.FtpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                ftpReturn.FtpStatusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
                ftpReturn.Content = string.Empty;

                ex.Response?.Close();
                ex.Response?.Dispose();

                ftpReturn.FtpOperationStatus = FtpOperationStatus.UpLoadFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, 
                          System.Reflection.MethodBase.GetCurrentMethod().Name + $", File: {targetPath}",
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            catch (Exception ex)
            {
                ex.ToConsoleSync();
                SendException(ex);

                ftpReturn.FtpOperationStatus = FtpOperationStatus.UpLoadFailure;
                ftpReturn.Exception = ex;

                Evlm1(EvlResult.FATAL, EvlSubject.TECHNOLOGY, 
                          System.Reflection.MethodBase.GetCurrentMethod().Name + $", File: {targetPath}",
                          $"Exception: {ex.Message}", ftpReturn.ToString());
            }
            return ftpReturn;
        }
        // Later should remove this
        public string DownLoadFileAndCalcSha1(string path)
        {
            // path = "configuration.php";
            var lst = new List<string>();
            try
            {
                // ConsoleSync.WriteLineT($"{path}");

                //if (IsUnixFtpServer)
                //    path = path.Replace("\\", ""); // .Replace("/","");

                string baseAdr = BaseAddress;
                if (IsUnixFtpServer)
                    baseAdr = baseAdr.Replace("/", "\\");

                // var sourcePath = Path.Combine(baseAdr, path);
                var sourcePath = string.Concat(baseAdr, path);

                if (IsUnixFtpServer)
                    sourcePath = sourcePath.Replace("\\", "/");

                //ConsoleSync.WriteLineT($"Path: {sourcePath}");
                ConsoleSync.WriteLineT($"{sourcePath}");
                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(sourcePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(User, Password);

                var response = (FtpWebResponse) request.GetResponse();
                //var responseStream = response.GetResponseStream();

                var stream = response.GetResponseStream();
                string content;
                using (var reader = new StreamReader(stream))
                {
                    //while (!reader.EndOfStream)
                    //{
                    //    var line = reader.ReadLine();
                    //    lst.Add(line);
                    //}
                    content = reader.ReadToEnd();
                }

                var spl = content.Split('\r', '\n');

                //byte[] dataAsBytes = lst.SelectMany(s =>
                //            Encoding.UTF8.GetBytes(s
                //            // + Environment.NewLine
                //            )).ToArray();

                byte[] dataAsBytes = spl.SelectMany(s =>
                    Encoding.UTF8.GetBytes(s
                        // + Environment.NewLine
                        )).ToArray();

                // byte[] dataAsBytes = (Encoding.UTF8.GetBytes(content)).ToArray();

                return GetHashSha1(dataAsBytes);

                //StreamReader reader = new StreamReader(responseStream);
                //Console.WriteLine(reader.ReadToEnd());

                //return responseStream.CopyToBytes();
            }
            catch (AggregateException ex)
            {
                // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                foreach (var e in ex.ToList())
                    ConsoleSync.WriteLineT(e);
            }
            catch (Exception ex)
            {
                // ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                foreach (var e in ex.ToList())
                    ConsoleSync.WriteLineT(e);
            }
            return null;
        }

        private IEnumerable<FtpFileInfo> UnixFtpStreamParse(Stream responseStream)
        {
            var lst = new List<FtpFileInfo>();
            try
            {
                using (var reader = new StreamReader(responseStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var fi = IsUnixFtpServer
                            ? ParseFtpRequestUnixLine(line)
                            : ParseFtpRequestWindowsLine(line);
                        if (fi != null)
                            lst.Add(fi);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                SendException(ex);

                Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          $"Exception: {ex.Message}","");

                return Enumerable.Empty<FtpFileInfo>();
            }
            return lst;
        }

        // https://stackoverflow.com/questions/1013486/parsing-ftpwebrequest-listdirectorydetails-line#40045894
        private FtpFileInfo ParseFtpRequestUnixLine(string line)
        {
            string pattern =
                @"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
                @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";

            Regex regex = new Regex(pattern);

            IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");

            string[] hourMinFormats =
            {"MMM dd HH:mm", "MMM dd H:mm", "MMM d HH:mm", "MMM d H:mm"};
            string[] yearFormats =
            {"MMM dd yyyy", "MMM d yyyy"};

            try
            {
                Match match = regex.Match(line);
                string permissions = match.Groups[1].Value;
                int inode = int.Parse(match.Groups[2].Value, culture);
                string owner = match.Groups[3].Value;
                string group = match.Groups[4].Value;
                long size = long.Parse(match.Groups[5].Value, culture);

                DateTime modified;
                string s = Regex.Replace(match.Groups[6].Value, @"\s+", " ");
                if (s.IndexOf(':') >= 0)
                {
                    modified = DateTime.ParseExact(s, hourMinFormats, culture, DateTimeStyles.None);
                }
                else
                {
                    modified = DateTime.ParseExact(s, yearFormats, culture, DateTimeStyles.None);
                }
                string name = match.Groups[7].Value;

                return new FtpFileInfo
                {
                    Modified = modified,
                    Name = name,
                    Permissions = permissions,
                    Size = size,
                    Node = inode,
                    Owner = owner,
                    Group = group,
                    DirMemberType = permissions.Substring(0, 1) != "d"
                        ? DirMemberEnum.File
                        : (name.Trim() == "."
                            ? DirMemberEnum.CurrentDir
                            : (name.Trim() == ".." ? DirMemberEnum.ParentDir : DirMemberEnum.Dir))
                };
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          $"Exception: {ex.Message}", line);
                return null;
            }
        }

        // https://stackoverflow.com/questions/7060983/c-sharp-class-to-parse-webrequestmethods-ftp-listdirectorydetails-ftp-response
        //Version2 size = 0  modified = 2011-08-10 12:02
        //image34.gif size = 144700153  modified = 2009-06-25 14:41
        //updates.txt size = 144700153  modified = 2009-06-25 14:51
        //digger.tif size = 144700214  modified = 2010-11-04 14:45
        private FtpFileInfo ParseFtpRequestWindowsLine(string line)
        {
            try
            {
                string pattern = @"^(\d+-\d+-\d+\s+\d+:\d+(?:AM|PM))\s+(<DIR>|\d+)\s+(.+)$";
                Regex regex = new Regex(pattern);
                IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");

                Match match = regex.Match(line);
                DateTime modified =
                   DateTime.ParseExact(
                       match.Groups[1].Value, "MM-dd-yy  hh:mmtt", culture, DateTimeStyles.None);
                long size = (match.Groups[2].Value != "<DIR>") ? long.Parse(match.Groups[2].Value) : 0;
                string name = match.Groups[3].Value;

                var isDir = match.Groups[2].Value == "<DIR>";

                //Console.WriteLine(
                //    "{0,-16} size = {1,9}  modified = {2} {3}",
                //    name, size, modified.ToString("yyyy-MM-dd HH:mm"), isDir ? "DIR" : "FILE");

                return
                    new FtpFileInfo
                    {
                        Modified = modified,
                        Name = name,
                        Permissions = string.Empty,
                        Size = size,
                        Owner = string.Empty,
                        Group = string.Empty,
                        DirMemberType = isDir ? DirMemberEnum.Dir : DirMemberEnum.File
                    };
            }
            catch (Exception ex)
            {
                SendException(ex);
                Evlm1(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          $"Exception: {ex.Message}", line);
                return null;
            }        
        }
        private static string GetHashSha1(byte[] bytes)
        {
            string hash;
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
            return hash;
        }

        public override string Key => Code;
    }
}

