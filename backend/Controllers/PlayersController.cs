using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToughBattle.Facades;
using ToughBattle.Models;

namespace ToughBattle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayersFacade _playersFacade;
        public PlayersController(IPlayersFacade playersFacade)
        {
            _playersFacade = playersFacade;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            return Ok(await _playersFacade.GetPlayers());
        }

        [HttpPost]
        public IActionResult CreateNewPlayer()
        {
            return Ok();
        }
    }
}
