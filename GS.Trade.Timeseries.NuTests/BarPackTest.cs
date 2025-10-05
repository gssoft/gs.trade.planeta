using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.Extensions;
using NUnit.Framework;

namespace GS.Trade.Timeseries.NuTests
{
    [TestFixture]
    public class BarPackTest
    {
        public BarList BarList { get; set; }

        [SetUp]
        public void Init()
        {
            BarList = new BarList();
        }

        [Test]
        public void Test_PackBar()
        {
            //Assert.DoesNotThrow(() => { 
            var barsOrig = BarList.Bars.ToList();
            var bs = barsOrig.Select(b => b.ToStrPacked());
            foreach(var s in bs)
                Console.WriteLine(s);
            var bars = bs.Select(s => BarSimpleExt.ToBarSimple(s)).ToList();

            foreach (var b in bars)
                Console.WriteLine(b.ToString());

            Assert.That(barsOrig.Count, Is.EqualTo(bars.Count()));
            var cnt = bars.Count();
            for(var i = 0; i < cnt; i++)
            {
                //Console.WriteLine("Orig: {0}", barsOrig[i]);
                //Console.WriteLine("Pack: {0}", bars[i]);

                Assert.That(barsOrig[i].Open, Is.EqualTo(bars[i].Open));
                Assert.That(barsOrig[i].High, Is.EqualTo(bars[i].High));
                Assert.That(barsOrig[i].Low, Is.EqualTo(bars[i].Low));
                Assert.That(barsOrig[i].Close, Is.EqualTo(bars[i].Close));
                Assert.That(barsOrig[i].Volume, Is.EqualTo(bars[i].Volume));
                Assert.That(barsOrig[i].DT, Is.EqualTo(bars[i].DT).Within(1).Seconds);
                Assert.That(barsOrig[i].DT, Is.EqualTo(bars[i].DT));

                Assert.IsTrue(barsOrig[i].CompareTo(bars[i]));
            }
            //});
        }
    }
}
