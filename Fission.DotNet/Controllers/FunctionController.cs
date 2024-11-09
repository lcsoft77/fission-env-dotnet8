using System.Text;
using Fission.DotNet.Common;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Fission.DotNet.Interfaces;

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

            Fission.DotNet.Common.FissionContext context = null;

            var httpArgs = request.Query.ToDictionary(x => x.Key, x => (object)x.Value);
            var headers = request.Headers.ToDictionary(x => x.Key, x => (string)x.Value);

            if (request.Headers.ContainsKey("Topic"))
            {
                context = new Fission.DotNet.Common.FissionMqContext(httpArgs,
                    new Fission.DotNet.Common.FissionHttpRequest(request.Body, request.Method, request.Path, headers));
            }
            else
            {
                context = new Fission.DotNet.Common.FissionContext(httpArgs,
                    new Fission.DotNet.Common.FissionHttpRequest(request.Body, request.Method, request.Path, headers));
            }

            try
            {
                return Ok(await _functionService.Run(context));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FunctionController.Run");
                return BadRequest( ex.Message);
            }
        }
    }
}
