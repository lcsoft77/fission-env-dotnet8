using Fission.DotNet.Common;
using Microsoft.Extensions.DependencyInjection;

namespace TestExternalLibrary;

public class Class1
{
    private readonly IService service;

    public Class1(IService service)
    {
        this.service = service;
    }
    public async Task<string> Execute(FissionContext input, ILogger logger)
    {
        return await service.Execute(input, logger);
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IService, Service>();
    }
}
