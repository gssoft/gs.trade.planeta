using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GS.Cloning
{
    public static class Cloner
    {
        public static object Clone(object something)
        {
            XmlSerializer serializer = new XmlSerializer(something.GetType());
            MemoryStream tempStream = new MemoryStream();
            serializer.Serialize(tempStream, something);
            tempStream.Seek(0, SeekOrigin.Begin);
            return serializer.Deserialize(tempStream);
        }
    }
}
