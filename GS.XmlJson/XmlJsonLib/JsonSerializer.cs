// https://www.newtonsoft.com/json/help/html/ConvertXmlToJson.htm
// https://www.newtonsoft.com/json/help/html/SerializeToBson.htm
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace XmlJsonLib
{
    public class Event
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
    }
    public class JsonSerializer
    {
        public static string ConvertXmlFileToJson(string xmlfile)
        {
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlfile);
            string json = JsonConvert.SerializeXmlNode(xmldoc);
            return json;
        }
        public static string ConvertXmlDocToJson(XmlDocument xmldoc)
        {
            return  JsonConvert.SerializeXmlNode(xmldoc);
        }
    }
    // https://www.newtonsoft.com/json/help/html/DeserializeFromBson.htm
    public class BsonSerializer
    {
        // 
        //public static string SerializeClassToBson(Event e)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    using (var writer = new BsonDataWriter(ms))
        //    {
        //        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //        serializer.Serialize(writer, e);
        //    }
        //    return Convert.ToBase64String(ms.ToArray());
        //}
        // ToBase64String
        public static string SerializeClassToBson<T>(T obj)
        {
            MemoryStream ms = new MemoryStream();
            using (var writer = new BsonDataWriter(ms))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public static string SerializeCollectionToBson<T>(IList<T> obj)
        {
            MemoryStream ms = new MemoryStream();
            using (var writer = new BsonDataWriter(ms))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public static T Deserialize<T>(string s)
        {
            byte[] data = Convert.FromBase64String(s);
            MemoryStream ms = new MemoryStream(data);
            T e;
            using (var reader = new BsonDataReader(ms))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                e = serializer.Deserialize<T>(reader);
            }
            return e;
        }
        public static IList<T> DeserializeCollection<T>(string s)
        {
            byte[] data = Convert.FromBase64String(s);
            MemoryStream ms = new MemoryStream(data);
            using (var reader = new BsonDataReader(ms))
            {
                reader.ReadRootValueAsArray = true;
                var serializer = new Newtonsoft.Json.JsonSerializer();
                var events = serializer.Deserialize<IList<T>>(reader);
                return events;
            }
        }
    }
    //
    // https://learn.microsoft.com/ru-ru/dotnet/api/system.convert.frombase64string?view=net-7.0#code-try-1
    // string UUencoded (base-64)
    internal static class BytesToBase64StringConverter
    {
        // byte[] bytes = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
        public static string ToBase64String(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);

            return s;
        }
        public static byte[] ToBytes(String s)
        {
            byte[] newBytes = Convert.FromBase64String(s);
            return newBytes;
        }
    }
}

    