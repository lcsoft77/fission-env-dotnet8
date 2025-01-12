using System;
using System.Text;
using System.Text.Json;

namespace Fission.DotNet.Common;

public class FissionHttpContext : FissionContext
{
    public FissionHttpContext(Dictionary<string, object> args, FissionRequest request) : base(args, request)
    {
        
    }

    public Stream Body => Request.Body;

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

    public Dictionary<string, string> Headers => Request.Headers;
    public string Url { get {
        var urlHeader = GetHeaderValue("X-Fission-Full-Url");

        if (urlHeader != null)
        {
            if (urlHeader.Contains("?"))
            {
                urlHeader = urlHeader.Substring(0, urlHeader.IndexOf("?"));
            }
            
            return  urlHeader;
        }
        else
        {
            return "/";
        }
    } }
    public string Method => Request.Method;
    public string Host => Request.Headers.ContainsKey("X-Forwarded-Host") ? Request.Headers["X-Forwarded-Host"] : null;
    public int Port => Request.Headers.ContainsKey("X-Forwarded-Port") ? Int32.Parse(Request.Headers["X-Forwarded-Port"]) : 0;
    public string UserAgent => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"] : null;
    public string ContentType => Request.Headers.ContainsKey("Content-Type") ? Request.Headers["Content-Type"] : null;
    public Int32 ContentLength => Request.Headers.ContainsKey("Content-Length") ? Int32.Parse(Request.Headers["Content-Length"]) : 0;
    public IDictionary<string, string> Params
    {
        get
        {
            var result = new Dictionary<string, string>();
            foreach (var key in Request.Headers.Keys)
            {
                if (key.StartsWith("X-Fission-Param-"))
                {
                    result[key.Substring("X-Fission-Param-".Length)] = Request.Headers[key];
                }
            }

            return result;
        }
    }
}
