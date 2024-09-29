using System;
using Fission.DotNet.Common;

namespace TestDockerNet8.Interfaces;

public interface IFunctionService
{
    object Run(FissionContext context);
}
