using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zbugar75.PlayersWallet.Api.Dtos;

namespace Zbugar75.PlayersWallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ILogger<TransactionsController> _logger;

        public TransactionsController(ILogger<TransactionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{playerId}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status404NotFound)]
        public IEnumerable<TransactionDto> Get(Guid playerId)
        {
            // TODO: Implement method
            IEnumerable<TransactionDto> list = new List<TransactionDto>();
            return list.ToArray();
        }
    }
}
