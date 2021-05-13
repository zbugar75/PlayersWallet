using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zbugar75.PlayersWallet.Api.Dtos;

namespace Zbugar75.PlayersWallet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(ILogger<PlayersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
        public IEnumerable<PlayerDto> Get()
        {
            // TODO: Implement method
            IEnumerable<PlayerDto> list = new List<PlayerDto>();
            return list.ToArray();
        }
    }
}
