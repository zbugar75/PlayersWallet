using System.Collections.Generic;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Infrastructure.DbContext;
using ZBugar75.PlayersWallet.Api.Tests.Shared.Extensions;

namespace ZBugar75.PlayersWallet.Api.IntegrationTests.Helper
{
    public static class Utilities
    {
        public static void InitializeDbForTests(PlayersWalletContext db)
        {
            db.Players.AddRange(GetSeedingPlayers());
            db.Wallets.AddRange(GetSeedingWallets());
            db.SaveChanges();
        }

        public static List<Wallet> GetSeedingWallets()
        {
            return new List<Wallet>()
            {
                new Wallet { PlayerId = 1.ToGuid(), Balance = 1000 },
                new Wallet { PlayerId = 2.ToGuid(), Balance = 20 },
                new Wallet { PlayerId = 3.ToGuid(), Balance = 0 }
            };
        }

        public static void ReinitializeDbForTests(PlayersWalletContext db)
        {
            db.Players.RemoveRange(db.Players);
            InitializeDbForTests(db);
        }

        public static List<Player> GetSeedingPlayers()
        {
            return new List<Player>()
            {
                new Player { Id = 1.ToGuid(), Username = "username1" },
                new Player { Id = 2.ToGuid(), Username = "username2" },
                new Player { Id = 3.ToGuid(), Username = "username3" }
            };
        }
    }
}
