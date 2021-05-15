using System.Collections.Generic;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class Player : EntityBase
    {
        public string Username { get; set; }

        public Wallet Wallet { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
