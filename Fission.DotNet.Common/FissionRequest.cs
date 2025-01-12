using System.Text;

namespace Fission.DotNet.Common;

public class FissionRequest
{
    public FissionRequest(Stream body, string method, Dictionary<string, string> headers)
    {
        Body = body;
        Method = method;
        Headers = headers;

        var urlHeader = GetHeaderValue(headers, "X-Fission-Full-Url");

        if (urlHeader != null)
        {
            //extract the path from the full url
            var uri = new Uri(urlHeader);
            Url = uri.AbsolutePath;
        }
        else
        {
            Url = "/";
        }
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

    private string GetHeaderValue(Dictionary<string, string> headers, string key, string defaultValue = null)
    {
        return headers.ContainsKey(key) ? headers[key] : defaultValue;
    }
}