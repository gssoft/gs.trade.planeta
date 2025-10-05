using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Web.Mvc.Evl01.ViewModels
{
    public class AppEventLog
    {
        public int AppID { get; set; }
        public string AppName { get; set; }
        public int EventLogID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDT { get; set; }
    }
}