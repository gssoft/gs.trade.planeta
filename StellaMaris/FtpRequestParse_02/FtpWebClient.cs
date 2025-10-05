using GS.ConsoleAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using GS.Extension;
using GS.Serialization;

namespace FtpRequestParse_02
{
    public class FtpWebClient
    {
        protected FtpWebRequest FtpWebRequest { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string BaseAddress { get; set; }

        public bool IsUnixFtpServer { get; set; }

        public bool Init()
        {
            try
                {
                    FtpWebRequest = (FtpWebRequest)WebRequest.Create(BaseAddress);
                    FtpWebRequest.Credentials = new NetworkCredential(User, Password);

                    if (FtpWebRequest == null)
                        return false;

                    ConsoleSync.WriteLineT("Init() is Success");
                    return true;
                }
                catch (Exception ex)
                {
                    ConsoleSync.WriteLineT("Init() is Wrong");
                    ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                    return false;
                }
        }

        public Stream GetListDirectoryDetailsStream(string dir)
        {
            try
            {
                FtpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                return FtpWebRequest.GetResponse().GetResponseStream();

            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(ex.Message);
                return null;
            }           
        }
        public IEnumerable<FtpFileInfo> GetListDirectoryDetails(string dir)
        {
            try
            {
                FtpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                var respStream = FtpWebRequest.GetResponse().GetResponseStream();

                return UnixFtpStreamParse(respStream);
            }
            catch (Exception ex)
            {
                ConsoleSync.WriteLineT(ex.ExceptionMessageAgg());
                return Enumerable.Empty<FtpFileInfo>();
            }
        }

        private IEnumerable<FtpFileInfo> UnixFtpStreamParse(Stream responseStream)
        {
            var lst = new List<FtpFileInfo>();
            try
            {
                var reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fi = ParseFtpRequestLine(line);
                    if (fi != null)
                        lst.Add(fi);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ExceptionMessageAgg());
                return null;
            }
            return lst;
        }
        private FtpFileInfo ParseFtpRequestLine(string line)
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
                    IsDirectory = permissions.Substring(0, 1) == "d"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ExceptionMessageAgg());
                return null;
            }
        }

    }
}