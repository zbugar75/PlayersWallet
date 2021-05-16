using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zbugar75.PlayersWallet.Api.Common.Exceptions;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations;
using ZBugar75.PlayersWallet.Api.Tests.Shared;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Builder;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.Tests.Infrastructure.Repositories
{
    public class PlayerServiceTests
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPlayerService _underTest;
        private readonly IPlayersWalletContext _playersWalletContext;

        public PlayerServiceTests()
        {
            _cancellationToken = CancellationToken.None;
            _playersWalletContext = DataContextHelper.GetInMemoryDataContext();
            var unitOfWork = DataContextHelper.GetUnitOfWork(_playersWalletContext);
            _underTest = new PlayerService(unitOfWork);
        }

        [Fact]
        public async Task GetPlayersAsync_ShouldReturnEmptyArray_WhenNoPlayerExists()
        {
            var result = await _underTest.GetPlayersAsync(_cancellationToken);

            result.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetPlayersAsync_ShouldReturnListOfPlayers_WhenPlayersExists()
        {
            var player1 = await new PlayerBuilder().CreateAsync(_playersWalletContext, _cancellationToken);
            var player2 = await new PlayerBuilder().CreateAsync(_playersWalletContext, _cancellationToken);

            var result = await _underTest.GetPlayersAsync(_cancellationToken);

            result.Should().Equal(
                new Player[] { player1, player2 },
                DataContextHelper.ComparePlayersListsFunc());
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldThrowArgumentNullException_WhenCalledWithNull()
        {
            Func<Task> act = async () => { await _underTest.CreatePlayerAsync(null, _cancellationToken); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldThrowArgumentNullException_WhenCalledWithEmptyString()
        {
            var username = string.Empty;

            Func<Task> act = async () => { await _underTest.CreatePlayerAsync(username, _cancellationToken); };

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldCreatePlayer_WhenCalledWithNewPlayerName()
        {
            var username = "username";

            await _underTest.CreatePlayerAsync(username, _cancellationToken);

            _playersWalletContext.Players.Count().Should().Be(1);
            _playersWalletContext.Players.Select(p => p.Username).Should().Equal(username);
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldCreatePlayerWithZeroBalance_WhenCalled()
        {
            var username = "username";

            await _underTest.CreatePlayerAsync(username, _cancellationToken);

            _playersWalletContext.Players.Count().Should().Be(1);
            _playersWalletContext.Players.Select(p => p.Wallet.Balance).Should().Equal(0);
        }

        [Fact]
        public async Task CreatePlayerAsync_ShouldShouldThrowException_WhenCalledWithDuplicateName()
        {
            var username = "username";

            await _underTest.CreatePlayerAsync(username, _cancellationToken);

            Func<Task> act = async () => { await _underTest.CreatePlayerAsync(username, _cancellationToken); };
            await act.Should().ThrowAsync<DuplicateEntityException>();
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldThrowEntityNotFoundException_WhenPlayerNotExists()
        {
            Func<Task> act = async () => { await _underTest.GetBalanceAsync(1.ToGuid(), _cancellationToken); };
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldReturnBalance_WhenCalledForExistingPlayer()
        {
            var playerId = 1.ToGuid();
            var balance = 1000;

            _ = await new PlayerBuilder()
                .WithId(playerId)
                .WithBalance(balance)
                .CreateAsync(_playersWalletContext, _cancellationToken);

            var result = await _underTest.GetBalanceAsync(1.ToGuid(), _cancellationToken);

            result.Should().Be(balance);
        }
    }
}
