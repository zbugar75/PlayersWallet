using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zbugar75.PlayersWallet.Api.Dtos;
using Zbugar75.PlayersWallet.Api.Infrastructure.Repositories;

namespace Zbugar75.PlayersWallet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWalletRepository _walletRepository;

        public PlayersController(ILogger<PlayersController> logger, IPlayerRepository playerRepository, IWalletRepository walletRepository)
        {
            _logger = logger;
            _playerRepository = playerRepository;
            _walletRepository = walletRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<PlayerDto>> GetPlayers(CancellationToken cancellationToken)
        {
            var players = await _playerRepository.GetPlayersAsync(cancellationToken).ConfigureAwait(false);
            return players.Select(PlayerDto.Create);
        }

        [HttpGet("{id}/balance")]
        [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public async Task<decimal> GetBalance(Guid id, CancellationToken cancellationToken)
        {
            var balance = await _walletRepository.GetBalanceAsync(id, cancellationToken).ConfigureAwait(false);
            return balance;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status409Conflict)]
        public async Task<PlayerDto> CreatePlayer(string username, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.CreatePlayerAsync(username, cancellationToken).ConfigureAwait(false);
            return PlayerDto.Create(player);
        }
    }
}
