using System;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class Wallet
    {
        public Wallet()
        {
            Balance = 0;
        }

        public Guid PlayerId { get; set; }

        public decimal Balance { get; set; }
    }
}
