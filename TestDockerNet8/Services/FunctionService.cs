using System;
using Fission.DotNet.Common;
using TestDockerNet8.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace TestDockerNet8.Services;

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
        var alc = new CustomAssemblyLoadContext();
        try
        {
            if (!System.IO.File.Exists($"/function/{assemblyPath}"))
            {
                throw new Exception($"File /function/{assemblyPath} not found.");
            }

            Assembly a = alc.LoadFromAssemblyPath($"/function/{assemblyPath}");

            alcWeakRef = new WeakReference(alc, trackResurrection: true);

            Type type = a.GetType($"{nameSpace}.{functionname}");

            if (type != null)
            {
                // Method to execute
                MethodInfo method = type.GetMethod("Execute");

                if (method != null)
                {
                    // Create an instance of the object, if necessary
                    object classInstance = Activator.CreateInstance(type);

                    // Method parameters, if required
                    object[] parameters = new object[] { context };

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

