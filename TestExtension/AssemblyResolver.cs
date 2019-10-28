using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestExtension
{
    public class AssemblyResolver
    {
        private static Dictionary<string, AssemblyName> _assemblies = new Dictionary<string, AssemblyName>
        {
            //{
            //    "17863af14b0044da",
            //    new AssemblyName
            //    {
            //        Name = "Autofac",
            //        Version = new Version(4,9,4,0),
            //        CultureInfo = CultureInfo.InvariantCulture
            //    }
            //},
            //{
            //    "adb9793829ddae60",
            //    new AssemblyName
            //    {
            //        Name = "Microsoft.Extensions.Logging.Abstractions",
            //        Version = new Version(3,0,0,0),
            //        CultureInfo = CultureInfo.InvariantCulture
            //    }
            //},
            {
                "b03f5f7f11d50a3a",
                new AssemblyName
                {
                    Name = "System.Reflection.TypeExtensions",
                    Version = new Version(4,6,0,0),
                    CultureInfo = CultureInfo.InvariantCulture
                }
            }
        };

        private static void RedirectAssembly(string publicKeyToken, string shortName, Version targetVersion)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

        public static void Redirect()
        {
            foreach (KeyValuePair<string, AssemblyName> redirectAssembly in _assemblies)
            {
                RedirectAssembly(redirectAssembly.Key, redirectAssembly.Value.Name, redirectAssembly.Value.Version);
            }
        }
    }
}
