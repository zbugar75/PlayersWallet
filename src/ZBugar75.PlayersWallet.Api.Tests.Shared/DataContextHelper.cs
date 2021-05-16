using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Dtos;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;

namespace ZBugar75.PlayersWallet.Api.Tests.Shared
{
    public static class DataContextHelper
    {
        public static IPlayersWalletContext GetInMemoryDataContext()
        {
            return new PlayersWalletContext(new DbContextOptionsBuilder<PlayersWalletContext>()
                .EnableServiceProviderCaching(false)
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), new InMemoryDatabaseRoot())
                .Options);
        }

        public static Func<Player, Player, bool> ComparePlayersListsFunc()
        {
            return (c1, c2) => c1.Id == c2.Id && c1.Username == c2.Username && c1.Wallet.Balance == c2.Wallet.Balance;
        }

        public static Func<PlayerDto, Player, bool> ComparePlayersDtoListsFunc()
        {
            return (c1, c2) => c1.Id == c2.Id && c1.Username == c2.Username;
        }

        public static Func<TransactionResponse, TransactionResponse, bool> CompareTransactionResponseListsFunc()
        {
            return (c1, c2) => c1.TransactionId == c2.TransactionId && c1.ResponseStatusCode == c2.ResponseStatusCode;
        }
    }
}
