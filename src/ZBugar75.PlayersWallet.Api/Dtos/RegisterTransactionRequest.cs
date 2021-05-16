using System;
using Zbugar75.PlayersWallet.Api.Dtos.Enums;

namespace Zbugar75.PlayersWallet.Api.Dtos
{
    public class RegisterTransactionRequest
    {
        public Guid Id { get; set; }

        public TransactionTypeDto TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
