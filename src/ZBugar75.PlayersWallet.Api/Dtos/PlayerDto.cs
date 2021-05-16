using System;
using Zbugar75.PlayersWallet.Api.Domain.Entities;

namespace Zbugar75.PlayersWallet.Api.Dtos
{
    public class PlayerDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public static PlayerDto Create(Player player) =>
            new PlayerDto
            {
                Id = player.Id,
                Username = player.Username
            };
    }
}
