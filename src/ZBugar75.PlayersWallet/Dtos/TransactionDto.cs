using System;

namespace Zbugar75.PlayersWallet.Api.Dtos
{
    public class TransactionDto
    {
        public Guid id { get; set; }
        public decimal amount { get; set; }
    }
}
