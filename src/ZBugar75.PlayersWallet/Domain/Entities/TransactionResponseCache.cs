using System;

namespace Zbugar75.PlayersWallet.Api.Domain.Entities
{
    public class TransactionResponseCache
    {
        public Guid TransactionId { get; set; }

        public int ResponseStatusCode { get; set; }
    }
}
