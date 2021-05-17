using Microsoft.Extensions.DependencyInjection;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations;
using Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork;

namespace Zbugar75.PlayersWallet.Api.Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterRepositories(
            this IServiceCollection services)
        {
            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IPlayerService, PlayerService>();

            return services;
        }
    }
}
