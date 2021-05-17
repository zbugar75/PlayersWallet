using System.Collections.Generic;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;
using Zbugar75.PlayersWallet.Api.Dtos.Enums;

namespace Zbugar75.PlayersWallet.Api.Utils.Extensions
{
    public static class TransactionTypeExtensions
    {
        private static readonly Dictionary<TransactionTypeDto, TransactionType> TransactionTypeTranslator =
            InitializeStateTranslator();

        public static TransactionType ToTransactionType(this TransactionTypeDto transactionType) =>
            TransactionTypeTranslator[transactionType];

        private static Dictionary<TransactionTypeDto, TransactionType> InitializeStateTranslator()
        {
            var translator = new Dictionary<TransactionTypeDto, TransactionType>
            {
                { TransactionTypeDto.Deposit,  TransactionType.Deposit },
                { TransactionTypeDto.Stake,  TransactionType.Stake },
                { TransactionTypeDto.Win,  TransactionType.Win }
            };

            return translator;
        }
    }
}