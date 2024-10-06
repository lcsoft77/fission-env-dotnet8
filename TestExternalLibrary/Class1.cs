using System.Text.Json;
using Fission.DotNet.Common;

namespace TestExternalLibrary;

public class Class1
{
    public string Execute(FissionContext input)
    {
        //var json = JsonSerializer.Serialize(input);

        return $"Hello from external library! - 1 - {input.Request.Method} {input.Request.Url}";
    }
}
