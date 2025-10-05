using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FtpRequestParse_01
{
    class Program
    {
        static void Main(string[] args)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@"ftp://62.173.145.180/www/stellamaris.su/");
            request.Credentials = new NetworkCredential("user41381", "kPywCImxAZ2z");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());

            //Console.WriteLine(reader.ReadToEnd());
            //Console.ReadLine();

            // UnixLineParse(reader);

            var lst = UnixFtpStreamParse(reader);

            foreach (var i in lst
                                .OrderByDescending(s => s.IsCurrentDirectory)
                                .ThenByDescending(s => s.IsParentDirectory)
                                .ThenByDescending(s=>s.IsDirectory))

                Console.WriteLine(i.ToString());

            Console.ReadLine();
        }

        private static void UnixLineParse(StreamReader reader)
        {
            string pattern =
                @"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
                @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";
            Regex regex = new Regex(pattern);
            IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");
            string[] hourMinFormats =
                new[] {"MMM dd HH:mm", "MMM dd H:mm", "MMM d HH:mm", "MMM d H:mm"};
            string[] yearFormats =
                new[] {"MMM dd yyyy", "MMM d yyyy"};

            var lst = new List<FtpFileInfo>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                Match match = regex.Match(line);
                string permissions = match.Groups[1].Value;
                int inode = int.Parse(match.Groups[2].Value, culture);
                string owner = match.Groups[3].Value;
                string group = match.Groups[4].Value;
                long size = long.Parse(match.Groups[5].Value, culture);

                DateTime modified;
                string s = Regex.Replace(match.Groups[6].Value, @"\s+", " ");
                if (s.IndexOf(':') >= 0)
                {
                    modified = DateTime.ParseExact(s, hourMinFormats, culture, DateTimeStyles.None);
                }
                else
                {
                    modified = DateTime.ParseExact(s, yearFormats, culture, DateTimeStyles.None);
                }
                string name = match.Groups[7].Value;

                lst.Add(new FtpFileInfo
                {
                    Modified = modified,
                    Name = name,
                    Permissions = permissions,
                    Size = size,
                   // IsDirectory = permissions.Substring(0,1) == "d"
                });

                //Console.WriteLine(
                //    "{0,-16} perms: {1}, size: {2, 9}, modified = {3}",
                //    name, permissions, size, modified.ToString("yyyy-MM-dd HH:mm"));
            }
            foreach(var i in lst.OrderByDescending(s=>s.IsDirectory))
                Console.WriteLine(i.ToString());

            Console.ReadLine();
        }
        private static IEnumerable<FtpFileInfo> UnixFtpStreamParse(StreamReader reader)
        {
            var lst = new List<FtpFileInfo>();
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fi = ParseFtpRequestLine(line);
                    if(fi != null)
                        lst.Add(fi);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lst;
        }

        private static FtpFileInfo ParseFtpRequestLine(string line)
        {
            string pattern =
                @"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
                @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";

            Regex regex = new Regex(pattern);

            IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");

            string[] hourMinFormats =
                { "MMM dd HH:mm", "MMM dd H:mm", "MMM d HH:mm", "MMM d H:mm" };
            string[] yearFormats =
                { "MMM dd yyyy", "MMM d yyyy" };

            try
            {
                Match match = regex.Match(line);
                string permissions = match.Groups[1].Value;
                int inode = int.Parse(match.Groups[2].Value, culture);
                string owner = match.Groups[3].Value;
                string group = match.Groups[4].Value;
                long size = long.Parse(match.Groups[5].Value, culture);

                DateTime modified;
                string s = Regex.Replace(match.Groups[6].Value, @"\s+", " ");
                if (s.IndexOf(':') >= 0)
                {
                    modified = DateTime.ParseExact(s, hourMinFormats, culture, DateTimeStyles.None);
                }
                else
                {
                    modified = DateTime.ParseExact(s, yearFormats, culture, DateTimeStyles.None);
                }
                string name = match.Groups[7].Value;

                return new FtpFileInfo
                {
                    Modified = modified,
                    Name = name,
                    Permissions = permissions,
                    Size = size,
                    // IsDirectory = permissions.Substring(0, 1) == "d",
                    Node = inode,
                    Owner = owner,
                    Group = group,
                    // IsCurrentDirectory = name.Trim() == ".",
                    // IsParentDirectory = name.Trim() == "..",
                    DirMemberType = permissions.Substring(0, 1) != "d"
                        ? DirMemberEnum.File
                        : (name.Trim() == "." 
                            ? DirMemberEnum.CurrentDir
                            : (name.Trim() == ".." ? DirMemberEnum.ParentDir : DirMemberEnum.Dir))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }            
        }
    }

}
