// Compress Strings With .NET and C#, Encoding, Compress
// https://www.blogger.com/blog/post/edit/3353785703431617754/8129728896804717972
// https://gs-softpro.blogspot.com/search/label/Compress
// https://www.blogger.com/blog/post/edit/1224718110936480928/7085861564975900220
// BinarySerialization, Compress, GZip, Шифрование
// // https://learn.microsoft.com/ru-ru/dotnet/api/system.convert.frombase64string?view=net-7.0#code-try-1

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace XmlJsonLib
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Convert xmlfile to json\n");

            // XML => JSON
            string xmlfile = @"<?xml version='1.0' standalone='no'?>
                    <root>
                        <person id='1'>
                            <name>Alan</name>
                            <url>http://www.google.com</url>
                            </person>
                            <person id='2'>
                            <name>Louis</name>
                            <url>http://www.yahoo.com</url>
                        </person>
                    </root>";

            // var xmldoc = new XmlDocument();
            // xmldoc.LoadXml(xmlfile);
            // string json = JsonConvert.SerializeXmlNode(xmldoc);

            string json = JsonSerializer.ConvertXmlFileToJson(xmlfile);

            Console.WriteLine(json);
            Console.WriteLine("End xml = > json\n");


            // BSON Writer
            Console.WriteLine("BSON Serializer");

            var evnt = new Event()
            {
                Name = "Movie Premiere",
                // StartDate = new DateTime(2013, 1, 22, 20, 30, 0, DateTimeKind.Utc)
                StartDate = new DateTime(2013, 1, 22, 20, 30, 0)
            };
            // ToBase64String
            var data = BsonSerializer.SerializeClassToBson(evnt);
            var obj = BsonSerializer.Deserialize<Event>(data);
            Debug.Assert(evnt.Name == obj.Name && evnt.StartDate == obj.StartDate, "Sorry. Does not equal.");

            var evnt2 = new Event()
            {
                Name = "I'm King",
                StartDate = new DateTime(2023, 9, 3, 16, 0, 0)
            };

            var lst = new List<Event>() { evnt, evnt2 };
            var lst_string = BsonSerializer.SerializeCollectionToBson(lst);
            var lst2 = BsonSerializer.DeserializeCollection<Event>(lst_string);

            Debug.Assert(
                lst[0].Name == lst2[0].Name && lst[0].StartDate == lst2[0].StartDate &&
                lst[1].Name == lst2[1].Name && lst[1].StartDate == lst2[1].StartDate,
                "Sorry. Does not equal."
                );

            Console.WriteLine("Bson\n");
            Console.WriteLine(data);
            Console.ReadLine();

            // === BytesToBase64StringConverter 
            // https://learn.microsoft.com/ru-ru/dotnet/api/system.convert.frombase64string?view=net-7.0#code-try-1
            Console.WriteLine("BytesToBase64StringConverter:\n");

            byte[] bytes = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            string s = BytesToBase64StringConverter.ToBase64String(bytes);
            Console.WriteLine("The base 64 string:\n   {0}\n", s);

            byte[] newBytes = BytesToBase64StringConverter.ToBytes(s);
            
            Console.WriteLine("The restored byte array: ");
            Console.WriteLine("   {0}\n", BytesToBase64StringConverter.ToBase64String(newBytes));
            Console.WriteLine("   {0}\n", BitConverter.ToString(newBytes));

            Console.ReadLine();
        }
    }
}
