using System;
using System.Net.Http.Json;
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
}
