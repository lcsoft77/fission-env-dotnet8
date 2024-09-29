using System.Reflection;
using System.Runtime.Loader;

namespace Fission.DotNet.Services
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base(isCollectible: true) { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null; // Manual loading required
        }
    }
}