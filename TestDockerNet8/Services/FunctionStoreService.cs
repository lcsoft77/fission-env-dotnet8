using System;
using TestDockerNet8.Interfaces;
using TestDockerNet8.Model;

namespace TestDockerNet8.Services;

public class FunctionStoreService : IFunctionStoreService
{
    private FunctionStore _function;
    public FunctionStore GetFunction()
    {
        return _function;
    }

    public void SetFunction(FunctionStore function)
    {
        _function = function;
    }
}
