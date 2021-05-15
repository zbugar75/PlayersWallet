using System;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class Wallet
    {
        public Wallet()
        {
        }

        public Wallet(decimal balance)
        {
            Balance = balance;
        }

        public Guid PlayerId { get; set; }

        public decimal Balance { get; set; }
    }
}
