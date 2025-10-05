using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GS.Xml
{
    public static class XDocExtensions
    {
        //private static readonly List<XDocument> XDocList = new List<XDocument>(); 
        //public static IEnumerable<XDocument> GetFiles(this XDocument xdoc, string dir, string files, bool first)
        //{
        //    if(first)
        //        XDocList.Clear();

        //    XElement xeDir;
        //    if ((xeDir = xdoc.Element(dir)) != null)
        //    {
        //        var xes = (from xe in xeDir.Elements("string") select xe).ToList();
        //        foreach (var xd in xes.Select(xe => XDocument.Load(xe.Value)))
        //        {
        //            xd.GetFiles(dir, files, false);
        //        }
        //    }
        //    else if (xdoc.Element(files) != null)
        //    {
        //        XDocList.Add(xdoc);
        //    }
            
        //    return XDocList;
        //}
        //public static IEnumerable<XDocument> GetFiles(this XDocument xdoc, 
        //                                                string node, string nodeElement, string fileElement, string dir,
        //                                                Func<string, string, XDocument> getFiles, bool first)
        //{
        //    if (first)
        //        XDocList.Clear();

        //    XElement xeDir;
        //    if ((xeDir = xdoc.Element(node)) != null)
        //    {
        //        var xes = (from xe in xeDir.Elements(nodeElement) select xe).ToList();
        //        foreach (var xe in xes)
        //        {
        //            var xd = getFiles(dir, xe.Value);
        //            if(xd != null)
        //                xd.GetFiles(node, nodeElement, fileElement, dir, getFiles, false);
        //        }
        //    }
        //    else if (xdoc.Element(fileElement) != null)
        //    {
        //        XDocList.Add(xdoc);
        //    }
        //    return XDocList;
        //}

        public static int GetFiles(this XDocument xdoc,
                                                       string node, string nodeElement, string fileElement, string dir,
                                                       Func<string, string, XDocument> getFiles, IList<XDocument> xDocs )
        {
            if (xDocs == null)
                return 0;
            XElement xeDir;
            if ((xeDir = xdoc.Element(node)) != null)
            {
                var xes = (from xe in xeDir.Elements(nodeElement) select xe).ToList();
                foreach (var xe in xes)
                {
                    var xd = getFiles(dir, xe.Value);
                    if (xd != null)
                        xd.GetFiles(node, nodeElement, fileElement, dir, getFiles, xDocs);
                }
            }
            else if (xdoc.Element(fileElement) != null)
            {
                xDocs.Add(xdoc);
            }
            return xDocs.Count;
        }
        public static IEnumerable<string> GetElementValues(this XDocument xdoc, string node, string nodeElement )
        {
            XElement xeDir;
            return (xeDir = xdoc.Element(node)) != null 
                    ? (from xe in xeDir.Elements(nodeElement) select xe.Value).ToList() 
                    : null;
        }
        /// <summary>
        /// Don't complete
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="node"></param>
        /// <param name="nodeElement"></param>
        /// <param name="dir"></param>
        /// <param name="getFiles"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        //public static IEnumerable<XDocument> GetFileNames(this XDocument xdoc,
        //                                                string node, string nodeElement, string dir,
        //                                                Func<string, string, XDocument> getFiles, IList<string> lst)
        //{
        //    XElement xeDir;
        //    if ((xeDir = xdoc.Element(node)) != null)
        //    {
        //        var xes = (from xe in xeDir.Elements(nodeElement) select xe).ToList();
        //        foreach (var xe in xes)
        //        {
        //            var xd = getFiles(dir, xe.Value);
        //            if (xd != null)
        //                xd.GetFileNames(node, nodeElement, dir, getFiles, lst);
        //        }
        //    }
        //    //else if (xdoc.Element(elements) != null)
        //    //{
        //    //    XDocList.Add(xdoc);
        //    //}
        //    return XDocList;
        //}

        public static XElement GetRoot(this XDocument xdoc, string rootName)
        {
            return xdoc.Element(rootName);
        }
        public static XElement GetNode(this XDocument xdoc, string nodeName)
        {
            return xdoc.Element(nodeName);
        }
        public static IEnumerable<string> GetTypeNameStrListEnumerable(string uri, string nodePath, string namesp)
        {
            var xdoc = XDocument.Load(uri);
            var node = xdoc.Element(nodePath);
            var nodeNamespace = node?.Attribute("ns")?.Value ?? namesp;
            if (node == null) yield break;
            if (!node.HasElements) yield break;

            foreach (var n in node.Elements())
            {
                // var ns = n.Attribute("ns")?.Value ?? namesp;
                var ns = n.Attribute("ns")?.Value ?? nodeNamespace;
                var str = ns + '.' + n.Name.ToString().Trim();
                yield return str;
            }
        }
        //Different Types, but interface is one
        public static IEnumerable<string> GetTypeNameStrListEnumerable(string uri, string nodePath, 
                                    string namesp, string assemblyName)
        {
            var xdoc = XDocument.Load(uri);
            var node = xdoc.Element(nodePath);
            if (node == null) yield break;
            if (!node.HasElements) yield break;

            var nodeNamespace = node.Attribute("ns")?.Value ?? namesp;
            var asmName = node.Attribute("as")?.Value ?? assemblyName;

            foreach (var n in node.Elements())
            {
                var ns = n.Attribute("ns")?.Value ?? nodeNamespace;
                var asm = n.Attribute("as")?.Value ?? asmName;
                var str = ns + '.' + n.Name.ToString().Trim() + ", " + asm.Trim();
                yield return str;
            }
        }
        public static IEnumerable<T> DeSerialize<T>(string uri, string nodePath, string namesp, string assemblyName)
            where T : class
        {
            var xdoc = XDocument.Load(uri);
            var node = xdoc.Element(nodePath);
            if (node == null) yield break;
            if (!node.HasElements) yield break;

            var nodeNamespace = node.Attribute("ns")?.Value ?? namesp;
            var asmName = node.Attribute("as")?.Value ?? assemblyName;

            foreach (var n in node.Elements())
            {
                var ns = n.Attribute("ns")?.Value ?? nodeNamespace;
                var asm = n.Attribute("as")?.Value ?? asmName;
                var typeStr = ns + '.' + n.Name.ToString().Trim() + ", " + asm.Trim();

                var ty = Type.GetType(typeStr, false, true);
                var s = Serialization.Do.DeSerialize(ty, n, null) as T;

                yield return s;
            }
        }
    }
}
