using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace GS.Extension
{
    public static class StreamExtensions
    {
        // https://stackoverflow.com/questions/2949666/deserialize-stream-to-listt-or-any-other-type
        public static Stream Serialize<T>(this T o) where T : new()
        {
            Stream stream = new MemoryStream();
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(stream, typeof(T));
            return stream;
        }
        public static T Deserialize<T>(this Stream stream) where T : new()
        {
            BinaryFormatter bin = new BinaryFormatter();
            return (T)bin.Deserialize(stream);
        }
        public static void WriteTo(this Stream source, Stream destination)
        {
            byte[] buffer = new byte[32 * 1024];
            //source.Position = 0;
            if (source.Length < buffer.Length) buffer = new byte[source.Length];
            int read = 0;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, read);
            }
            //source.Position = 0;
            //destination.Position = 0;
        }
        public static void WriteTo1(this Stream source, Stream destination)
        {
            byte[] buffer = new byte[32 * 1024];
            //source.Position = 0;
            if (source.Length < buffer.Length) buffer = new byte[source.Length];
            int read = 0;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, read);
            }
            //source.Position = 0;
            destination.Position = 0;
        }
        public static void WriteTo2(this Stream source, Stream destination)
        {
            byte[] buffer = new byte[32 * 1024];
            source.Position = 0;
            if (source.Length < buffer.Length) buffer = new byte[source.Length];
            int read = 0;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, read);
            }
            source.Position = 0;
            destination.Position = 0;
        }

        public static byte[] CopyToBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static byte[] CopyToBytes(this Stream stream, int buffersize)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms, buffersize);
                return ms.ToArray();
            }
        }
        //public static Task<byte[]> async CopyToBytesAsync(this Stream stream)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var r = stream.CopyToAsync(ms);
        //        return ms.ToArray();
        //    }
        //}
    }
}
