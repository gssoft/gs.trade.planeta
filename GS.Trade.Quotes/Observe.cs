using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Process;

namespace GS.Trade.Quotes
{
    public partial class Quotes
    {
        public delegate void QuoteToObserveCallback();

        public ObservableCollection<Quote> QuoteObserveCollection;

        public QuoteToObserveCallback CallbackGetQuoteToObserve;
        private SimpleProcess _observeQuoteProcess;

        public void ExecuteObserveProcess()
        {
            if (CallbackGetQuoteToObserve != null) CallbackGetQuoteToObserve();
        }
        public void GetQuotesToObserve()
        {
            lock (_lockPutQuote)
            {
                QuoteObserveCollection.Clear();
                foreach( var q in QuoteDictionary.Values)
                {
                    QuoteObserveCollection.Add(q);
                }
            }
        }
        public void StartObserve()
        {
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Quotes", "Quotes", "Start Observe Process", "", "");
            _observeQuoteProcess.Start();
        }
        public void StopObserve()
        {
            _eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Quotes", "Quotes", "Stop Observe Process", "", "");
            _observeQuoteProcess.Stop();
        }
    }
}