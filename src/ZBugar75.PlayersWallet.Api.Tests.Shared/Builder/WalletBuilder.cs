using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

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
    }
}
