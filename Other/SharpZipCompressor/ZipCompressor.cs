using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpZipCompressor
{
    // Load from Nuget ICSharpCode.SharpZipLib.Zip
    // Install-Package ICSharpCode.SharpZipLib.dll

    public class ZipCompressor
    {
        /// <summary>
        /// Заархивировать файл
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static byte[] CompressFile(byte[] inputData, string filename)
        {
            var output = new MemoryStream();
            var gz = new ZipOutputStream(output);
            var ze = new ZipEntry(filename);
            gz.PutNextEntry(ze);
            gz.Write(inputData, 0, inputData.Length);
            gz.Finish();

            byte[] outputData = output.ToArray();
            gz.Close();
            return outputData;
        }

        /// <summary>
        /// Пытается разархивировать файл. Если не получается, возвращает неразархивированный файл.
        /// </summary>
        /// <param name="bs">Byte-array файла</param>
        public static Byte[] TryDecompressFile(byte[] bs)
        {
            var input = new MemoryStream(bs);
            var gz = new ZipInputStream(input);
            byte[] content;
            var output = new MemoryStream();
            try
            {
                var bufsize = 4096;
                var data = new byte[bufsize];
                var ze = gz.GetNextEntry();
                content = new byte[ze.Size];
                while (true)
                {
                    var size = gz.Read(data, 0, data.Length);
                    if (size > 0) output.Write(data, 0, size); else break;
                }
                output.Flush();
                output.Position = 0;
                output.Read(content, 0, Convert.ToInt32(output.Length));
            }
            catch
            {
                content = bs;
            }
            finally
            {
                output.Dispose();
                input.Dispose();
                gz.Dispose();
            }
            return content;
        }
    }
}
