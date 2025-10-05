using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.DownLoader;
//using GS.Trade.QuoteDownLoader.Works;
//using Starksoft.Net.Ftp;

namespace GS.Trade.QuoteDownLoader.Works
{
    public class FtpWork2 : IWorkItem
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string RemoteUri { get; set; } // = @"ftp://ftp.rts.ru";

        public string Login { get; set; }
        public string Password { get; set; }

        public string RemotePath { get; set; }// = @"/pub/info/stats/history/F/2012/";
        public string RemoteFileFilter { get; set; }

        public string LocalPath { get; set; }
        public string LocalFileFilter { get; set; }

        [XmlIgnore]
        public WorkContainer Works { get; set; }

        public void DoWork()
        {
            FileInfo[] files;
            try
            {
                var dir = new DirectoryInfo(LocalPath);
                files = dir.GetFiles(LocalFileFilter);
                Works.EvlMessage(EvlResult.SUCCESS, "Get Target FileList From: " + LocalPath + " FilesFilter: " + LocalFileFilter , "Files Count:" + files.Count());
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, e.Message, "");
                return;
            }
            var localFileList = files.Select(f => f.Name.ToUpper()).ToList();

            //ftp ftpClient = new ftp(RemoteUri, Login, Password);
            //string[] simpleDirectoryListing = ftpClient.directoryListDetailed(RemotePath);

            //var fnames = new List<string>();
            //foreach (var s in simpleDirectoryListing)
            //{
            //    var sa = s.Split(new[] {' '});
            //    fnames.Add(sa[sa.Length-1].TrimUpper());
            //}

            //using (var ftp = new FtpClient(RemoteUri))
            //{
                // var ftp = new FtpClient(RemoteUri);
                // ftp.DataTransferMode = DataTransferMode.Passive;

            var filesProcessed = 0;
                try
                {
                    ftp ftpClient = new ftp(RemoteUri, Login, Password);
                    string[] simpleDirectoryListing = ftpClient.directoryListDetailed(RemotePath);

                    var fnames = new List<string>();
                    foreach (var s in simpleDirectoryListing)
                    {
                        var sa = s.Split(new[] { ' ' });
                        fnames.Add(sa[sa.Length - 1].TrimUpper());
                    }

                    var remoteFileList = (from f in fnames where f.ToUpper().Contains(RemoteFileFilter.ToUpper())
                                          select f.ToUpper()).ToList();

                    Works.EvlMessage(EvlResult.SUCCESS, "Get Source FileList From: " + RemotePath + " FilesFilter: " + RemoteFileFilter, "Files Count:" + remoteFileList.Count);

                    var filesToLoad = remoteFileList.Except<string>(localFileList);

                    if (filesToLoad.Count() <= 0)
                    {
                        // Works.EvlMessage(EvlResult.WARNING, "Nothing Files To DownLoad From", RemoteUri + RemotePath);
                    }
                    else
                    {
                        Works.EvlMessage(EvlResult.INFO, "Starts DownLoading.",
                                   "Files To DownLoad Count: " + filesToLoad.Count());
                        foreach (var f in filesToLoad)
                        {
                            //ftp.GetFile(f, LocalPath + f.ToUpper(), FileAction.Create);
                            filesProcessed++;
                            // ftpClient.download("etc/test.txt", @"C:\Users\metastruct\Desktop\test.txt");
                            ftpClient.download(RemotePath + f, LocalPath + f);
                            Works.EvlMessage(EvlResult.SUCCESS, "File is Loaded:", f);

                        }
                        //Works.EvlMessage(EvlResult.SUCCESS, "Finish DownLoading.",
                        //String.Format("Files To DownLoad: {0}, Files Processed: {1}",filesToLoad.Count(), filesProcessed));
                    }
                    Works.EvlMessage(EvlResult.SUCCESS, "Finish DownLoading.",
                            String.Format("Files To DownLoad: {0}, Files Processed: {1}", filesToLoad.Count(), filesProcessed));
                }
                catch (Exception e)
                {
                    //ftp.Close();
                    Works.EvlMessage(EvlResult.FATAL, e.Message, "");
                  //  Works.EvlMessage(EvlResult.FATAL, "================", "=================");
                    return;
                }
                // ftp.Close();
             //   Works.EvlMessage(EvlResult.SUCCESS, "================", "=================");
            //}
        }
     }
}
