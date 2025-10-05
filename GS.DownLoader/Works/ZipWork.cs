using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class ZipWork : IWorkItem
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        
        public string SourcePath { get; set; }// = @"/pub/info/stats/history/F/2012/";
        public string SourceFileFilter { get; set; }

        public string TargetPath { get; set; }
        public string TargetFileFilter { get; set; }
        public string TargetFileFilter2 { get; set; }

        public string AddExtension { get; set; }
        [XmlIgnore]
        public WorkContainer Works { get; set; }

        public void DoWork()
        {
            FileInfo[] files;
            try
            {
                var dir = new DirectoryInfo(SourcePath);
                files = dir.GetFiles(SourceFileFilter);
                
                Works.EvlMessage(EvlResult.SUCCESS, "Get Source FileList From: " + SourcePath + " FilesFilter: " + SourceFileFilter, "Files Count:" + files.Count());
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, e.Message, "");
                return;
            }
            var sourceFileList = files.Select(f => f.Name.ToUpper()).ToList();

            try
            {
                var dir = new DirectoryInfo(TargetPath);
                files = dir.GetFiles(TargetFileFilter);

                Works.EvlMessage(EvlResult.SUCCESS, "Get Target FileList From: " + TargetPath + " FilesFilter: " + TargetFileFilter, "Files Count:" + files.Count());
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, e.Message, "");
            //    Works.EvlMessage(EvlResult.SUCCESS, "================", "=================");
                return;
            }
            //var targetFileList = files.Select(f => f.Name.Split('.')[0]).ToList();
            var targetFileList = files.Select(f => f.Name.ToUpper()).ToList();
            //var targetFileList = (from f in files where f.Name.Contains(TargetFileFilter2) select f.Name).ToList();

            // var filesToProcess = sourceFileList.Except<string>(targetFileList);
            var filesToProcess = sourceFileList.Except(targetFileList, new FileNameComparer());
            var fileUnZipedCount = 0;
            if (filesToProcess.Count() <= 0)
            {
             //   Works.EvlMessage(EvlResult.WARNING, "Nothing Files To UnZip From", SourcePath);
            }
            else
            {
                Works.EvlMessage(EvlResult.INFO, "Starts Zip Process.", "Files To UnZip Count: " + filesToProcess.Count());
                
                foreach (var f in filesToProcess)
                {
                    var zip = ZipStorer.Open(SourcePath + f, FileAccess.Read);
                    List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                    foreach (ZipStorer.ZipFileEntry entry in dir)
                    {
                        var fileName = Path.GetFileName(entry.FilenameInZip);

                        if (fileName == null || !fileName.ToUpper().Contains(TargetFileFilter2.ToUpper())) continue;
                        var fn = (f.Split('.')[0] + AddExtension + '.' + fileName.Split('.')[1]).ToUpper();
                        string path = Path.Combine(TargetPath, fn);
                        bool result = zip.ExtractFile(entry, path);

                        if (result)
                        {
                            fileUnZipedCount++;
                            Works.EvlMessage(EvlResult.SUCCESS, "UnZiped: " + f, "-> " + TargetPath + fn);
                        }
                        else
                            Works.EvlMessage(EvlResult.FATAL, "Not UnZiped: ", TargetPath + fn);

                    }
                    zip.Close();
                }
            }
            Works.EvlMessage(EvlResult.SUCCESS, "Finish UnZiping.", "Files To UnZip: " + filesToProcess.Count() + ". Files Processed: " + fileUnZipedCount);
        }
    }
    
    class FileNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Split('.')[0] == y.Split('.')[0]; ;
        }
        public int GetHashCode(string x)
        {
            return x.Split('.')[0].GetHashCode();

            //var s = x.Split('.')[0].TrimStart('F', 'T'); //.GetHashCode();
            //return s.GetHashCode();
            // return x.Split('.')[0].TrimStart('F', 'T').GetHashCode();
            //return Path.GetFileNameWithoutExtension(x).GetHashCode();
        }
    }
}
