using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;


namespace CA_LoadAssToDomain_03
{
    class Program
    {
        static void Main(string[] args)
        {
            // AssemblyLoadFromPath();
            AssembliesLoadFromDir();
        }

        private static void AssemblyLoadFromPath()
        {
            const string assemblyPath = @"D:\VC\1305\gs.trade\GS.Assemblies\TestAssembly\bin\Debug\TestAssembly.dll";
            var newDomain = AppDomain.CreateDomain("MyNewDomain");
            var asmLoaderProxy = (ProxyDomain)newDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProxyDomain).FullName);

            asmLoaderProxy.GetAssembly(assemblyPath);

            foreach (var a in asmLoaderProxy.GetAssembliesLoaded(newDomain))
            {
                Console.WriteLine(a);
            }
            Console.WriteLine(Environment.NewLine);

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}, Ass: {a}");
            }

            // ERROR
            //foreach (var a in asmLoaderProxy.GetAssembliesLoaded(AppDomain.CurrentDomain))
            //{
            //    Console.WriteLine(a);
            //}

            Console.ReadLine();
        }
        // This way, all the dlls will automaically be resolved from dllsSearchPath.
        private static void AssembliesLoadFromDir()
        {
            const string dllsSearchPath = @"D:\VC\1305\gs.trade\GS.Assemblies\CA_LoadAssToDomain_03\bin\Debug";
            const string assemblyPath = @"D:\VC\1305\gs.trade\GS.Assemblies\TestAssembly\bin\Debug\TestAssembly.dll";
            try
            {
                // var newDomain = AppDomain.CreateDomain("MyNewDomain");
                var newDomain = AppDomain
                    .CreateDomain("DomainWithAssFromDir", new Evidence(), dllsSearchPath, "", true);

              //  Console.ReadLine();
                AppDomain.CurrentDomain.AssemblyResolve += NewDomainOnAssemblyResolve;
                var asmLoaderProxy = (ProxyDomain)newDomain
                        .CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProxyDomain).FullName);

                newDomain.AssemblyLoad += NewDomainOnAssemblyLoad;

                asmLoaderProxy.GetAssembly(assemblyPath);

                foreach (var a in asmLoaderProxy.GetAssembliesLoaded(newDomain))
                {
                    Console.WriteLine(a);
                }
                Console.WriteLine(Environment.NewLine);

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Console.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}, Ass: {a}");
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }         
        }

        private static void NewDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var a = args.LoadedAssembly;
            Console.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}, Ass Loaded: {a}");
        }

        private static Assembly NewDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;
            var assToResolve = args.RequestingAssembly;
            System.Diagnostics.Debug.WriteLine($"Name: {name}, Ass: {assToResolve} ");
            return assToResolve;
        }

        class ProxyDomain : MarshalByRefObject
        {
            public void GetAssembly(string assemblyPath)
            {
                try
                {
                    Assembly.LoadFrom(assemblyPath);
                    //If you want to do anything further to that assembly, you need to do it here.
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }

            public IEnumerable<string> GetAssembliesLoaded(AppDomain domain)
            {
                try
                {
                    var domainname = domain.FriendlyName;
                    var ass = domain.GetAssemblies().Select(a => $"Domain: {domainname}, Ass: {a.ToString()}").ToArray();
                    return ass;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Enumerable.Empty<string>();
                }
            }
        }
    }
}
