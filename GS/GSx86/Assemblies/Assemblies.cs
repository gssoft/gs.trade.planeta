using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GS.Assemblies
{
    public static class Assemblies
    {
        private const string Postfix 
            = @", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        public static bool IsAssemblyFullNameLoaded(string fullName)
        {
            var assLoaded = AppDomain.CurrentDomain.GetAssemblies();
            var assmbl = assLoaded.FirstOrDefault(a => a.FullName == fullName);
            return assmbl != null;
        }
        public static bool IsAssemblyShortNameLoaded(string shortName)
        {
            var fname = Path.GetFileNameWithoutExtension(shortName);
            var fullName = fname + Postfix;
            var assLoaded = AppDomain.CurrentDomain.GetAssemblies();
            var assmbl = assLoaded.FirstOrDefault(a => a.FullName == fullName);
            return assmbl != null;
        }

        public static IEnumerable<Assembly> GetAssembliesLoaded()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        public static Assembly GetAssemblyByShortName(string shortname)
        {
            var fullName = shortname + Postfix;
            var assLoaded = AppDomain.CurrentDomain.GetAssemblies();
            var assmbl = assLoaded.FirstOrDefault(a => a.FullName == fullName);
            return assmbl;
        }
        public static Assembly GetAssemblyByFullName(string fullName)
        {
            var assLoaded = AppDomain.CurrentDomain.GetAssemblies();
            var assmbl = assLoaded.FirstOrDefault(a => a.FullName == fullName);
            return assmbl;
        }

    }
}
