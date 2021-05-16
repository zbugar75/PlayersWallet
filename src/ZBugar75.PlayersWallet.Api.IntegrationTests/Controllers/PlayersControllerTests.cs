using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;
using Zbugar75.PlayersWallet.Api;
using Zbugar75.PlayersWallet.Api.Dtos;
using ZBugar75.PlayersWallet.Api.IntegrationTests.Helper;
using ZBugar75.PlayersWallet.Api.Tests.Shared;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.IntegrationTests.Controllers
{
    public class PlayersControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public PlayersControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetPlayers_ShouldReturnSeededPlayers_WhenCalled()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/players");
            var jsonFromResponse = await response.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<IEnumerable<PlayerDto>>(jsonFromResponse);

            response.EnsureSuccessStatusCode();

            singleResponse.Should().Equal(
                Utilities.GetSeedingPlayers(),
                DataContextHelper.ComparePlayersDtoListsFunc());
        }

        [Fact]
        public async Task GetBalance_ShouldReturnBalance_WhenCalledForExistingPlayer()
        {
            var client = _factory.CreateClient();
            var wallet = Utilities.GetSeedingWallets()[0];

            var response = await client.GetAsync($"/players/{wallet.PlayerId}/balance");
            var jsonFromResponse = await response.Content.ReadAsStringAsync();
            var playerBalanceResponse = JsonConvert.DeserializeObject<PlayerBalanceDto>(jsonFromResponse);

            response.EnsureSuccessStatusCode();

            playerBalanceResponse.Balance.Should().Be(wallet.Balance);
            playerBalanceResponse.PlayerId.Should().Be(wallet.PlayerId);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnNotFound_WhenCalledForNotExistingPlayer()
        {
            var client = _factory.CreateClient();

            var notExistingPlayer = 999.ToGuid();
            var response = await client.GetAsync($"/players/{notExistingPlayer}/balance");

            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task CreatePlayer_ShouldReturnBadRequest_WhenCalledWithEmptyUsername()
        {
            var client = _factory.CreateClient();
            var body = new { username = "" };

            var response = await client.PostAsync("/players", ContentHelper.GetStringContent(body));

            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreatePlayer_ShouldReturnConflict_WhenCalledWithExistingUsername()
        {
            var client = _factory.CreateClient();
            var player = Utilities.GetSeedingPlayers()[0];
            var body = new { username = player.Username };

            var response = await client.PostAsync("/players", ContentHelper.GetStringContent(body));

            response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact]
        public async Task CreatePlayer_ShouldCreatePlayer_WhenCalledNewUsername()
        {
            var client = _factory.CreateClient();
            const string username = "new user";
            var body = new { username };

            var response = await client.PostAsync("/players", ContentHelper.GetStringContent(body));
            var jsonFromResponse = await response.Content.ReadAsStringAsync();
            var playerResponse = JsonConvert.DeserializeObject<PlayerDto>(jsonFromResponse);

            response.EnsureSuccessStatusCode();

            playerResponse.Username.Should().Be(username);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnStatus404NotFound_WhenCalledWithNotExistingPlayer()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/players/{999.ToGuid()}/transactions");

            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnSeededTransactions_WhenCalledWithExistingPlayer()
        {
            var client = _factory.CreateClient();

            var player = Utilities.GetSeedingPlayers()[0];

            var response = await client.GetAsync($"/players/{player.Id}/transactions");
            var jsonFromResponse = await response.Content.ReadAsStringAsync();
            var transactionsResponse = JsonConvert.DeserializeObject<IEnumerable<TransactionDto>>(jsonFromResponse);

            response.EnsureSuccessStatusCode();

            transactionsResponse.Should().Equal(
                Utilities.GetSeedingTransactions().Where(t => t.PlayerId == player.Id),
                DataContextHelper.CompareTransactionDtoToTransaction());
        }
    }
}