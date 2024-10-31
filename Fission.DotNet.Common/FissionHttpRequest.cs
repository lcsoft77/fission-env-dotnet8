using System.Text;

namespace Fission.DotNet.Common;

public class FissionHttpRequest
{
    public FissionHttpRequest(Stream body, string method, string url, Dictionary<string, string> headers)
    {
        Body = body;
        Method = method;
        Url = url;
        Headers = headers;
    }

    public Stream Body { get; private set; }

    public async Task<string> BodyAsString()
    {
        if (Body == null)
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(Body, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }        
    }

    public Dictionary<string, string> Headers { get; private set; }
    public string Url { get; private set; }
    public string Method { get; private set; }
}