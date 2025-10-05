using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Extension;

namespace CA_FileReadAndHash_01
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt1, dt2 ;
            TimeSpan diff1, diff2;
            var fileFullName = @"F:\Web\StellaMaris\171128\All_Image\stellamaris.su.test\mycontent\drevo\drevo.pdf";
            dt1 = DateTime.Now;
            var hash1 = GetHashSha1FromFileLinesBytes2(fileFullName);
            dt2 = DateTime.Now;
            diff1 = dt2 - dt1;

           // Task.Delay(15*1000).Wait();

           // ConsoleSync.WriteReadLineT("Press any key ...");

            dt1 = DateTime.Now;
            var hash2 = GetHashSha1FromFileLinesBytes(fileFullName);
            dt2 = DateTime.Now;
            diff2 = dt2 - dt1;

            //Task.Delay(15 * 1000).Wait();

            ConsoleSync.WriteReadLineT($"{hash1} {hash2} {hash1 == hash2} diff1: {diff1}, diff2: {diff2}");
        }

        public static string GetHashSha1FromFileLinesBytes(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }
            var bytes = ToLinesInBytes(content);
            return Crypto.GetHashSha1(bytes);
        }

        public static byte[] ToLinesInBytes(string content)
        {
            var spl = content.Split('\r', '\n');

            content = null;

            return spl.SelectMany(s =>
                       Encoding.UTF8.GetBytes(s
                       // + Environment.NewLine
                       )).ToArray();
        }

        public static string GetHashSha1FromFileLinesBytes2(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd(); 
                stream.Close();
                reader.Close();
            }
            content = content.Replace("\r", "").Replace("\n", "");            
            var bs = Encoding.UTF8.GetBytes(content);

            return Crypto.GetHashSha1(bs);
        }
        public static byte[] ToLineBytes(string content)
        {
            var s = content.Replace("\r", "").Replace("\n", "");
            return Encoding.UTF8.GetBytes(s);
        }
       public static byte[] ToLinesInBytes3(string content)
        {
            var spl = content.Split('\r', '\n');

            var bytes = new ArrayList();
            foreach (var s in spl)
            {
                var bs = Encoding.UTF8.GetBytes(s);
                bytes.AddRange(bs);
            }

            var a = spl.SelectMany(s =>
                Encoding.UTF8.GetBytes(s));

            return spl.SelectMany(s =>
                       Encoding.UTF8.GetBytes(s)).ToArray();
        }
    }
}
