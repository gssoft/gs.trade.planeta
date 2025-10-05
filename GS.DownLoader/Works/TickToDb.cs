using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class TickToDb : IWorkItem
    {
        
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string SourceFileFilter { get; set; }

        public string TargetConnectionString { get; set; }
        
        public bool IsEnabled { get; set; }

        private DataTable _dataTable;

       // private long _unicID;
        [XmlIgnore]
        public WorkContainer Works { get; set; }

        public void DoWork()
        {
            _dataTable = MakeTable();

            var lastDT = GetMaxDateTimeFromDb();
            if (lastDT == DateTime.MinValue)
            {
                Works.EvlMessage(EvlResult.FATAL,"Can't to Get Max Quote DateTime From DB", TargetConnectionString);
                return;
            }
            var strLastDT = lastDT.ToString("yyMMdd");

            FileInfo[] files;
            try
            {
                var dir = new DirectoryInfo(SourcePath);
                files = dir.GetFiles(SourceFileFilter);
               // Works.EvlMessage(EvlResult.SUCCESS, "Get Source FileList From: " + SourcePath + " FilesFilter: " + SourceFileFilter, "Files Count:" + files.Count());
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, e.Message, "");
                return;
            }
            //var sourceFileList = files.Select(f => f.Name.ToUpper()).OrderBy(n=>n).ToList();
            // var ss = (files[0].Name).Split(new char[] {'.'})[0].TrimStart(new char[] {'F', 'T'});
            IEnumerable<string> sourceFileList;
            try
            {
                 sourceFileList = (
                    from f in files
                    // where (f.Name).Split(new[] { '.' })[0].TrimStart(new[] { 'F', 'T' }).CompareTo(strLastDT) > 0
                    where String.Compare((f.Name).Split(new [] { '.' })[0].TrimStart(new [] { 'F', 'T' }),
                                                                        strLastDT, StringComparison.Ordinal) > 0
                    select f.Name.ToUpper()).OrderBy(f => f).ToList();

                 Works.EvlMessage(EvlResult.SUCCESS, "Get Source FileList From: " + SourcePath + " FilesFilter: " + strLastDT + " " + SourceFileFilter,
                 "Files Count:" + sourceFileList.Count());
            }
            catch (Exception e)
            {
               Works.EvlMessage(EvlResult.FATAL, "Invalid FileName Format", e.Message);
               return;
            }
            
            foreach (var f in sourceFileList)
            {
                var dt = DateTime.Now;

                //var ts = GetTicks(SourcePath+f);
                GetTick(SourcePath + f);
                var tsp = DateTime.Now - dt;
                Works.EvlMessage(EvlResult.SUCCESS, f, String.Format("Ticks File: {0}, Elapsed Time: {1:T}", _dataTable.Rows.Count, tsp));

                dt = DateTime.Now; 
                BulkSqlFile();
                tsp = DateTime.Now - dt;
                Works.EvlMessage(EvlResult.SUCCESS, f, String.Format("Tick in DataBase: {0}, Elapsed Time: {1:T}", _dataTable.Rows.Count, tsp));
            }
        }
        /*
        private IEnumerable<Tick> GetTicks(string fileName)
        {
            var ts = new List<Tick>();
            var dlmt = new char[] {';'};
            using (var sr = new StreamReader(fileName))
            {
                try
                {
                    var line = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        var s = line.Split(dlmt);
                        if (int.Parse(s[6].Trim()) != 0) continue;
                        var t = new Tick
                                    {
                                        Code = s[0].Trim().ToUpper(),
                                        LongCode = s[1].Trim().ToUpper(),
                                        Price = float.Parse(s[2].Trim()),
                                        Amount = int.Parse(s[3].Trim()),
                                        DateTime = DateTime.Parse(s[4].Trim()),
                                        TradeNo = long.Parse(s[5].Trim())
                                    };
                        ts.Add(t);
                    }
                }
                catch (Exception e)
                {
                    Works.EvlMessage(EvlResult.FATAL, fileName, e.Message);
                }
                finally
                {
                    sr.Close();
                }
            }
            return ts;
        }
        */ 
        private void GetTick(string fileName)
        {
            _dataTable.Clear();

            var dlmt = new char[] {';'};
            using (var sr = new StreamReader(fileName))
            {
                try
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var s = line.Split(dlmt);
                        if (int.Parse(s[6].Trim()) != 0) continue;

                        var r = _dataTable.NewRow();
                     //   r["TickID"] = ++_unicID;
                        r["Code"] = s[0].Trim().ToUpper();
                        r["LongCode"] = s[1].Trim().ToUpper();
                        r["Price"] = float.Parse(s[2].Trim());
                        r["Amount"] = int.Parse(s[3].Trim());
                        r["DT"] = DateTime.Parse(s[4].Trim());
                        r["TradeNo"] = long.Parse(s[5].Trim());

                        _dataTable.Rows.Add(r);
                    }
                }
                catch (Exception e)
                {
                    Works.EvlMessage(EvlResult.FATAL, fileName, e.Message);
                }
                finally
                {
                    sr.Close();
                }
            }
        }
        private static DataTable MakeTable()
        {
            var dt = new DataTable();
           // dt.Columns.Add("TickID", typeof(long));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("LongCode", typeof(string));
            dt.Columns.Add("Price", typeof(float));
            dt.Columns.Add("Amount", typeof(int));
            dt.Columns.Add("DT", typeof(DateTime));
            dt.Columns.Add("TradeNo", typeof(long));

            return dt;
        }
        private void BulkSqlFile()
        {
            //var connectionString = @"Data Source=P5Ad2\;Initial Catalog=T;Integrated Security=True";

                if (_dataTable.Rows.Count <= 0) return;
                using (var bulkCopy = new SqlBulkCopy(TargetConnectionString))
                {
                    bulkCopy.BulkCopyTimeout = 300;
                    bulkCopy.DestinationTableName = "dbo.Ticks";
                    try
                    {
                        bulkCopy.WriteToServer(_dataTable);
                    }
                    catch (Exception ex)
                    {
                        Works.EvlMessage(EvlResult.FATAL, "Bulk_Sql Copy Error", ex.Message);
                    }
                 }
            }
        private DateTime GetMaxDateTimeFromDb()
        {
            const string sql = "Select Max(DT) From Ticks";
            var dt = DateTime.MinValue; 
            using (var conn = new SqlConnection(TargetConnectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        dt = (DateTime) cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Works.EvlMessage(EvlResult.FATAL, "GetMaxDateFromDB", ex.Message);
                    }
                }
            }
            return dt;
        }
    }
}
