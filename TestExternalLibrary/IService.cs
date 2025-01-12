using Fission.DotNet.Common;

namespace TestExternalLibrary
{
    public interface IService
    {
        Task<string> Execute(FissionContext input, ILogger logger);
    }
}