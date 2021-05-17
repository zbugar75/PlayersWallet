using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Dtos;

namespace Zbugar75.PlayersWallet.Api.Utils.Extensions
{
    public static class RegisterTransactionRequestExtensions
    {
        public static Transaction ToTransaction(this RegisterTransactionRequest transaction, Guid playerId) =>
            new Transaction
            {
                Id = transaction.Id,
                PlayerId = playerId,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType.ToTransactionType()
            };

    }
}
