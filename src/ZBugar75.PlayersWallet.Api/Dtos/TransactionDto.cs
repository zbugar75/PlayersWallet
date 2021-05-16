using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Dtos.Enums;
using Zbugar75.PlayersWallet.Api.Utils.Extensions;

namespace Zbugar75.PlayersWallet.Api.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public TransactionTypeDto TransactionType { get; set; }

        public decimal Amount { get; set; }

        public static TransactionDto Create(Transaction transaction) =>
            new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType.ToTransactionTypeDto()
            };
    }
}
