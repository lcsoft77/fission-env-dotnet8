using Fission.DotNet.Common;

namespace TestExternalLibrary;

public class Class1
{
    public Task<string> Execute(FissionContext input)
    {
        return Task.FromResult($"Hello from external library!");
    }
}
