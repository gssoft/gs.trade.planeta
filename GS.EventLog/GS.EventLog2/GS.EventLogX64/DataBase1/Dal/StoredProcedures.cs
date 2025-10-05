using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.EventLog.DataBase1.Dal
{
    public partial class EvlContext1
    {
        public void ClearItemsExceptTwoDaysFromAll()
        {
            Database.ExecuteSqlCommand("exec dbo.SP_Clear_DB_Last2Days");
        }
        /// <summary>
        /// usage with try catch
        /// 
        /// </summary>
        /// <returns></returns>
        public Task ClearItemsExceptTwoDaysFromAllAsync()
        {
            return Task.Factory.StartNew(ClearItemsExceptTwoDaysFromAll);       
        }
    }
}
