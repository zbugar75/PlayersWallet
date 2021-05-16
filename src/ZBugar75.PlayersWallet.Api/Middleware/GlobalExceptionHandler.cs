using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Utils.Extensions;

namespace Zbugar75.PlayersWallet.Api.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                using (BeginErrorContextScope(context))
                {
                    HandleExceptionAsync(context, ex);
                }
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, $"Global exception handler catched exception.");
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = this.GetStatusCodeFromException(exception);
        }

        private int GetStatusCodeFromException(Exception ex)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            if (ex != null)
            {
                if (ex is EntityNotFoundException)
                {
                    code = HttpStatusCode.NotFound;
                }
                else if (ex is DuplicateUsernameException)
                {
                    code = HttpStatusCode.Conflict;
                }
                else if (ex is ArgumentNullException)
                {
                    code = HttpStatusCode.BadRequest;
                }
            }

            return (int)code;
        }


        private IDisposable BeginErrorContextScope(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var headersDictionary = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var logPropertyTuples = new ValueTuple<string, object>[]
            {
                ("RequestHeadersJson", JsonConvert.SerializeObject(headersDictionary)),
                ("RequestHost", request.Host),
                ("RequestProtocol", request.Protocol)
            };

            return _logger.BeginPropertyScope(logPropertyTuples);
        }

    }
}
