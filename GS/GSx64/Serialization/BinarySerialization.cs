using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GS.Serialization
{

    // See ByteArray.cs in the same Catalog
    public static class BinarySerialization
    {
        public static byte[] SerializeToByteArray<T>(T o)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                    bf.Serialize(ms, o);
                    ms.Seek(0, 0);

                    var bts = ms.ToArray();
                    return bts;
            }
        }
        public static T DeSerialize<T>(byte[] bytes)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, 0);

                var dst =  (T) bf.Deserialize(ms);
                
                return dst;
            }
        }
    }
}
