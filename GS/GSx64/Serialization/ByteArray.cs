using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace GS.Serialization
{
    public static class ByteArrayHelper
    {
        public static byte[] ToByteArray(this object ob)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, ob);
                ms.Seek(0, 0);

                return ms.ToArray();
            }
        }
        public static T ToObject<T>(this byte[] ba) where T : class
        {
            var bf = new BinaryFormatter();
            using (var stream = new MemoryStream(ba))
            {
                return bf.Deserialize(stream) as T;
            }
        }

        public static byte[] ToByteArrayCompressed(this object ob)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            using (var msCompressed = new MemoryStream())
            using (var compressionStream = new GZipStream(msCompressed, CompressionMode.Compress))
            {
                bf.Serialize(ms, ob);
                ms.Seek(0, 0);

                var byteToCompressed = ms.ToArray();
                compressionStream.Write(byteToCompressed, 0, byteToCompressed.Length);

                msCompressed.Seek(0, 0);
                return msCompressed.ToArray();
            }
        }

        public static T ToObjectCompressed<T>(this byte[] ba) where T : class
        {
            var bytesDecompressed = new byte[ba.Length];
            var bf = new BinaryFormatter();
            using (var bytesCompressed = new MemoryStream(ba))
            using (var decompressionStream = new GZipStream(bytesCompressed, CompressionMode.Decompress))
            {
                decompressionStream.Read(bytesDecompressed, 0, ba.Length);
                return bf.Deserialize(new MemoryStream(bytesDecompressed)) as T;
            }
        }

        public static async Task<byte[]> ToByteArrayCompressedAsync(this object ob)
        {
            var bf = new BinaryFormatter();

            using (var ms = new MemoryStream())
            using (var msCompressed = new MemoryStream())
            using (var compressionStream = new GZipStream(msCompressed, CompressionMode.Compress))
            {
                bf.Serialize(ms, ob);
                ms.Seek(0, 0);

                var byteCompressed = ms.ToArray();

                await compressionStream.WriteAsync(byteCompressed, 0, byteCompressed.Length);

                msCompressed.Seek(0, 0);
                return msCompressed.ToArray();
            }
        }
        public static async Task<T> ToObjectCompressedAsync<T>(this byte[] ba) where T : class
        {
            var bytesDecompressed = new byte[ba.Length];
            var bf = new BinaryFormatter();
            using (var bytesCompressed = new MemoryStream(ba))
            using (var decompressionStream = new GZipStream(bytesCompressed, CompressionMode.Decompress))
            {
                var i = await decompressionStream.ReadAsync(bytesDecompressed, 0, ba.Length);

                return bf.Deserialize(new MemoryStream(bytesDecompressed, 0, i)) as T;
            }
        }
    }
}
