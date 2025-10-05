using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Strategies.Signals
{
    public class Signal
    {
        private IDictionary<string, Func<string,float>> _signals 
            = new Dictionary<string, Func<string,float>> (); 
        
        private Action<string> a = (s) => { };
        private Func<string, float> b = s => 0.1f;

        public float ShortEntry(string name)
        {
            var ef = _signals[name];
            return ef("a");
        }
    }
}
