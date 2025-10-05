using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.ConsoleAS;
using GS.Elements;
// using GS.Trade.QuoteDownLoader.Works;
using GS.Interfaces;
using GS.Serialization;

namespace Ca_GS.Trade.Works
{
    class Program : Element1<string>
    {
        static void Main(string[] args)
        {
            //var dwl = new WorkContainer();
            //dwl.DoWorks(@"Init\Works3.xml");

            var evl = Builder.Build2<IEventLog>(@"Init\EventLog.xml", "EventLogs");
            evl.Init();

            var ws = new BatchWork();
            ws.Init(evl);
            // ws.Init(@"Init\Works3.xml");
            ws.Init(@"Init\Forts_2016.xml");
            //ws.Init(@"Init\Forts_2015.xml");
            //ws.Init(@"Init\Forts_2014.xml");

            ws.DoWorks();
            // ws.Parent = this;
            ConsoleSync.WriteReadLineT("Works is Completed");
            //var w = new FtpWork
            //{
            //    Name = "FTP DownLoading FTYYMMDD.ZIP Files",

            //    LocalPath = @"F:\Forts\2016\Zip\",
            //    LocalFileFilter = @"FT*.Zip",

            //    RemoteUri = @"ftp.moex.ru",
            //    Login = "anonymous",
            //    Password = string.Empty,

            //    RemotePath = @"/pub/info/stats/history/F/2016/",
            //    RemoteFileFilter = @"FT*",
            //    Works = dwl
            //};
            //dwl.AddWork(w);

            //w = new FtpWork
            //{
            //    Name = "FTP DownLoading TEYYMMDD.ZIP Files",

            //    LocalPath = @"F:\Forts\2016\Zip\",
            //    LocalFileFilter = @"TE*.Zip",

            //    RemoteUri = @"ftp.rts.ru",
            //    Login = "anonymous",
            //    Password = string.Empty,

            //    RemotePath = @"/pub/info/stats/history/F/2012/",
            //    RemoteFileFilter = @"TE",
            //    Works = dwl
            //};
            //dwl.AddWork(w);

            //z = new ZipWork
            //{
            //    WorkName = "UnZIP FTYYMMDD.ZIP Files to FTYYMMDD.f.csv ",

            //    SourcePath = @"T:\Forts\2012\Zip\",
            //    SourceFileFilter = @"FT*.Zip",

            //    TargetPath = @"T:\Forts\2012\Txt\",
            //    TargetFileFilter = @"FT*.f.csv",
            //    TargetFileFilter2 = @"ft.csv",

            //    AddExtension = @".f",

            //    Works = dwl
            //};
            //dwl.AddWork(z);

            //z = new ZipWork
            //{
            //    WorkName = "UnZip TEYYMMDD.ZIP Files to TEYYMMDD.f.csv ",

            //    SourcePath = @"T:\Forts\2012\Zip\",
            //    SourceFileFilter = @"TE*.Zip",

            //    TargetPath = @"T:\Forts\2012\Txt\",
            //    TargetFileFilter = @"TE*.f.csv",
            //    TargetFileFilter2 = @"fe.csv",

            //    AddExtension = @".f",

            //    Works = dwl
            //};
            //dwl.AddWork(z);
        }

        public override string Key
        {
            get { return "Main"; }
        }
    }
}
