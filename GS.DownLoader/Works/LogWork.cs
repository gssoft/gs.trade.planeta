using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class LogWork : IWorkItem
    {
        public bool IsEnabled { get; set; }

        public string Name { get; set; }

        public string Subject { get; set; }
        public string Description { get; set; }

        [XmlIgnore]
        public WorkContainer Works { get; set; }

        public void DoWork()
        {
            Works.EvlMessage(EvlResult.INFO, Subject, Description);
        }
    }
}
