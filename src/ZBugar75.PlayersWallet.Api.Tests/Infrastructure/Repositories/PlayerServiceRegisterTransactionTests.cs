using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Domain.Entities.Enums;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations;
using Zbugar75.PlayersWallet.Api.Infrastructure.UnitOfWork;
using ZBugar75.PlayersWallet.Api.Tests.Shared;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Builder;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.Tests.Infrastructure.Repositories
{
    public class PlayerServiceRegisterTransactionTests
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPlayerService _underTest;
        private readonly IPlayersWalletContext _playersWalletContext;
        private IUnitOfWork _unitOfWorkMock;

        public PlayerServiceRegisterTransactionTests()
        {
            _cancellationToken = CancellationToken.None;
            _playersWalletContext = DataContextHelper.GetInMemoryDataContext();
            _unitOfWorkMock = DataContextHelper.GetUnitOfWorkMock(_playersWalletContext);
            _underTest = new PlayerService(_unitOfWorkMock);
        }

        [Theory]
        [InlineData(StatusCodes.Status202Accepted)]
        [InlineData(StatusCodes.Status403Forbidden)]
        public async Task RegisterTransactionAsync_ShouldReturnCachedValueAndGetExistingAsync_WhenCachedValueExists(int statusCode)
        {
            var playerId = 11.ToGuid();
            var transactionId = 21.ToGuid();

            _ = await new PlayerBuilder()
                .WithId(playerId)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var transaction = await new TransactionBuilder()
                .WithId(transactionId)
                .WithPalyerId(playerId)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            await new TransactionResponseBuilder()
                .WithTransactionId(transactionId)
                .WithResponseStatusCode(statusCode)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var result = await _underTest.RegisterTransactionAsync(transaction, _cancellationToken);

            result.ResponseStatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(0, TransactionType.Win, 10, 10)]
        [InlineData(50, TransactionType.Win, 50, 100)]
        [InlineData(0, TransactionType.Deposit, 10, 10)]
        [InlineData(50, TransactionType.Deposit, 50, 100)]
        [InlineData(10, TransactionType.Stake, 10, 0)]
        [InlineData(100, TransactionType.Stake, 50, 50)]
        public async Task RegisterTransactionAsync_ShouldExecuteOperation_WhenTransactionNotExistsAncNoLowBalance(
            decimal oldbalance, TransactionType transactionType, decimal amount, decimal newbalance)
        {
            var playerId = 11.ToGuid();
            var transactionId = 21.ToGuid();

            await new PlayerBuilder()
                .WithId(playerId)
                .WithBalance(oldbalance)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var transaction = await new TransactionBuilder()
                .WithId(transactionId)
                .WithPalyerId(playerId)
                .WithAmount(amount)
                .WithTransactionType(transactionType)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var result = await _underTest.RegisterTransactionAsync(transaction, _cancellationToken);

            result.ResponseStatusCode.Should().Be(StatusCodes.Status202Accepted);
            _playersWalletContext.Wallets.Select(w => w.Balance).Should().Equal(newbalance);
            _playersWalletContext.TransactionResponseCache
                .Where(tr => tr.TransactionId == transactionId && tr.ResponseStatusCode == StatusCodes.Status202Accepted)
                .Should().NotBeNull();
        }

        [Theory]
        [InlineData(0, TransactionType.Stake, 1)]
        [InlineData(100, TransactionType.Stake, 200)]
        public async Task RegisterTransactionAsync_ShouldNotExecuteOperationAndReturnForbidden_WhenTransactionNotExistsAncLowBalance(
            decimal balance, TransactionType transactionType, decimal amount)
        {
            var playerId = 11.ToGuid();
            var transactionId = 21.ToGuid();

            await new PlayerBuilder()
                .WithId(playerId)
                .WithBalance(balance)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var transaction = await new TransactionBuilder()
                .WithId(transactionId)
                .WithPalyerId(playerId)
                .WithAmount(amount)
                .WithTransactionType(transactionType)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var result = await _underTest.RegisterTransactionAsync(transaction, _cancellationToken);

            result.ResponseStatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _playersWalletContext.Wallets.Select(w => w.Balance).Should().Equal(balance);
            _playersWalletContext.TransactionResponseCache
                .Where(tr => tr.TransactionId == transactionId && tr.ResponseStatusCode == StatusCodes.Status403Forbidden)
                .Should().NotBeNull();
        }

        [Fact]
        public void RegisterTransactionAsync_ShouldRunWithoutErrorsAndIdempotent_WhenCalledConcurrently()
        {
            var players = new Player[5];
            Tuple<int, TransactionType, decimal>[] transactionBasis =
            {
                Tuple.Create(1, TransactionType.Deposit, 20M),
                Tuple.Create(2, TransactionType.Stake, -1M),
                Tuple.Create(3, TransactionType.Win, 20M),
                Tuple.Create(4, TransactionType.Stake, -10M),
                Tuple.Create(5, TransactionType.Stake, -100M),
                Tuple.Create(6, TransactionType.Deposit, 10M),
                Tuple.Create(7, TransactionType.Win, 20M),
            };
            var results = new ConcurrentQueue<TransactionResponse>();

            var responses = new Dictionary<Guid, int>();

            Parallel.For(0, players.Length, async i =>
            {
                players[i] = await _underTest.CreatePlayerAsync($"player{i}", _cancellationToken);

                Parallel.ForEach(transactionBasis, (tb) =>
                    {
                        var transaction = new Transaction
                        {
                            Id = (i * 1000 + tb.Item1).ToGuid(),
                            PlayerId = players[i].Id,
                            TransactionType = tb.Item2,
                            Amount = tb.Item3
                        };
                        Parallel.For(0, 20, async i =>
                        {
                            var result = await _underTest.RegisterTransactionAsync(transaction, _cancellationToken);
                            results.Enqueue(result);
                        });
                    });
            });

            while (results.TryDequeue(out var result))
            {
                if (responses.ContainsKey(result.TransactionId))
                {
                    responses[result.TransactionId].Should().Be(result.ResponseStatusCode);
                }
                else
                {
                    responses.Add(result.TransactionId, result.ResponseStatusCode);
                }
            }

            true.Should().BeTrue();
        }
    }
}
