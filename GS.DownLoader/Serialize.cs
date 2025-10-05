using System.Xml.Linq;
using GS.Trade.QuoteDownLoader;

namespace GS.Trade.DownLoader
{
    public partial class FinamDownLoader
    { 
        /*
        public bool SerializeCollection()
        {
            try
            {
                var xDoc = XDocument.Load(@"Init.xml");
                var xe = xDoc.Descendants("Tickers_XmlFileName").First();
                var xmlfname = xe.Value;

                TextWriter tr = new StreamWriter(xmlfname);
                var sr = new XmlSerializer(typeof(List<Pair>));
                sr.Serialize(tr, TickerList);
                tr.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeserializeQuotes()
        {
            try
            {
                var xDoc = XDocument.Load(@"Init.xml");
                var xe = xDoc.Descendants("Tickers_XmlFileName").First();
                var xmlfname = xe.Value;

                var f = new FileStream(xmlfname, FileMode.Open);
                var newSr = new XmlSerializer(typeof(List<Pair>));
                TickerList = (List<Pair>)newSr.Deserialize(f);
                f.Close();

                return true;
            }
            catch { return false; }
        }
        */ 
        public void LoadTickersXml()
        {
            var xe = XElement.Load("FinamTickers.xml");
            foreach (var node in xe.Elements())
            {
                if (node.Attribute("ID") == null || node.Attribute("Code") == null || node.Attribute("Name") == null)
                    continue;

                //Console.WriteLine(node.Attribute("ID").Value + ' ' + node.Attribute("Code").Value + ' ' +
                //                  node.Attribute("Name").Value);

                int id;
                if (int.TryParse(node.Attribute("ID").Value, out id))
                {
                    var t = new Ticker
                                {
                                    ID = id,
                                    Code = node.Attribute("Code").Value.Trim().ToUpper(),
                                    Name = node.Attribute("Name").Value.Trim().ToUpper()
                                };
                    AddTicker(t);
                }
            }
        }
        public void LoadTimeIntsXml()
        {
            var xe = XElement.Load("FinamTimeInts.xml");
            foreach (var node in xe.Elements())
            {
                if (    node.Attribute("ID") == null || node.Attribute("Code") == null ||
                        node.Attribute("Name") == null || node.Attribute("DaysPerPass") == null )
                    continue;

                //Console.WriteLine(node.Attribute("ID").Value + ' ' + node.Attribute("Code").Value + ' ' +
                //                  node.Attribute("Name").Value);

                int id, days;
                if (int.TryParse(node.Attribute("ID").Value, out id) && int.TryParse(node.Attribute("DaysPerPass").Value, out days))
                {
                    var ti = new TimeInt
                                {
                                    ID = id,
                                    Code = node.Attribute("Code").Value.Trim().ToUpper(),
                                    Name = node.Attribute("Name").Value.Trim().ToUpper(),
                                    DaysPerPass = days
                    };
                    AddTimeInt(ti);
                }
            }
           
        }
        private void AddTicker( Ticker ticker)
        {
            Ticker t;
            if ( !TickerDictionary.TryGetValue(ticker.Code, out t))
            {
                TickerDictionary.Add(ticker.Code, ticker);
            }
            else
            {

            }

        }
        private void AddTimeInt(TimeInt ti)
        {
            TimeInt t;
            if (! TimeIntDictionary.TryGetValue(ti.Code, out t))
            {
                TimeIntDictionary.Add(ti.Code, ti);
            }
            else
            {

            }
        }

    }
}