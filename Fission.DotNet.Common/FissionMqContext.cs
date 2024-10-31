using System;

namespace Fission.DotNet.Common;

public class FissionMqContext : FissionContext
{
    public FissionMqContext(Dictionary<string, object> args, FissionHttpRequest request) : base(args, request)
    {
        Topic = GetHeaderValue("Topic");
        ErrorTopic = GetHeaderValue("Errortopic");
        ResponseTopic = GetHeaderValue("Resptopic");
    }

    public string Topic { get; }
    public string ErrorTopic { get; }
    public string ResponseTopic { get; }
}
