using System;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class TransactionResponse
    {
        public Guid TransactionId { get; set; }

        public int ResponseStatusCode { get; set; }
    }
}
