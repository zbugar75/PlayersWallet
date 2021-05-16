using System;
using System.Threading;
using System.Threading.Tasks;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Builder
{
    public class PlayerBuilder
    {
        private Guid _id;
        private string _username;
        private decimal _balance;

        public PlayerBuilder()
        {
            _id = Guid.NewGuid();
            _username = $"username {_id}";
            _balance = 0;
        }

        public PlayerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PlayerBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public PlayerBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public Player Build()
        {
            var wallet = new WalletBuilder()
                .WithBalance(_balance)
                .WithPlayerId(_id)
                .Build();

            return new Player
            {
                Id = _id,
                Username = _username,
                Wallet = wallet
            };
        }
        public async Task<Player> CreateAsync(IPlayersWalletContext dbContext, CancellationToken cancellationToken)
        {
            var entity = Build();

            await dbContext.Players.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }
    }
}
