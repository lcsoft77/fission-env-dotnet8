using Fission.DotNet.Common;

namespace TestExternalLibrary;

public class Service: IService
{
    public async Task<string> Execute(FissionContext input)
    {
        await Task.Delay(500);
        return "Hello from external service!";
    }
}
