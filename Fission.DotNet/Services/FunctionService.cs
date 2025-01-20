using System;
using Fission.DotNet.Common;
using Fission.DotNet.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Fission.DotNet.Adapter;

namespace Fission.DotNet.Services;

public class FunctionService : IFunctionService
{
    private readonly IFunctionStoreService _functionStoreService;
    private readonly ILogger<FunctionService> _logger;

    public FunctionService(IFunctionStoreService functionStoreService, ILogger<FunctionService> logger)
    {
        this._functionStoreService = functionStoreService;
        this._logger = logger;
    }
    public Task<object> Run(FissionContext context)
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

        return ExecuteAndUnload(function.Assembly, function.Namespace, function.FunctionName, context);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private async Task<object> ExecuteAndUnload(string assemblyPath, string nameSpace, string classFunctionName, FissionContext context)
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

        if (File.Exists($"/function/Microsoft.Extensions.DependencyInjection.Abstractions.dll"))
        {
            File.Delete($"/function/Microsoft.Extensions.DependencyInjection.Abstractions.dll");
        }

        WeakReference alcWeakRef = null;

        var alc = new CustomAssemblyLoadContext($"/function/{assemblyPath}", isCollectible: true);
        try
        {
            var assemblyFunction = alc.LoadFromAssemblyPath($"/function/{assemblyPath}");

            alcWeakRef = new WeakReference(alc, trackResurrection: true);

            /*// Ottieni tutte le classi nell'assembly
            Type[] types = assemblyFunction.GetTypes();
            _logger.LogInformation("Elenco delle classi nell'assembly:");
            foreach (var type1 in types)
            {
                _logger.LogInformation(type1.FullName);
            }*/

            _logger.LogInformation($"Class try found: {nameSpace}.{classFunctionName}");

            var classFunctionNameType = assemblyFunction.GetType($"{nameSpace}.{classFunctionName}");

            if (classFunctionNameType != null)
            {
                MethodInfo configureServicesMethod = classFunctionNameType.GetMethod("ConfigureServices", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                ServiceCollection serviceCollection = null;
                ServiceProvider serviceProvider = null;
                if (configureServicesMethod != null)
                {
                    serviceCollection = new ServiceCollection();
                    configureServicesMethod.Invoke(null, new object[] { serviceCollection });
                    serviceCollection.AddTransient(classFunctionNameType);
                    serviceProvider = serviceCollection.BuildServiceProvider();
                }

                // Method to execute
                var executeMethod = classFunctionNameType.GetMethod("Execute");

                if (executeMethod != null)
                {
                    // Create an instance of the object
                    object classInstance = null;

                    if (serviceProvider != null)
                    {
                        classInstance = serviceProvider.GetService(classFunctionNameType);
                    }
                    else
                    {
                        classInstance = Activator.CreateInstance(classFunctionNameType);
                    }

                    if (classInstance == null)
                    {
                        throw new Exception("Instance not created.");
                    }

                    var executeMethodParameters = new object[] { context };

                    _logger.LogDebug($"Executing {classFunctionNameType.FullName}.{executeMethod.Name}");

                    var parameters = executeMethod.GetParameters();
                    if (parameters.Length > 1)
                    {
                        _logger.LogDebug($"Method {executeMethod.Name} has more than one parameter.");
                        if (parameters[1].ParameterType == typeof(Common.ILogger))
                        {
                            executeMethodParameters = new object[] { context, new FissionLoggerAdapter(_logger) };
                        }
                    }

                    _logger.LogDebug($"Method {executeMethod.Name} has {parameters.Length} parameters.");

                    // Execute the method
                    var result = executeMethod.Invoke(classInstance, executeMethodParameters);

                    if (result is Task task)
                    {
                        _logger.LogInformation("Task found.");
                        await task.ConfigureAwait(false);
                        var taskType = task.GetType();
                        if (taskType.IsGenericType)
                        {
                            return taskType.GetProperty("Result").GetValue(task);
                        }
                        return null;
                    }
                    else
                    {
                        _logger.LogInformation("Task not found.");
                    }

                    return result;
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

            for (int i = 0; alcWeakRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}

