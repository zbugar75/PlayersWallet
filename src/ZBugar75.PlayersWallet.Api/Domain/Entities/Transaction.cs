using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class Transaction : EntityBase
    {
        public Guid PlayerId { get; set; }

        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public TransactionResponse TransactionResponse { get; set; }
    }
}
