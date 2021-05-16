using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zbugar75.PlayersWallet.Api.Exceptions;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories.Implementations;
using ZBugar75.PlayersWallet.Api.Tests.Shared;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Builder;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.Tests.Infrastructure.Repositories
{
    public class WalletRepositoryTests
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IPlayersWalletContext _playersWalletContext;
        private readonly IWalletRepository _underTest;

        public WalletRepositoryTests()
        {
            _cancellationToken = CancellationToken.None;
            _playersWalletContext = DataContextHelper.GetInMemoryDataContext();
            _underTest = new WalletRepository(_playersWalletContext);
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
