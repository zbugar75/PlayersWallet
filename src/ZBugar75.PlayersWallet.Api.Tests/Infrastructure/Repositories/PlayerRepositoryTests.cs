using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class PlayerRepositoryTests
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPlayersWalletContext _playersWalletContext;
        private readonly IPlayerRepository _underTest;

        public PlayerRepositoryTests()
        {
            _cancellationToken = CancellationToken.None;
            _playersWalletContext = DataContextHelper.GetInMemoryDataContext();
            _underTest = new PlayerRepository(_playersWalletContext);
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
            string username = null;

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
            await act.Should().ThrowAsync<DuplicateUsernameException>();
        }
    }
}
