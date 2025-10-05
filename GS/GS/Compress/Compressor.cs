using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace GS.Compress
{
    public static class Compressor
    {
        public static byte[] Compress(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                return ms.ToArray();
            }
        }
        public static byte[] DeCompress(byte[] bytes)
        {
            using (var gzstream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
            {
                const int size = 8192;
                var buffer = new byte[size];
                using (var mstr = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = gzstream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            mstr.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return mstr.ToArray();
                }
            }
        }
    }
}
