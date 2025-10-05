using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.DataBase.Configuration.Dal;
using GS.DataBase.Configuration.Init;

using GS.DataBase.Configuration.Model;
// using GS.Reflection;
using GS.Serialization;

namespace AppDbConfigCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var strCommand = "Save";
            var strCommand = "Restore";
            switch (strCommand)
            {
                case "Save":
                    var cnfLst = CreateBackUp();
                    if (cnfLst == null || cnfLst.Count <= 0)
                        throw new NullReferenceException("Create BackUp Model Failure");

                    Do.Serialize<List<AppDbConfigCreator.Model.Configuration>>(@"D:\Temp\Configurations.xml", cnfLst);

                    var cnfListCheck =
                        Builder.Build<List<AppDbConfigCreator.Model.Configuration>>(@"D:\Temp\Configurations.xml",
                            "ArrayOfConfiguration");

                    CheckSerialization(cnfLst, cnfListCheck);
                    break;
                case "Restore":
                    RestoreBackUp();
                    break;
            }

            //  Database.SetInitializer(new InitDb());

          //  var db = new ConfigurationContext();


            //var cnf = new Configuration
            //{
            //    Code = "GS.Trade.Open",
            //    Catalog = @"VC/1303/Tests/Wpf_Test_TradeContext/"
            //};
            //db.Configurations.Add(cnf);
            //var saveChanges = db.SaveChanges();
            ////
            /*
            Configuration cnf; Item item; Transaction trans;
            cnf = new Configuration
            {
                Code = "GS.Trade.Open",
                Name = "GS.Trade.Open",
                Catalog = @"VC/1303/Tests/Wpf_Test_TradeContext/"
            };
            item = new Item { Code = "Strategies" };
            trans = new Transaction { Source = "IP", Operation = "Get", Object = "File", Result = OperResultEnum.Failure };
            item.Transactions.Add(trans);
            cnf.Add(item);
            db.Add(cnf);
            cnf = new Configuration
            {
                Code = "GS.Trade.Finam",
                Name = "GS.Trade.Finam",
                Catalog = @"VC/1303/Tests/Wpf_Test_TradeContext/"
            };
            item = new Item { Code = "Strategies" };
            trans = new Transaction { Source = "IP", Operation = "Get", Object = "File", Result = OperResultEnum.Failure };
            item.Transactions.Add(trans);
            cnf.Add(item);
            db.Add(cnf);
            cnf = new Configuration
            {
                Code = "Renessanse",
                Name = "Renessanse",
                Catalog = @"VC/1303/Tests/Wpf_Test_TradeContext/"
            };
            item = new Item {Code = "Strategies"};
            trans = new Transaction {Source = "IP", Operation = "Get", Object = "File", Result = OperResultEnum.Failure};
            item.Transactions.Add(trans);
            cnf.Add(item);
            db.Add(cnf);

            cnf = new Configuration
            {
                Code = "BC.Stanadard",
                Name = "BC.Stanadard",
                Catalog = @"VC/1303/Tests/Wpf_Test_TradeContext/"
            };
            item = new Item { Code = "Strategies" };
            trans = new Transaction { Source = "IP", Operation = "Get", Object = "File", Result = OperResultEnum.Failure };
            item.Transactions.Add(trans);
            cnf.Add(item);
            db.Add(cnf);
            
            */

        }

        private static bool CheckSerialization(List<AppDbConfigCreator.Model.Configuration> orig,
            List<AppDbConfigCreator.Model.Configuration> target)
        {
            var arOrig = orig.ToArray();
            var arTarget = target.ToArray();

            if (arOrig.Count() != arTarget.Count())
                throw new NullReferenceException(String.Format("Configurations Count is NOT the Same: {0} {1}",
                                arOrig.Count(), arTarget.Count()));

            for (int i = 0; i < arOrig.Count(); i++)
            {
                if (arOrig[i].Code == arTarget[i].Code)
                {
                    var cnfCode = arOrig[i].Code;
                    var a1 = arOrig[i].Items.ToArray();
                    var a2 = arTarget[i].Items.ToArray();
                    if(a1.Count() != a2.Count())
                        throw new NullReferenceException(String.Format("Cnf: {0} Items Count is NOT the Same: {1} {2}",
                                arOrig[i].Code, a1.Count(), a2.Count()));

                    for (int j = 0; j < a1.Count(); j++)
                    {
                        var itemCode = a1[j].Code;  
                        if(a1[j].Code != a2[j].Code)
                            throw new NullReferenceException(String.Format("Item Code: {0} does Not Equal to {1}"
                                , a1[j].Code, a2[j].Code));
                    }
                }
                else
                    throw new NullReferenceException(String.Format("Configuration Code: {0} does Not Equal to {1}"
                                    , arOrig[i].Code, arTarget[i].Code));
            }
            ConsoleSync.WriteReadLine("Check is Passed\r\nPress any Key ... ");

            return true;
        }

        private static List<AppDbConfigCreator.Model.Configuration> CreateBackUp()
        {

            var cnfLst = new List<AppDbConfigCreator.Model.Configuration>();

            try
            {
                var db = new ConfigurationContext();
                var cnfs = db.Configurations.Include(c => c.Items).ToList();
                foreach (var cnf in cnfs)
                {
                    var c = new AppDbConfigCreator.Model.Configuration
                    {
                        Id = cnf.Id,
                        Enabled = cnf.Enabled,
                        Catalog = cnf.Catalog,
                        Code = cnf.Code,
                        CreatedDT = cnf.CreatedDT,
                        ModifiedDT = cnf.ModifiedDT
                    };
                    foreach (var i in cnf.Items)
                    {
                        var myi = new AppDbConfigCreator.Model.Item
                        {
                            Id = i.Id,
                            ConfigurationId = i.ConfigurationId,
                            Enabled = i.Enabled,
                            Catalog = i.Catalog,
                            Code = i.Code,
                            Obj = i.Obj,
                            CreatedDT = i.CreatedDT,
                            ModifiedDT = i.ModifiedDT
                        };
                        c.Items.Add(myi);
                    }
                    cnfLst.Add(c);
                }
                return cnfLst;
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }
        }

        private static void RestoreBackUp()
        {
            Database.SetInitializer(new InitDb());

            var db = new ConfigurationContext();

            foreach (var c in db.Configurations.ToList())
            {
                db.Configurations.Remove(c);
            }

            var backupLst =
                        Builder.Build<List<AppDbConfigCreator.Model.Configuration>>(@"D:\Temp\Configurations.xml",
                            "ArrayOfConfiguration");

            foreach (var cbk in backupLst)
            {
                var cnf = new GS.DataBase.Configuration.Model.Configuration
                {
                    Catalog = cbk.Catalog,
                    Code = cbk.Code,
                    CreatedDT = cbk.CreatedDT,
                    ModifiedDT = cbk.ModifiedDT
                };
                foreach (var i in cbk.Items)
                {
                    var item = new GS.DataBase.Configuration.Model.Item
                    {
                        Catalog = i.Catalog,
                        Code = i.Code,
                        CreatedDT = i.CreatedDT,
                        ModifiedDT = i.ModifiedDT,
                        Obj = i.Obj,
                        TrCount = i.TrCount,
                    };
                    cnf.Add(item);
                }
                db.Configurations.Add(cnf);
            }

            db.SaveChanges();
        }
    }
}
