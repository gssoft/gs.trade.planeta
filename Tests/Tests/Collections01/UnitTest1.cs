using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using GS.Collections;
using GS.ConsoleAS;
using GS.Containers5;
using GS.Counters;
using GS.Elements;
using GS.Events;
using GS.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Collections01
{
    public class TradeKeyItem : Element1<string>
    {
        public string Account { get; set; }
        public long TradeNumber { get; set; }
        public int Value { get; set; }
        public override string Key => string.Join("@", Account,  TradeNumber.ToString());
        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
        public bool IsEquals(TradeKeyItem t)
        {
            if (t == null)
                return false;
            return Key == t.Key && Value == t.Value;
        }

        public TradeKeyItem Clone()
        {
            return new TradeKeyItem
            {
                Account = Account,
                TradeNumber = TradeNumber,
                Value = Value
            };
        }
    }
    [TestClass]
    public class GSCollectionsUnitTest
    {
        public ListCollection2<string, TradeKeyItem> TradeKeyList = new ListCollection2<string, TradeKeyItem>();
        //public ObservableListCollection2<string, TradeKeyItem> TradeKeyList =
        //                            new ObservableListCollection2<string, TradeKeyItem>();
        //public DictionaryCollection2<string, TradeKeyItem> TradeKeyList =
        //                            new DictionaryCollection2<string, TradeKeyItem>();
        //public ConcurrentDictionaryCollection2<string, TradeKeyItem> TradeKeyList =
        //                            new ConcurrentDictionaryCollection2<string, TradeKeyItem>();
        //public ConcurrentBagCollection2<string, TradeKeyItem> TradeKeyList =
        //                            new ConcurrentBagCollection2<string, TradeKeyItem>();
        // public ListContainer<string, TradeKeyItem> TradeKeyList = new ListContainer<string, TradeKeyItem>() ;

        public List<TradeKeyItem> TradeKeySamples { get; set; }
        public List<TradeKeyItem> TradeKeyAnotherSamples { get; set; }

        public int ItemCount;

        public List<IEventArgs> ListEventArgs = new List<IEventArgs>();

        [TestInitialize]
        public void SetUp()
        {
            ListEventArgs?.Clear();

            ItemCount = 5;

            TradeKeySamples = new List<TradeKeyItem>();
            TradeKeyAnotherSamples = new List<TradeKeyItem>();

            foreach (var i in Enumerable.Range(1, ItemCount))
            {
                var t = new TradeKeyItem {Account = "Acc" + i, TradeNumber = i%2 == 0 ? i*3 + 1 : i*2, Value = i};
                TradeKeySamples.Add(t);
            }
            foreach (var i in Enumerable.Range(1, ItemCount))
            {
                var t = new TradeKeyItem
                { Account = "Acc" + 1*i, TradeNumber = i % 2 == 0 ? i * 5 + 1 : i * 3, Value = i*100 };
                TradeKeyAnotherSamples.Add(t);
            }
            // 19.08.26
            TradeKeyList.ChangedEvent += ConsoleOutput;
            TradeKeyList.ChangedEvent += (ob, args) =>
            {
                var o = args.Object;
                ListEventArgs.Add(args);
                ConsoleSync.WriteLineT(args.ToString());
            };
        }
        private string CreateKey(string s1, string s2)
        {
            return string.Join("@", s1, s2.ToString());
        }

        private void ConsoleOutput(object o, IEventArgs args)
        {
            var ob = args.Object;
            ConsoleSync.WriteLineT(ob.ToString());
        }

        [TestMethod]
        public void Add_Remove_Item()
        {
            bool  res;
            res = TradeKeyList.Add(TradeKeySamples[0]);
            Assert.IsTrue(res);
            Assert.AreEqual(1,TradeKeyList.Count);

            res = TradeKeyList.AddNew(TradeKeySamples[0]);
            Assert.IsFalse(res);
            Assert.AreEqual(1, TradeKeyList.Count);

            res = TradeKeyList.Remove(TradeKeySamples[0]);
            Assert.IsTrue(res);
            Assert.AreEqual(0, TradeKeyList.Count);

            res = TradeKeyList.Add(TradeKeySamples[0]);
            Assert.IsTrue(res);
            Assert.AreEqual(1, TradeKeyList.Count);

            res = TradeKeyList.AddNew(TradeKeySamples[0]);
            Assert.IsFalse(res);
            Assert.AreEqual(1, TradeKeyList.Count);
            // !!!!!!!!! RemoveNokey
            res = TradeKeyList.RemoveNoKey(TradeKeySamples[1]);
            Assert.IsFalse(res);
            Assert.AreEqual(1, TradeKeyList.Count);

            res = TradeKeyList.RemoveNoKey(TradeKeySamples[0]);
            Assert.IsTrue(res);
            Assert.AreEqual(0, TradeKeyList.Count);
        }

        [TestMethod]
        public void VerifySamplesAreDifferent()
        {
            foreach(var i in TradeKeySamples)
                foreach (var j in TradeKeyAnotherSamples)
                {
                    Assert.AreNotEqual(i.ToString(),j.ToString(), $"Should Be Differ: {i}, {j}");
                }
        }

        [TestMethod]
        public void AddNew_SomeDifferItems_CountEqSumItems()
        {
            //TradeKeyList.AddNew(new TradeKeyItem {Account = "acc1", TradeNumber = MiliSeconds, Value = 1});
            //TradeKeyList.AddNew(new TradeKeyItem {Account = "acc2", TradeNumber = Days, Value = 2});

            var res = TradeKeyList.AddNew(null);
            Assert.IsFalse(res, "Should Be False");

            foreach (var i in TradeKeySamples)
            {
                var result = TradeKeyList.AddNew(i);
                Assert.IsTrue(result, "Should Be TRUE");
            }
            Assert.IsTrue(TradeKeyList.Count == ItemCount, $"Count Should Be {ItemCount}");
            foreach (var i in TradeKeyAnotherSamples)
            {
                var result = TradeKeyList.AddNew(i);
                Assert.IsTrue(result, "ShouldBe TRUE");
            }
            Assert.IsTrue(TradeKeyList.Count == 2*ItemCount, $"Count Should Be {2*ItemCount}");

            // The Same Items try to Add. Expect No Added

            foreach (var i in TradeKeyList.Items)
                ConsoleSync.WriteLineT(i.ToString());

            foreach (var i in TradeKeySamples)
            {
                var result = TradeKeyList.AddNew(i);
                Assert.IsFalse(result, "ShouldBe FALSE");
            }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}");

            foreach (var i in TradeKeyAnotherSamples)
            {
                var result = TradeKeyList.AddNew(i);
                Assert.IsFalse(result, "ShouldBe FALSE");
            }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}");

            Assert.IsTrue(ListEventArgs
               .Count(i => i.Operation == "AddNew".TrimUpper()) ==  2 * ItemCount,
               $"12 E: { 2 * ItemCount}, F: {ListEventArgs.Count}");

            foreach (var i in TradeKeyList.Items)
                ConsoleSync.WriteLineT(i.ToString());

        }
        //[TestMethod]
        //public void AddNew_SomeTheSameItems_CountEqOnlyNotIdentical()
        //{
        //    // See Up in the Previouse test

        //    AddNew_SomeDifferItems_CountEqSumItems();
        //    Assert.IsTrue(TradeKeyList.Count == 2*ItemCount, $"Count Should Be {2*ItemCount} : ");
        //    // Pass the Same items

        //    foreach (var i in TradeKeySamples)
        //    {
        //        var result = TradeKeyList.AddNew(i);
        //        Assert.IsFalse(result, "ShouldBe FALSE");
        //    }
        //    Assert.IsTrue(TradeKeyList.Count == 2*ItemCount, $"Count Should Be {2*ItemCount}");

        //    foreach (var i in TradeKeyAnotherSamples)
        //    {
        //        var result = TradeKeyList.AddNew(i);
        //        Assert.IsFalse(result, "ShouldBe FALSE");
        //    }
        //    Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}");
         
        //    foreach (var i in TradeKeyList.Items)
        //        ConsoleSync.WriteLineT(i.ToString());
        //}

        [TestMethod]
        public void GetByKey_SomeTheSameItems_CountEqOnlyNotIdentical()
        {
            AddNew_SomeDifferItems_CountEqSumItems();
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount} : ");

            string key = null;
            var item = TradeKeyList.GetByKey(key);
            Assert.IsNull(item, "Should Be NULL");

            //var result = TradeKeyList.Contains(null);
            //Assert.IsFalse(result, "Should be false");

            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.GetByKey(i.Key);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.GetByKey(i.Key);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }

            TradeKeyList.Clear();
            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.GetByKey(i.Key);
                Assert.IsFalse(i.IsEquals(t), $"Should Be Not The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.GetByKey(i.Key);
                Assert.IsFalse(i.IsEquals(t), $"Should Be Not The Same: {i}, {t}");
            }
        }

        [TestMethod]
        public void AddOrGet_SomeTheSameItems_CountEqOnlyNotIdentical()
        {
            AddNew_SomeDifferItems_CountEqSumItems();
            Assert.IsTrue(TradeKeyList.Count == 2*ItemCount, $"Count Should Be {2*ItemCount} : ");

            var item = TradeKeyList.AddOrGet(null);
            Assert.IsNull(item, "Should Be False");

            var oneitem = TradeKeyList.Items.FirstOrDefault()?.Clone();
            Assert.IsNotNull(oneitem);
            var rightKey = oneitem.Key;
            oneitem.Account = null;
            item = TradeKeyList.AddOrGet(oneitem);
            Assert.IsNotNull(item, $"Should Be Not Null: {item.Key}", $"RightKey: {item.Key}");
            // ConsoleSync.WriteLineT($"BadKey: {oneitem.Key}, RightKey: {rightKey}");
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount + 1, $"Count Should Be {2 * ItemCount + 1}");
            foreach (var i in TradeKeyList.Items)
                ConsoleSync.WriteLineT(i.ToString());
            ConsoleSync.WriteLineT($"************* TradeKeyList.Count: {TradeKeyList.Count}");
            TradeKeyList.Remove(oneitem.Key);
            foreach (var i in TradeKeyList.Items)
                ConsoleSync.WriteLineT(i.ToString());
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}, but has {TradeKeyList.Count} ");

            ConsoleSync.WriteLineT($"************* TradeKeyList.Count: {TradeKeyList.Count}");
            

            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.AddOrGet(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.AddOrGet(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}, but has {TradeKeyList.Count} ");

            TradeKeyList.Clear();
            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.AddOrGet(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.AddOrGet(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }

            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount} : ");

            Assert.IsTrue(ListEventArgs
               .Count(i => i.Operation == "AddOrGet".TrimUpper()) ==  2 * ItemCount + 1,
               $"12 E: { 2 * ItemCount + 1}, F: {ListEventArgs.Count}");
        }
        [TestMethod]
        public void Remove_SomeTheSameItems_CountEqOnlyNotIdentical()
        {
            AddNew_SomeDifferItems_CountEqSumItems();
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"0 Count Should Be {2 * ItemCount} : ");

            TradeKeyItem n = null;
            var res = TradeKeyList.Remove(n);
            Assert.IsFalse(res, "1 Should Be False");

            string s = null;
            res = TradeKeyList.Remove(s);
            Assert.IsFalse(res, "2 Should Be False");

            foreach (var i in TradeKeyAnotherSamples)
            {
                var r = TradeKeyList.Remove(i);
                Assert.IsTrue(r, $"3 Should Be True: {r}");
            }
            Assert.IsTrue(TradeKeyList.Count == 1 * ItemCount, $"4 Count Should Be {1 * ItemCount} : ");
            foreach (var i in TradeKeySamples)
            {
                var r = TradeKeyList.Remove(i);
                Assert.IsTrue(r, $"5 Should Be True: {r}");
            }
            Assert.IsTrue(TradeKeyList.Count == 0 * ItemCount, $"6 Count Should Be {0 * ItemCount} : ");

            // TradeKeyList.Clear();
            foreach (var i in TradeKeyAnotherSamples)
            {
                var r = TradeKeyList.Remove(i);
                Assert.IsFalse(r, $"7 Should Be False: {r}");
            }
            foreach (var i in TradeKeySamples)
            {
                var r = TradeKeyList.Remove(i);
                Assert.IsFalse(r, $"8 Should Be False: {r}");
            }
            Assert.IsTrue(TradeKeyList.Count == 0 * ItemCount, $"9 Count Should Be {0 * ItemCount} : ");

            Assert.IsTrue(ListEventArgs
               .Count(i => i.Operation == "Remove".TrimUpper()) == 2 * ItemCount,
               $"12 E: {2 * ItemCount}, F: {ListEventArgs.Count}");
        }
        [TestMethod]
        public void AddOrUpdate_SomeTheSameItems_CountEqOnlyNotIdentical()
        {
            AddNew_SomeDifferItems_CountEqSumItems();

            var item = TradeKeyList.AddOrUpdate(null);
            Assert.IsNull(item, "Should Be False");

            foreach (var i in TradeKeySamples)
                foreach (var j in TradeKeyAnotherSamples)
                {
                    var iv = i.Value;
                    var jv = j.Value;
                    i.Value = jv;
                    j.Value = iv;
                }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount}, but is {TradeKeyList.Count}");
            // Update At First
            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.AddOrUpdate(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.AddOrUpdate(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount} : ");

            // Add at Second
            TradeKeyList.Clear();
            foreach (var i in TradeKeyAnotherSamples)
            {
                var t = TradeKeyList.AddOrUpdate(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            foreach (var i in TradeKeySamples)
            {
                var t = TradeKeyList.AddOrUpdate(i);
                Assert.IsTrue(i.IsEquals(t), $"Should Be The Same: {i}, {t}");
            }
            Assert.IsTrue(TradeKeyList.Count == 2 * ItemCount, $"Count Should Be {2 * ItemCount} : ");

            Assert.IsTrue(ListEventArgs
                .Count == 2 * 3 * ItemCount, $"11 E: {2 * 3 * ItemCount}, F: {ListEventArgs.Count}");

            Assert.IsTrue(ListEventArgs
                .Count(i => i.Operation == "AddOrUpdate".TrimUpper()) == 2 * 2 * ItemCount,
                $"12 E: {2 * 2 * ItemCount}, F: {ListEventArgs.Count}");

            ConsoleSync.WriteLineT("-----------------------------------------------------");
            foreach (var i in TradeKeyList.Items)
                ConsoleSync.WriteLineT(i.ToString());
        }
    }
}
