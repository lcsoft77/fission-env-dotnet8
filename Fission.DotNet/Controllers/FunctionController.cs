using System.Text;
using Fission.DotNet.Common;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Fission.DotNet.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace Fission.DotNet.Controllers
{
    [ApiController]
    [Route("/")]
    public class FunctionController : Controller
    {
        private readonly ILogger<FunctionController> _logger;
        private readonly IFunctionService _functionService;

        public FunctionController(ILogger<FunctionController> logger, IFunctionService functionService)
        {
            this._logger = logger;
            this._functionService = functionService;
        }
        /// <summary>
        ///     Handle HTTP GET requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpGet]
        public async Task<IActionResult> Get() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP POST requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpPost]
        public async Task<IActionResult> Post() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP PUT requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpPut]
        public async Task<IActionResult> Put() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP HEAD requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpHead]
        public async Task<IActionResult> Head() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP OPTIONS requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpOptions]
        public async Task<IActionResult> Options() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP DELETE requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete() => await this.Run(Request);

        /// <summary>
        ///     Invokes the Fission function on behalf of the caller.
        /// </summary>
        /// <returns>
        ///     200 OK with the Fission function return value; or 400 Bad Request with the exception message if an exception
        ///     occurred in the Fission function; or 500 Internal Server Error if the environment container has not yet been
        ///     specialized.
        /// </returns>
        private async Task<IActionResult> Run(HttpRequest request)
        {
            _logger.LogInformation("FunctionController.Run");

            // Copy the body to a MemoryStream so it can be read again
            request.EnableBuffering();
            using (var memoryStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                /*using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    _logger.LogInformation($"Body: {await reader.ReadToEndAsync()}");
                }

                memoryStream.Seek(0, SeekOrigin.Begin);*/
                request.Body = memoryStream;


                Fission.DotNet.Common.FissionContext context = null;

                try
                {
                    var httpArgs = request.Query.ToDictionary(x => x.Key, x => (object)x.Value);
                    var headers = request.Headers.ToDictionary(x => x.Key, x => (string)x.Value);
                    var parameters = new Dictionary<string, string>();

                    // Extract headers that start with "X-Fission-Params-"
                    foreach (var header in request.Headers)
                    {
                        if (header.Key.StartsWith("X-Fission-Params-"))
                        {
                            var key = header.Key.Substring("X-Fission-Params-".Length);
                            var value = header.Value.ToString();
                            parameters[key] = value;
                        }
                    }

                    if (request.Headers.ContainsKey("Topic"))
                    {
                        _logger.LogInformation("FunctionController.Run: FissionMqContext");
                        context = new Fission.DotNet.Common.FissionMqContext(request.Body, httpArgs, headers, parameters);
                    }
                    else
                    {
                        if (request.Headers.ContainsKey("X-Forwarded-Proto"))
                        {
                            _logger.LogInformation("FunctionController.Run: FissionHttpContext");
                            context = new Fission.DotNet.Common.FissionHttpContext(request.Body, request.Method, httpArgs, headers, parameters);
                            /*context = new Fission.DotNet.Common.FissionHttpContext(httpArgs,
                                new Fission.DotNet.Common.FissionRequest(request.Body, request.Method, headers));*/
                        }
                        else
                        {
                            _logger.LogInformation("FunctionController.Run: FissionContext");
                            context = new Fission.DotNet.Common.FissionContext(request.Body, httpArgs, headers, parameters);
                            /*context = new Fission.DotNet.Common.FissionContext(httpArgs,
                                new Fission.DotNet.Common.FissionRequest(request.Body, request.Method, headers));*/
                        }
                    }

                    if (context is FissionHttpContext)
                    {
                        _logger.LogInformation($"Body stream: {context.Content.Position}-{context.Content.Length}");
                        //var body = await (context as FissionHttpContext).ContentAsString();

                        _logger.LogInformation($"Request Method: {(context as FissionHttpContext).Method}");
                        //_logger.LogInformation($"Request Body: {body}");
                        _logger.LogInformation($"Request Arguments: {string.Join(", ", (context as FissionHttpContext).Arguments.Select(x => $"{x.Key}={x.Value}"))}");
                    }
                    _logger.LogInformation($"Request Method: {request.Method}");
                    _logger.LogInformation($"Request Headers: {string.Join(", ", request.Headers.Select(x => $"{x.Key}={x.Value}"))}");
                    _logger.LogInformation($"Request Query: {string.Join(", ", request.Query.Select(x => $"{x.Key}={x.Value}"))}");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FunctionController.Run");
                    return BadRequest(ex.Message);
                }

                try
                {
                    return Ok(await _functionService.Run(context));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FunctionController.Run");
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}

