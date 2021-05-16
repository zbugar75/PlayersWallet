using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations;
using ZBugar75.PlayersWallet.Api.Tests.Shared;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Builder;

namespace ZBugar75.PlayersWallet.Api.Tests.Infrastructure.Repositories
{
    public class TransactionResponseCacheRepositoryTests
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPlayersWalletContext _playersWalletContext;
        private readonly ITransactionResponseCacheRepository _underTest;

        public TransactionResponseCacheRepositoryTests()
        {
            _cancellationToken = CancellationToken.None;
            _playersWalletContext = DataContextHelper.GetInMemoryDataContext();
            _underTest = new TransactionResponseCacheRepository(_playersWalletContext);
        }

        [Fact]
        public async Task AddTransactionResponseAsync_ShouldThrowDuplicateUsernameException_WhenCalledForExistingTransaction()
        {
            var transactionResponse = await new TransactionResponseBuilder().CreateAsync(_playersWalletContext, _cancellationToken);

            Func<Task> act = async () => { await _underTest.AddTransactionResponseAsync(transactionResponse, _cancellationToken); };

            await act.Should().ThrowAsync<DuplicateUsernameException>();
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldCreatePlayer_WhenCalledWithNewPlayerName()
        {
            var transactionResponse = new TransactionResponseBuilder().Build();

            await _underTest.AddTransactionResponseAsync(transactionResponse, _cancellationToken);

            _playersWalletContext.TransactionResponseCache.Count().Should().Be(1);
            _playersWalletContext.TransactionResponseCache.Should().Equal(
                new TransactionResponse[] { transactionResponse },
                DataContextHelper.CompareTransactionResponseListsFunc());
        }

        [Fact]
        public async Task GetTransactionResponseAsync_ShouldThrowEntityNotFoundException_WhenCalledForNotExistingTransaction()
        {
            var transactionId = Guid.NewGuid();

            Func<Task> act = async () => { await _underTest.GetTransactionResponseAsync(transactionId, _cancellationToken); };

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Theory]
        [InlineData(StatusCodes.Status202Accepted)]
        [InlineData(StatusCodes.Status403Forbidden)]
        [InlineData(StatusCodes.Status404NotFound)]
        public async Task GetTransactionResponseAsync_ShouldReturnResponseStatusCode_WhenCalledForExistingTransaction(int statusCode)
        {
            var transactionResponse = await new TransactionResponseBuilder()
                .WithResponseStatusCode(statusCode)
                .CreateAsync(_playersWalletContext, _cancellationToken);


            var result = await _underTest.GetTransactionResponseAsync(transactionResponse.TransactionId, _cancellationToken);

            result.Should().Be(statusCode);
        }
    }
}
