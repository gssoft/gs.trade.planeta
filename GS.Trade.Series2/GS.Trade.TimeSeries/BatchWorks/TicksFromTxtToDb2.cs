using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.BatchWorks;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.TimeSeries.FortsTicks2.Dal;
using GS.Trade.TimeSeries.FortsTicks2.Model;
using GS.Trade.TimeSeries.General;

namespace GS.Trade.TimeSeries.BatchWorks
{
    public class TicksFromTxtToDb2 : Element1<string>, IBatchWorkItem
    {
        public string SourcePath { get; set; } // = @"F:\Forts\2016\Txt\";
        public string SourceFileFilter { get; set; } // = @"FT*.CSV";

        public string ConnectionString { get; set; }

        public override string Key
        {
            get { return FullCode; }
        }

        public void DoWork()
        {
            try
            {
                Database.SetInitializer(new FortsTicks2.Init.Initializer());

                if (ConnectionString.HasNoValue())
                {
                    SendException(FullCode, "DoWork()",
                        new ArgumentException("ConnectionString is Empty"));
                    return;
                }
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "AddTick()", "Start", "", "");
                AddTicks();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode, "AddTick()", "Complete", "", "");

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "UpdateTicksSeriesStat()", "Start", "", "");
                UpdateTickSeriesStat();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode, "UpdateTicksSeriesStat()", "Complete", "", "");

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "UpdateTicksStatDaily()", "Start", "", "");
                UpdateTicksStatDaily();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode,  "UpdateTicksStatDaily()", "Complete", "", "");

                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "UpdateTicksStatAll()", "Start", "", "");
                UpdateTicksStatAll();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullCode, "UpdateTicksStatAll()", "Complete", "", "");
            }
            catch (Exception ex)
            {
                SendException(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                // throw new Exception(ex.Message);
            }
        }
        private void AddTicks()
        {
            try
            {
                var dir = new DirectoryInfo(SourcePath);
                var files = dir.GetFiles(SourceFileFilter);

                using (var context = new FortsTicksContext2(ConnectionString))
                {
                    var tickers = context.Tickers.ToList();
                    //Assert.AreEqual(3, tickers.Count());
                    if (!tickers.Any())
                    {
                        context.InitTickers();
                        //SendException(FullCode, "AddTick()",
                        //    new ArgumentException("Tickers List is Empty"));
                        //return;
                    }
                    var filesInDb = context.GetFiles();
                    var filesToProcess = files.Select(f => f.FullName).Except(filesInDb).ToList();

                    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode,"AddTick()",
                                            "FileCnt to Processed: " + filesToProcess.Count(),"","");

                    foreach (var f in filesToProcess)
                    {
                        var path = Path.Combine(SourcePath, f);
                        context.AddTicks2(path);
                        Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, FullCode, "AddTick()",
                                            "File has been Processed: " + f, "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                SendException(FullCode, "AddTick2()", ex);
            }
        }

        private void UpdateTicksStatAll()
        {
            try
            {
                using (var db = new FortsTicksContext2(ConnectionString))
                {
                    db.UpdateTicksStatAll();
                }
            }
            catch (Exception ex)
            {
                SendException(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            
        }

        private void UpdateTickSeriesStat()
        {
            try
            {
                using (var db = new FortsTicksContext2(ConnectionString))
                {
                    db.UpdateTickSeriesStat();
                }
            }
            catch (Exception ex)
            {
                SendException(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        private void UpdateTicksStatDaily()
        {
            try
            {
                using (var db = new FortsTicksContext2(ConnectionString))
                {
                    db.UpdateTicksStatDaily();
                }
            }
            catch (Exception ex)
            {
                SendException(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}
