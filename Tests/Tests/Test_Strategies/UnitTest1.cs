using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using GS.ConsoleAS;
using GS.Serialization;
using GS.Trade.Strategies;

namespace Test_Strategies
{
    [TestClass]
    public class TestStrategies
    {
        public Strategies Strategies; 

        [TestInitialize]
        public void Setup()
        {
            Strategies = new Strategies();
            //{
            //    StrategiesInitXmlFile = "StrategiesXmlFile",
            //    StrategiesXmlElementPath = "Strategies"
            //};
        }
        [TestMethod]
        public void Test01_StrategiesNew()
        {
            var s = new Strategies();
            Assert.IsNotNull(s,"Strategies is null");
        }
        // 
        [TestMethod]
        public void Test021_StrategiesSerialization()
        {
            const string relation = @"..\..\XmlFiles\";
            var path = Path.Combine(Environment.CurrentDirectory, relation, "ss.xml");
            var f = Do.Serialize( path, Strategies);
            Assert.IsTrue(f, "Serialization Failure");
            Assert.IsNull(Strategies.StrategiesInitXmlFile);
            Assert.IsNull(Strategies.StrategiesXmlElementPath);
            ConsoleSync.WriteLineT($@"InitFile: {Strategies.StrategiesInitXmlFile}, FilePath: {Strategies.StrategiesXmlElementPath}");
        }
        [TestMethod]
        public void Test022_StrategiesSerialization()
        {
            Strategies.StrategiesInitXmlFile = "StrategiesXmlFile";
            Strategies.StrategiesXmlElementPath = "Strategies";
            const string relation = @"..\..\XmlFiles\";
            var path = Path.Combine(Environment.CurrentDirectory, relation, "ss.xml");
            var f = Do.Serialize(path, Strategies);
            Assert.IsTrue(f, "Serialization Failure");
            Assert.IsNotNull(Strategies.StrategiesInitXmlFile);
            Assert.IsNotNull(Strategies.StrategiesXmlElementPath);
            ConsoleSync.WriteLineT($@"InitFile: {Strategies.StrategiesInitXmlFile}, FilePath: {Strategies.StrategiesXmlElementPath}");
        }
        [TestMethod]
        public void Test03_StrategiesDeSerialization()
        {
            const string relation = @"..\..\XmlFiles\";
            var path = Path.Combine(Environment.CurrentDirectory, relation, "ss.xml");
            var ss = Builder.DeSerialization<Strategies>(path, "Strategies");
            Assert.IsNotNull(ss, "DeSerialization Failure");
            Assert.IsNotNull(ss.StrategiesInitXmlFile);
            Assert.IsNotNull(ss.StrategiesXmlElementPath);
            ConsoleSync.WriteLineT($@"InitFile: {Strategies.StrategiesInitXmlFile}, FilePath: {Strategies.StrategiesXmlElementPath}");
        }
        [TestMethod]
        public void Test04_StrategiesFields()
        {
            var xml_file = Strategies.StrategiesInitXmlFile;
            var element_path = Strategies.StrategiesXmlElementPath;
            Assert.IsNull(xml_file, "XmlFile is Null");
            Assert.IsNull(element_path, "Xml_Element is Null");

            // Assert.IsNotNull(ss, "DeSerialization Failure");
        }
        [TestMethod]
        public void Test05_StrategiesDeSerialization()
        {
            const string relation = @"..\..\XmlFiles\";
            var path = Path.Combine(Environment.CurrentDirectory, relation, "ss2.xml");
            var ss = Builder.DeSerialization<Strategies>(path, "Strategies");
            var initfile = ss.StrategiesInitXmlFile;
            var elempath = ss.StrategiesXmlElementPath;
            Assert.IsNotNull(initfile, "StrategiesInitXmlFile IsNull");
            Assert.IsNotNull(elempath,  "StrategiesXmlElementPath IsNull");
            ConsoleSync.WriteLineT($@"InitFile: {initfile}, FilePath: {elempath}");
        }
    }
}
