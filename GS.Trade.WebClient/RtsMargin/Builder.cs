using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RtsMargin
{
    public class Builder
    {
        public static T Build<T>(string uri, string root) where T : class
        {
            return DeSerializationCollection<T>(uri, root);
        }

        private static T DeSerializationCollection<T>(string uri, string root) where T : class
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var x = xDoc.Descendants(root).FirstOrDefault();
                if (x == null)
                    return default(T);

             //   var typeName = GetType().Namespace + '.' + x.Name.ToString().Trim();
                var typeName = x.Name.ToString().Trim();
                var t = Type.GetType(typeName, false, true);
                return GS.Serialization.Do.DeSerialize<T>(x, null);

                //    _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");
                //  return false;

            }
            //return default(T);
        }
    }
}
