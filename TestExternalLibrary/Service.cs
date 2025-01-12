using Fission.DotNet.Common;

namespace TestExternalLibrary;

public class Service: IService
{
    public async Task<string> Execute(FissionContext input, ILogger logger)
    {
        await Task.Delay(5);
        logger.LogInformation("Log from external service!");
        return "Hello from external service!";
    }
}
