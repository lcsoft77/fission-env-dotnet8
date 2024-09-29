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
        public async Task<object> Get() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP POST requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpPost]
        public async Task<object> Post() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP PUT requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpPut]
        public async Task<object> Put() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP HEAD requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpHead]
        public async Task<object> Head() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP OPTIONS requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpOptions]
        public async Task<object> Options() => await this.Run(Request);

        /// <summary>
        ///     Handle HTTP DELETE requests by forwarding to the Fission function.
        /// </summary>
        /// <returns>The function return value.</returns>
        [HttpDelete]
        public async Task<object> Delete() => await this.Run(Request);

        /// <summary>
        ///     Invokes the Fission function on behalf of the caller.
        /// </summary>
        /// <returns>
        ///     200 OK with the Fission function return value; or 400 Bad Request with the exception message if an exception
        ///     occurred in the Fission function; or 500 Internal Server Error if the environment container has not yet been
        ///     specialized.
        /// </returns>
        private async Task<object> Run(HttpRequest request)
        {
            var requestTask = Task.Run(() =>
            {
                _logger.LogInformation("FunctionController.Run");

                var context = FissionContext.Build(request);

                return _functionService.Run(context);
            });

            return await requestTask;
        }
    }
}
