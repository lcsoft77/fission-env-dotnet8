using System;
using Fission.DotNet.Common;
using Fission.DotNet.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Fission.DotNet.Services;

public class FunctionService : IFunctionService
{
    private readonly IFunctionStoreService _functionStoreService;

    public FunctionService(IFunctionStoreService functionStoreService)
    {
        this._functionStoreService = functionStoreService;
    }
    public object Run(FissionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var function = this._functionStoreService.GetFunction();

        if (function == null)
        {
            throw new Exception("Function not specialized.");
        }

        WeakReference testAlcWeakRef = null;
        try
        {
            return ExecuteAndUnload(function.Assembly, function.Namespace, function.FunctionName, context, out testAlcWeakRef);
        }
        finally
        {
            if (testAlcWeakRef != null)
            {
                UnloadWeakReference(testAlcWeakRef);
            }
        }
    }

    private void UnloadWeakReference(WeakReference testAlcWeakRef)
    {
        for (int i = 0; testAlcWeakRef.IsAlive && (i < 10); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static object ExecuteAndUnload(string assemblyPath, string nameSpace, string functionname, FissionContext context, out WeakReference alcWeakRef)
    {
        if (!System.IO.File.Exists($"/function/{assemblyPath}"))
        {
            throw new Exception($"File /function/{assemblyPath} not found.");
        }

        // Delete the common library if it exists. This is to ensure that the latest version is always loaded.
        if (File.Exists($"/function/Fission.DotNet.Common.dll"))
        {
            File.Delete($"/function/Fission.DotNet.Common.dll");
        }

        var alc = new CustomAssemblyLoadContext($"/function/{assemblyPath}", isCollectible: true);
        try
        {
            var assemblyFunction = alc.LoadFromAssemblyPath($"/function/{assemblyPath}");

            alcWeakRef = new WeakReference(alc, trackResurrection: true);

            var type = assemblyFunction.GetType($"{nameSpace}.{functionname}");

            if (type != null)
            {
                // Method to execute
                var method = type.GetMethod("Execute");

                if (method != null)
                {
                    // Create an instance of the object, if necessary
                    var classInstance = Activator.CreateInstance(type);

                    // Method parameters, if required
                    var parameters = new object[] { context };

                    // Execute the method
                    return method.Invoke(classInstance, parameters);
                }
                else
                {
                    throw new Exception("Method not found.");
                }
            }
            else
            {
                throw new Exception("Type not found.");
            }
        }
        finally
        {
            alc.Unload();
        }
    }
}

