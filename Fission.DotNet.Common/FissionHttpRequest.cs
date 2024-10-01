using System.Text;
using Microsoft.AspNetCore.Http;


namespace Fission.DotNet.Common;

public class FissionHttpRequest
{
    private readonly HttpRequest _request;
    internal FissionHttpRequest(HttpRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        _request = request;
    }

    public Stream Body { get { return _request.Body; } }

    public string BodyAsString()
    {
        int length = (int)_request.Body.Length;
        byte[] data = new byte[length];
        _request.Body.Read(data, 0, length);
        return Encoding.UTF8.GetString(data);
    }

    public Dictionary<string, IEnumerable<string>> Headers
    {
        get
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            foreach (var kv in _request.Headers)
            {
                headers.Add(kv.Key, kv.Value);
            }
            return headers;
        }
    }

    public string Url { get { return _request.Path.ToString(); } }
    public string Method { get { return _request.Method; } }
}