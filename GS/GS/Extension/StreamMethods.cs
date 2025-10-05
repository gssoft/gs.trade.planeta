using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Extension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace GS.Extension
    {
        public static class StreamMethods
        {
            private static byte[] ReadFully(Stream input)
            {
                byte[] buffer = new byte[16 * 1024];
                using (var ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            public static byte[] StreamToByteArray(Stream stream)
            {
                var memoryStream = stream as MemoryStream;
                return memoryStream?.ToArray() ?? ReadFully(stream);
            }
        }
    }
}
