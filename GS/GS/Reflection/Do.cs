using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace GS.Reflection
{
    public class Do
    {
        public static Type GetType(XElement xe, Action<string, string> errMessage)
        {
            if (xe == null)
            {
                if (errMessage != null) errMessage("GetType Failure:", "XElement is Null Refferenced");
                return null;
            }
            var a = xe.Attribute("Path");
            var path = a != null ? a.Value : string.Empty;
            a = xe.Attribute("FullName");
            var name = a != null ? a.Value : string.Empty;

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
            {
                if (errMessage != null) errMessage("Path or FullName is Invalid :", path + " " + name);
                return null;
            }
            return GetType(path, name, errMessage);
        }
        public static Type GetType(XElement xe, out string assemblyPath, Action<string, string> errMessage)
        {
            assemblyPath = string.Empty;
            if (xe == null)
            {
                if (errMessage != null) errMessage("GetType Failure:", "XElement is Null Refferenced");
                return null;
            }
            var a = xe.Attribute("Path");
            var path = a != null ? a.Value : string.Empty;
            a = xe.Attribute("FullName");
            var name = a != null ? a.Value : string.Empty;

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
            {
                if (errMessage != null) errMessage("Path or FullName is Invalid :", path + " " + name);
                return null;
            }
            assemblyPath = path;
            return GetType(path, name, errMessage);
        }

        public static Type GetType(string assemblyPath, string typeFullName, Action<string, string> errMessage)
        {
            Type t = null;
            try
            {
                var asm = Assembly.LoadFrom(assemblyPath);
                t = asm.GetTypes().Where(s => s.IsClass && s.FullName == typeFullName).FirstOrDefault();
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("Refection Exception:", e.Message);
            }
            if (t == null)
            {
                if (errMessage != null) errMessage("Reflection Failure:", assemblyPath + " " + typeFullName);
            }
            return t;
        }
        public static string GetTypeKey(XElement xe)
        {
            if (xe == null)
            {
                return string.Empty;
            }
            var a = xe.Attribute("Path");
            var path = a != null ? a.Value : string.Empty;
            a = xe.Attribute("FullName");
            var name = a != null ? a.Value : string.Empty;

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return path.Trim() + name.Trim();
        }

        /*
        public static Type GetType(XElement xe, object o, Action<string,string> errMessage)
        
        {
            Type t = null;
            if (xe == null)
            {
                if (errMessage != null) errMessage("GetType Failure:", "XElement is Null Refferenced");
                return null;
            }
           // var name = xe.Name;
            if (o != null)
            {
                var ty = o.GetType();
                if (ty.FullName != null)
                {
                    var typefullname = ty.FullName.Substring(0, ty.FullName.LastIndexOf('.') + 1) + xe.Name;
                    t = Type.GetType(typefullname);
                }
            }
            else
            {
                t = Type.GetType(xe.Name.ToString(),false,true);
            }
            return t;
        }
         */ 
    }
}
