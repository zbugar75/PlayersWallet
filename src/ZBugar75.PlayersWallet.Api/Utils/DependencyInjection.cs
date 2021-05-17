using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zbugar75.PlayersWallet.Api.Common.Configurations;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations;
using Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork;

namespace Zbugar75.PlayersWallet.Api.Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterRepositories(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IPlayerService, PlayerService>()
                .AddSingleton<ISimpleMemoryCache<TransactionResponse>, SimpleMemoryCache<TransactionResponse>>();

            services.UseConfigurationValidation();
            services.ConfigureValidatableSetting<SimpleMemoryCacheConfiguration>(configuration.GetSection("SimpleMemoryCache"));

            return services;
        }
    }
}
