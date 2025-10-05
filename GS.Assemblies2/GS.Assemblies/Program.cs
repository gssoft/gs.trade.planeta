using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GS.Assemblies
{
    class Program
    {
        static void Main(string[] args)
        {
            Method1();
        }

        private static void Method1()
        {
            try
            {
                //create new domain
                AppDomain domain = AppDomain.CreateDomain("MyDomain");
                // domain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                //load dll into new domain
                var path = @"D:\VC\1305\gs.trade\GS\GS\bin\Release\GS.dll";
                //AssemblyName assemblyName =  
                //    new AssemblyName
                //    {
                //        CodeBase = @"D:\VC\1305\gs.trade\GS\GS\bin\Debug\GS.dll"
                //    };

                // Assembly assembly = domain.Load(assemblyName);
               // var fileinbytes = File.ReadAllBytes(path);
               // Assembly assembly = domain.Load(fileinbytes);

                var proxy = new ProxyDomain();
                var ass = proxy.GetAssembly(path, domain);

                var assemblies = domain.GetAssemblies();

                //do work with dll
                //...

                //unload dll
                AppDomain.Unload(domain);

                //still showing dll below ?????
                Assembly[] aAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                assemblies = domain.GetAssemblies();
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] tokens = args.Name.Split(",".ToCharArray());
            System.Diagnostics.Debug.WriteLine("Resolving : " + args.Name);
            return args.RequestingAssembly;

            // return Assembly.LoadFile(Path.Combine(new string[] {@"D:\VC\1305\gs.trade\GS\GS\bin\Debug", tokens[0] + ".dll"}));
        }
    }
    class ProxyDomain : MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath, AppDomain domain)
        {
            try
            {
                //return Assembly.LoadFrom(assemblyPath);
                return domain.Load(assemblyPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
