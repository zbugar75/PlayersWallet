using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Dtos
{
    public class PlayerBalanceDto
    {
        public Guid PlayerId { get; set; }

        public decimal Balance { get; set; }

        public static PlayerBalanceDto Create(Wallet wallet) =>
            new PlayerBalanceDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance
            };
    }
}
