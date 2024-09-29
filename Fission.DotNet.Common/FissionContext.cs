using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Fission.DotNet.Common;

public class FissionContext
{
    public string PackagePath { get; set; }
    public FissionContext(Dictionary<string, object> args, FissionHttpRequest request)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        //if (logger == null) throw new ArgumentNullException(nameof(logger));
        if (request == null) throw new ArgumentNullException(nameof(request));
        Arguments = args;
        //Logger = logger;
        Request = request;
    }

    public Dictionary<string, object> Arguments { get; private set; }

    public FissionHttpRequest Request { get; private set; }

    //public Logger Logger { get; private set; }

    public static FissionContext Build(HttpRequest request)
    {
        return new FissionContext(request.Query.ToDictionary(x => x.Key, x => (object)x.Value),
                                    new FissionHttpRequest(request));
    }
    //this is to support aditional setting file read from source/deployment package via function
    public T GetSettings<T>(string relativePath)
    {
        var filePath = Path.Combine(this.PackagePath, relativePath);
        Console.WriteLine($"Going to Get Setting from :{filePath}");
        string json = GetSettingsJson(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    private string GetSettingsJson(string relativePath)
    {
        return File.ReadAllText(Path.Combine(this.PackagePath, relativePath));
    }
}

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

    //public X509Certificate Certificate { get { return _request.ClientCertificate; } }

    public string Url { get { return _request.Path.ToString(); } }
    public string Method { get { return _request.Method; } }
}