using System;
using Fission.DotNet.Common;

namespace Fission.DotNet.Interfaces;

public interface IFunctionService
{
    object Run(FissionContext context);
}
