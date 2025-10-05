using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Extension
{
    public static class CryptoExtensions
    {
        public static string GetHashSha1(this byte[] bytes)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
        }
        public static string GetHashSha1(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
        }
        public static string GetHashSha1(this Stream stream)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(stream));
            }
        }
        //    public static string GetHashSha1FromFileStream(this string filename)
        //    {
        //        using (var stream = File.OpenRead(filename))
        //        {
        //            return stream.GetHashSha1();
        //        }
        //    }
        //    public static string GetHashSha1FromFileLinesBytes(this string filename)
        //    {
        //        string content;
        //        using (var stream = File.OpenRead(filename))
        //        using (var reader = new StreamReader(stream))
        //        {
        //            content = reader.ReadToEnd();
        //        }
        //        var bytes = content.ToLinesInBytes();
        //        return bytes.GetHashSha1();
        //    }

        //    //public static string GetHashSha1FromFileBytesWithoutrn(string filename)
        //    //{
        //    //    string content;
        //    //    using (var stream = File.OpenRead(filename))
        //    //    using (var reader = new StreamReader(stream))
        //    //    {
        //    //        content = reader.ReadToEnd();
        //    //        stream.Close();
        //    //        reader.Close();
        //    //    }
        //    //    content = content.Replace("\r", "").Replace("\n", "");
        //    //    var bs = Encoding.UTF8.GetBytes(content);

        //    //    return bs.GetHashSha1();
        //    //}
        //    public static string GetHashSha1FromFileAllBytes(this string filename)
        //    {
        //        var bytes = File.ReadAllBytes(filename);  // !!!!!
        //        return bytes.GetHashSha1();
        //    }

        // ********* MD5 ***********
        public static string GetHashMd5(this byte[] bytes)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???
                // return Convert.ToBase64String(hash);
            }
        }
        public static string GetHashMd5(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???
               // return Convert.ToBase64String(hash);
            }
        }
        public static string GetHashMd5(this Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???
                // return Convert.ToBase64String(hash);
            }
        }
        //    // String = FileName
        //    public static string GetHashMd5FromFileStream(this string filename)
        //    {
        //        using (var stream = File.OpenRead(filename))
        //        {
        //            return stream.GetHashMd5();
        //        }
        //    }
        //    public static string GetHashMd5FromFileLinesSplitedInBytes(this string filename)
        //    {
        //        string content;
        //        using (var stream = File.OpenRead(filename))
        //        using (var reader = new StreamReader(stream))
        //        {
        //            content = reader.ReadToEnd();
        //        }
        //        var bytes = content.ToLinesInBytes(); // !!!!!
        //        return bytes.GetHashMd5();
        //    }
        //    public static string GetHashMd5FromFileContentInBytes(this string filename)
        //    {
        //        string content;
        //        using (var stream = File.OpenRead(filename))
        //        using (var reader = new StreamReader(stream))
        //        {
        //            content = reader.ReadToEnd();
        //        }
        //        var bytes = content.ToBytes();  // !!!!!
        //        return bytes.GetHashMd5();
        //    }
        //    public static string GetHashMd5FromFileAllBytes(this string filename)
        //    {
        //        var bytes = File.ReadAllBytes(filename);  // !!!!!
        //        return bytes.GetHashMd5();
        //    }
    }

        public static class Crypto
    {
        // ********* SHA1 ***********
        public static string GetHashSha1(byte[] bytes)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
        }
        public static string GetHashSha1(string s)
        {
            var bytes = s.ToBytes();
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
        }
        public static string GetHashSha1(Stream stream)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(stream));
            }
        }
        public static string GetHashSha1FromFileStream(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return GetHashSha1(stream);
            }
        }
        public static string GetHashSha1FromFileInString(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            return GetHashSha1(content);
        }
        public static string GetHashSha1FromFileLinesSplitedInBytes(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            var bytes = GSstring.ToLinesInBytes(content); // !!!!
            return GetHashSha1(bytes);
        }
        public static string GetHashSha1FromFileBytesWithoutrn(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            content = content.Replace("\r", "").Replace("\n", "");
            return GetHashSha1(content);
        }
        public static string GetHashSha1FromFileAllBytes(string filename)
        {
            var bytes = File.ReadAllBytes(filename);  // !!!!!
            return GetHashSha1(bytes);
        }

        // ********* MD5 ***********
        public static string GetHashMd5(byte[] bytes)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(bytes);
            }
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???
            // return Convert.ToBase64String(hash);
        }
        public static string GetHashMd5(string s)
        {
            var bytes = s.ToBytes();
            byte[] hash; 
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(bytes);
            }
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???
           // return Convert.ToBase64String(hash);
        }
        public static string GetHashMd5(Stream stream)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(stream);
            }
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // ???                                            
            // return Convert.ToBase64String(hash);
        }
        // String = FileName
        public static string GetHashMd5FromFileStream(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return GetHashMd5(stream);
            }
        }
        public static string GetHashMd5FromFileInString(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            return GetHashMd5(content);
        }
        public static string GetHashMd5FromFileLinesSplitedInBytes(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }
            var bytes = GSstring.ToLinesInBytes(content); // !!!!!
            return GetHashMd5(bytes);
        }
        public static string GetHashMd5FromFileBytesWithoutrn(string filename)
        {
            string content;
            using (var stream = File.OpenRead(filename))
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            content = content.Replace("\r", "").Replace("\n", "");
            return GetHashMd5(content);
        }
        public static string GetHashMd5FromFileAllBytes(string filename)
        {
            var bytes = File.ReadAllBytes(filename);  // !!!!!
            return GetHashMd5(bytes);
        }
    }
}
