using System;
using TestDockerNet8.Model;

namespace TestDockerNet8.Interfaces;

public interface IFunctionStoreService
{
    void SetFunction(FunctionStore function);
    FunctionStore GetFunction();
}
