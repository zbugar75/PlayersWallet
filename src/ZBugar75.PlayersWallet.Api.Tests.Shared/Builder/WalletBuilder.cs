using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Builder
{
    public class WalletBuilder
    {
        private Guid _playerId;
        private decimal _balance;

        public WalletBuilder()
        {
            _playerId = Guid.NewGuid();
            _balance = 0;
        }

        public WalletBuilder WithPlayerId(Guid playerId)
        {
            _playerId = playerId;
            return this;
        }

        public WalletBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public Wallet Build()
        {
            return new Wallet
            {
                PlayerId = _playerId,
                Balance = _balance
            };
        }

        public async Task<Wallet> CreateAsync(IPlayersWalletContext dbContext, CancellationToken cancellationToken)
        {
            var entity = Build();

            await dbContext.Wallets.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }
    }
}
