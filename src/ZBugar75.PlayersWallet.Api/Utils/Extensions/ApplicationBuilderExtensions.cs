using Microsoft.AspNetCore.Builder;
using Zbugar75.PlayersWallet.Api.Middleware;

namespace Zbugar75.PlayersWallet.Api.Utils.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseErrorLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
