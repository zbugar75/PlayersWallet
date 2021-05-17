using System.Collections.Generic;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;
using Zbugar75.PlayersWallet.Api.Dtos.Enums;

namespace Zbugar75.PlayersWallet.Api.Utils.Extensions
{
    public static class TransactionTypeDtoExtensions
    {
        private static readonly Dictionary<TransactionType, TransactionTypeDto> TransactionTypeTranslator =
            InitializeStateTranslator();

        public static TransactionTypeDto ToTransactionTypeDto(this TransactionType transactionType) =>
            TransactionTypeTranslator[transactionType];

        private static Dictionary<TransactionType, TransactionTypeDto> InitializeStateTranslator()
        {
            var translator = new Dictionary<TransactionType, TransactionTypeDto>
            {
                { TransactionType.Deposit,  TransactionTypeDto.Deposit },
                { TransactionType.Stake,  TransactionTypeDto.Stake },
                { TransactionType.Win,  TransactionTypeDto.Win }
            };

            return translator;
        }
    }
}