using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zbugar75.PlayersWallet.Api.Common.Exceptions;
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
            if (exception is EntityNotFoundException || exception is DuplicateEntityException || exception is ArgumentNullException)
                _logger.LogInformation("Global exception handler catched exception: {message}", exception.Message);
            else
                _logger.LogError(exception, "Global exception handler catched exception.");

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = GetStatusCodeFromException(exception);
        }

        private static int GetStatusCodeFromException(Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            if (exception != null)
            {
                if (exception is EntityNotFoundException)
                {
                    code = HttpStatusCode.NotFound;
                }
                else if (exception is DuplicateEntityException)
                {
                    code = HttpStatusCode.Conflict;
                }
                else if (exception is ArgumentNullException)
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
