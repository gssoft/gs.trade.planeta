using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoadAssToDomain_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Method1();
        }
        private static void Method1()
        {
            //create new domain
            AppDomain domain = AppDomain.CreateDomain("MyDomain");
            try
            {
                // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                domain.AssemblyResolve += MyDomain_AssemblyResolve;
                domain.AssemblyLoad += MyDomain_AssemblyLoad;
                // domain.BaseDirectory = @"D:\VC\1305\gs.trade\GS.Assemblies\TestAssembly\bin\Debug";

                //load dll into new domain
                var path = @"D:\VC\1305\gs.trade\GS.Assemblies\TestAssembly\bin\Release\TestAssembly.dll";
                // var path = @"TestAssembly.dll"; // this for Assembly in Bin Directory

                AssemblyName assemblyName = new AssemblyName();
                // assemblyName.CodeBase = "c:\\mycode.dll";
                assemblyName.CodeBase = path;

                Assembly assembly = domain.Load(assemblyName);

                Console.WriteLine($"Domain: {domain.FriendlyName},\r\nBaseDir: {domain.BaseDirectory},\r\nAssemblies:");

                Assembly[] domainAssemblies = domain.GetAssemblies();
                foreach (var a in domainAssemblies)
                    Console.WriteLine(a.FullName + Environment.NewLine);
                Console.ReadLine();

                AppDomain.Unload(domain);

                //still showing dll below ?????
                Console.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}, Assemblies: ");
                Assembly[] aAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach(var a in aAssemblies)
                    Console.WriteLine(a.FullName + Environment.NewLine);
                Console.ReadLine();

            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                AppDomain.Unload(domain);
            }
            catch (FileLoadException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                AppDomain.Unload(domain);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                AppDomain.Unload(domain);
            }
        }
        private static Assembly MyDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] tokens = args.Name.Split(",".ToCharArray());
            System.Diagnostics.Debug.WriteLine("Resolving : " + args.Name);
            // return args.RequestingAssembly;
            return Assembly.LoadFile(Path.Combine(new string[] { "c:\\", tokens[0] + ".dll" }));
        }
        private static void MyDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs assemblyLoadEventArgs)
        {
            var a = assemblyLoadEventArgs.LoadedAssembly;
            System.Diagnostics.Debug.WriteLine($"Loading : {a}");
        }

    }
}
