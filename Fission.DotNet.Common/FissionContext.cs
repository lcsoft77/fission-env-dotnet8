namespace Fission.DotNet.Common;

public class FissionContext
{
    public FissionContext(Dictionary<string, object> args, FissionHttpRequest request)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        if (request == null) throw new ArgumentNullException(nameof(request));
        Arguments = args;
        Request = request;

        TraceID = GetHeaderValue("traceparent", Guid.NewGuid().ToString());
        FunctionName = GetHeaderValue("X-Fission-Function-Name");
        Namespace = GetHeaderValue("X-Fission-Function-Namespace");
        ResourceVersion = GetHeaderValue("X-Fission-Function-Resourceversion");
        UID = GetHeaderValue("X-Fission-Function-Uid");
        Trigger = GetHeaderValue("Source-Name");
        ContentType = GetHeaderValue("Content-Type");
        var strContentLength = GetHeaderValue("Content-Length");
        if (strContentLength != null)
        {
            try
            {
                ContentLength = Int32.Parse(strContentLength);
            }
            catch (FormatException)
            {
                ContentLength = 0;
            }
        }
    }

    protected string GetHeaderValue(string key, string defaultValue = null)
    {
        return Request.Headers.ContainsKey(key) ? Request.Headers[key] : defaultValue;
    }

    public Dictionary<string, object> Arguments { get; private set; }

    public FissionHttpRequest Request { get; private set; }
    public string TraceID { get; }
    public string FunctionName { get; }
    public string Namespace { get; }
    public string ResourceVersion { get; }
    public string UID { get; }
    public string Trigger { get; }
    public string ContentType { get; }
    public Int32 ContentLength { get; }
}
