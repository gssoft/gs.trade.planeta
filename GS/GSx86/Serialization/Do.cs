using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GS.Serialization
{
    public class Do
    {
        public static T DeSerialize<T>(XElement xe, Action<string, string> errMessage) where T : class
        {
            if (xe == null)
            {
                if (errMessage != null) errMessage("DeSerialization Failure:", "XElement is Null Refferenced");
                return null;
            }
            var x = default(T);
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(typeof(T)); // DeSerializer
                    x = xds.Deserialize(sr) as T;
                }
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("DeSerialization Exception:", e.Message);
            }
            if (x == null)
                if (errMessage != null) errMessage("DeSerialization Failure: ", xe.ToString());

            return x;
        }
        public static T DeSerialize<T>(Type t, XElement xe, Action<string, string> errMessage) where T : class
        {
            if (xe == null || t == null)
            {
                if (errMessage != null) errMessage("Type is Null Refferenced. Or ", "XElement is Null Refferenced");
                return null;
            }
            var x = default(T);
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(t); // DeSerializer
                    var obj = xds.Deserialize(sr);
                    x = obj as T;
                }
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("DeSerialization Exception:", e.Message);
            }
            if (x == null)
                if (errMessage != null) errMessage("DeSerialization Failure: ", xe.ToString());

            return x;
        }

        public static T DeSerialize<T>(Type t, XElement xe, string ns, Action<string, string> errMessage) where T : class
        {
            if (xe == null || t == null)
            {
                if (errMessage != null) errMessage("Type is Null Refferenced. Or ", "XElement is Null Refferenced");
                return null;
            }
            var x = default(T);
            //var bo = Assemblies.Assemblies.IsAssemblyShortNameLoaded("GS.Trade.Data");
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(t, ns); // DeSerializer
                    var obj = xds.Deserialize(sr);
                    x = obj as T;
                }
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("DeSerialization Exception:", e.Message);
            }
            if (x == null)
                if (errMessage != null) errMessage("DeSerialization Failure: ", xe.ToString());

            return x;
        }

        public static object DeSerialize(Type t, XElement xe,  Action<string, string> errMessage)
        {
            if (xe == null || t == null)
            {
                if (errMessage != null) errMessage("Type is Null Refferenced. Or ", "XElement is Null Refferenced");
                return null;
            }
            object x = null;
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(t); // DeSerializer
                    x = xds.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("DeSerialization Exception:", e.Message);
            }
            if (x == null)
                if (errMessage != null) errMessage("DeSerialization Failure: ", xe.ToString());

            return x;
        }
        public static T DeSerialize2<T>(XElement xe, Action<string, string> errMessage) where T : class
        {
            if (xe == null)
            {
                if (errMessage != null) errMessage("DeSerialization Failure:", "XElement is Null Refferenced");
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
            Type t = null;
            try
            {
                var asm = Assembly.LoadFrom(path);
                t = asm.GetTypes().FirstOrDefault(s => s.IsClass && s.FullName == name);
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("Refection Exception:", e.Message);
            }
            if (t == null)
            {
                if (errMessage != null) errMessage("Reflection Failure:", path + " " + name);
                return null;
            }
            object x = null;
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(t); // DeSerializer
                    x = xds.Deserialize(sr) as T;
                }
            }
            catch (Exception e)
            {
                if (errMessage != null) errMessage("DeSerialization Exception:", e.Message);
            }
            if (x == null)
                if (errMessage != null) errMessage("DeSerialization Failure: ", xe.ToString());

            return x as T;
        }
        public static bool Serialize<T>(string xmlFileName, T t)
        {
            TextWriter tr = null;
            try
            {
                tr = new StreamWriter(xmlFileName);
                var sr = new XmlSerializer(typeof (T));
                sr.Serialize(tr, t);
                tr.Close();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Serialization Exception. Type: {typeof(T)}, FileName: {xmlFileName}, Mess: {e.Message}");
            }
            finally
            {
                tr?.Close();
            }
        }
    }
}
