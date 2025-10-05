using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Assemblies
{
    public class AssemblyInfoList
    {
        public List<AssemblyInfo> Assemblies { get; set; }

        public AssemblyInfoList() 
        {
            Assemblies = new List<AssemblyInfo>();
        }

        public AssemblyInfo GetByKey(string key)
        {
            return Assemblies.FirstOrDefault(a => a.Key == key);
        }
        public AssemblyInfo GetByName(string name)
        {
            return Assemblies.FirstOrDefault(a => a.Name == name);
        }
        public AssemblyInfo GetByFullName(string fname)
        {
            return Assemblies.FirstOrDefault(a => a.FullName == fname);
        }

        public string GetFullNameByKey(string key)
        {
            var ai = GetByKey(key);
            
            return ai == null ? null : ai.FullName;
        }
        public IList<string> GetDependenciesByKey(string key)
        {
            var a = GetByKey(key);
            if (a == null)
                return null;
            return a.Dependencies.Count > 0 ? a.Dependencies : null;
        }

        public IList<string> GetDependenciesByName(string name)
        {
            var a = GetByName(name);
            if(a==null)
                return null;
            return a.Dependencies.Count > 0 ? a.Dependencies : null;
        }
        public IList<string> GetDependenciesByFullName(string fname)
        {
            var a = GetByFullName(fname);
            if (a == null)
                return null;
            return a.Dependencies.Count > 0 ? a.Dependencies : null;
        }

         public bool IsAllDependenciesLoaded(string key)
         {
             var asi = GetByKey(key);
             if(asi == null)
                 return false;
             return asi.Dependencies.Select(dk => GetByKey(dk)).All(a => GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(a.FullName));
         }
         public bool IsAllDependenciesLoaded(AssemblyInfo asi)
         {
             if (asi == null)
                 return false;
             return asi.Dependencies.Select(dk => GetByKey(dk)).All(a => GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(a.FullName));
         }

        public IList<string> GetDependenciesUnLoaded(AssemblyInfo ai)
        {
            if (ai == null)
                return null;
            var lst = (ai.Dependencies.Select(dkey => GetByKey(dkey))
                            .Where(ainfo => !GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(ainfo.FullName))
                            .Select(ainfo => ainfo.Key)).ToList();
            return lst;
        }
    }
    
    public class AssemblyInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public string Path { get; set; }

        public bool Enabled { get; set; }

        public List<string> Dependencies { get; set; }

        public string Key {
            get { return Name; }
        }

        public AssemblyInfo()
        {
            Dependencies = new List<string>();
        }
        //public IList<string> GetDependenciesUnLoaded()
        //{
        //   var lst = (Dependencies.Select(dkey => GetByKey(dkey))
        //                    .Where(ainfo => !GS.Assemblies.Assemblies.IsAssemblyFullNameLoaded(ainfo.FullName))
        //                    .Select(ainfo => ainfo.Key)).ToList();
        //    return lst;
        //}
       
    }

}
