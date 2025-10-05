using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GS.Serialization
{
    public class XmlSerialization
    {
        public static void Serialize<T>(XDocument doc, List<T> list)
        {
            var serializer = new XmlSerializer(list.GetType());
            var writer = doc.CreateWriter();
            serializer.Serialize(writer, list);
            writer.Close();
        }
        public static List<T> Deserialize<T>(XDocument doc)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            var reader = doc.CreateReader();
            var result = (List<T>)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }
        public static void Serialize<T>(XDocument doc, List<T> list, Action<Exception> excaction)
        {
            try
            {
                var serializer = new XmlSerializer(list.GetType());
                var writer = doc.CreateWriter();
                serializer.Serialize(writer, list);
                writer.Close();
            }
            catch (Exception e)
            {
                excaction.Invoke(e);                
            }
        }
        public static List<T> Deserialize<T>(XDocument doc, Action<Exception> excaction)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (List<T>));
                var reader = doc.CreateReader();
                var result = (List<T>) serializer.Deserialize(reader);
                reader.Close();
                return result;
            }
            catch (Exception e)
            {
                excaction.Invoke(e);
            }
            return null;
        }
    }
}
