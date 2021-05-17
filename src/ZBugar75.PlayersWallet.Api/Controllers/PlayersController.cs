using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zbugar75.PlayersWallet.Api.Domain.Entities;
using Zbugar75.PlayersWallet.Api.Dtos;
using Zbugar75.PlayersWallet.Api.Infrastructure.Services;
using Zbugar75.PlayersWallet.Api.Utils.Extensions;

namespace Zbugar75.PlayersWallet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ISimpleMemoryCache<TransactionResponse> _transactionResponseMemoryCache;

        public PlayersController(IPlayerService playerService, ISimpleMemoryCache<TransactionResponse> transactionResponseMemoryCache)
        {
            _playerService = playerService;
            _transactionResponseMemoryCache = transactionResponseMemoryCache;
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

        [HttpGet("{id}/transactions")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<TransactionDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            var transactions = await _playerService.GetTransactionsAsync(id, cancellationToken).ConfigureAwait(false);
            return transactions.Select(TransactionDto.Create);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status409Conflict)]
        public async Task<PlayerDto> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken cancellationToken)
        {
            var player = await _playerService.CreatePlayerAsync(request.Username, cancellationToken).ConfigureAwait(false);
            return PlayerDto.Create(player);
        }

        [HttpPost("{id}/registertransaction")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterTransaction(Guid id, [FromBody] RegisterTransactionRequest request, CancellationToken cancellationToken)
        {
            var transactionResponse = await _transactionResponseMemoryCache.GetOrCreateAsync(
                request.Id, () => _playerService.RegisterTransactionAsync(request.ToTransaction(id), cancellationToken))
                .ConfigureAwait(false);

            return StatusCode(transactionResponse.ResponseStatusCode);
        }
    }
}
