using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class Transaction : EntityBase
    {
        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public TransactionResponseCache TransactionResponseCache { get; set; }
    }
}
