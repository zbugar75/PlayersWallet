using System;
using Zbugar75.PlayersWallet.Api.Dtos;
using Zbugar75.PlayersWallet.Api.Dtos.Enums;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared.Builder
{
    public class RegisterTransactionRequestBuilder
    {
        private decimal _amount;
        private Guid _id;
        private TransactionTypeDto _transactionType;

        public RegisterTransactionRequestBuilder()
        {
            _id = Guid.NewGuid();
            _amount = 0;
            _transactionType = TransactionTypeDto.Win;
        }

        public RegisterTransactionRequestBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public RegisterTransactionRequestBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public RegisterTransactionRequestBuilder WithTransactionType(TransactionTypeDto transactionType)
        {
            _transactionType = transactionType;
            return this;
        }

        public RegisterTransactionRequest Build()
        {
            return new RegisterTransactionRequest
            {
                Amount = _amount,
                Id = _id,
                TransactionType = _transactionType
            };
        }
    }
}
