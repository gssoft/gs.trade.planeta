using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase1.Dal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GS.Asp.Net.Users.Admin.Tests.StoredProcedures
{
    [TestClass]
    public class StoredProceduresTest
    {
        [TestMethod]
        public void ClearItemsExceptTwoDaysFromAllItems()
        {
            using (var evl = new EvlContext1())
            {
                try
                {
                    evl.ClearItemsExceptTwoDaysFromAllAsync();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

    }
}
