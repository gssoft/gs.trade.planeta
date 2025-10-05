using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ca_LoadAssToDomain_02
{
    class Program
    {
        static string _assemblyDirectory;
        static void Main(string[] args)
        {
            LoadAssembly();
        }

        private static void LoadAssembly()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var path = @"D:\VC\1305\gs.trade\GS.Assemblies\TestAssembly\bin\Release\TestAssembly.dll";
            var filename = @"TestAssembly";

            _assemblyDirectory = Path.GetDirectoryName(path);
            Assembly loadedAssembly = Assembly.LoadFile(path);
            // Assembly loadedAssembly = Assembly.Load(filename);

            List<Type> assemblyTypes = loadedAssembly.GetTypes().ToList<Type>();
            var assms = AppDomain.CurrentDomain.GetAssemblies();

            Console.WriteLine("Assemblies:" + Environment.NewLine);
            foreach(var a in assms)
                Console.WriteLine(a + Environment.NewLine);

            Console.ReadLine();

            //foreach (var type in assemblyTypes)
            //{
            //    if (type.IsInterface == false)
            //    {
            //        StreamWriter jsonFile = File.CreateText(string.Format(@"c:\Provisioning\{0}.json", type.Name));
            //        JavaScriptSerializer serializer = new JavaScriptSerializer();
            //        jsonFile.WriteLine(serializer.Serialize(Activator.CreateInstance(type)));
            //        jsonFile.Close();
            //    }
            //}
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] tokens = args.Name.Split(",".ToCharArray());
            System.Diagnostics.Debug.WriteLine("Resolving : " + args.Name);
            var filepath = Path.Combine(new string[] {_assemblyDirectory, tokens[0] + ".dll"});
            return Assembly.LoadFile(filepath);
        }
    }
}
