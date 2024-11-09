using System;
using Fission.DotNet.Common;

namespace Fission.DotNet.Interfaces;

public interface IFunctionService
{
    Task<object> Run(FissionContext context);
}
