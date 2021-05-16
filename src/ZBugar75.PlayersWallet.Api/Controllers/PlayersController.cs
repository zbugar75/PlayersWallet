using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zbugar75.PlayersWallet.Api.Dtos;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;

namespace Zbugar75.PlayersWallet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<PlayerDto>> GetPlayers(CancellationToken cancellationToken)
        {
            var players = await _playerService.GetPlayersAsync(cancellationToken).ConfigureAwait(false);
            return players.Select(PlayerDto.Create);
        }

        [HttpGet("{id}/balance")]
        [ProducesResponseType(typeof(PlayerBalanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public async Task<PlayerBalanceDto> GetBalance(Guid id, CancellationToken cancellationToken)
        {
            var wallet = await _playerService.GetBalanceAsync(id, cancellationToken).ConfigureAwait(false);
            return PlayerBalanceDto.Create(wallet);
        }

        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public async Task<decimal> GetBalance(Guid id, CancellationToken cancellationToken)
        {
            var balance = await _playerService.GetBalanceAsync(id, cancellationToken).ConfigureAwait(false);
            return balance;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status409Conflict)]
        public async Task<PlayerDto> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken cancellationToken)
        {
            var player = await _playerService.CreatePlayerAsync(request.Username, cancellationToken).ConfigureAwait(false);
            return PlayerDto.Create(player);
        }
    }
}
