using Microsoft.Extensions.DependencyInjection;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations;

namespace Zbugar75.PlayersWallet.Api.Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterRepositories(
            this IServiceCollection services)
        {
            services
                .AddScoped<IPlayerRepository, PlayerRepository>()
                .AddScoped<IWalletRepository, WalletRepository>();

            return services;
        }
    }
}
