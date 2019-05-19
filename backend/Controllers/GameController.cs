using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToughBattle.Controllers.Dto;
using ToughBattle.Facades;
using ToughBattle.Models;

namespace ToughBattle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameFacade _gameFacade;
        public GameController(IGameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GameInfo(int gameId)
        {
            var game = await _gameFacade.GameInfo(gameId);
            if (game == null)
            {
                return NotFound("Not found");
            }
            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> StartGame(NewGame teams)
        {

            return Ok(await _gameFacade.CreateGame(teams));
        }

        [HttpPost("tournamentGame")]
        public async Task<IActionResult> StartTournamentGame(NewTournamentGame game)
        {
            return Ok(await _gameFacade.CreateTournamentGame(game));
        }

        [HttpPost("playoffsGame")]
        public async Task<IActionResult> StartPlayoffsGame(NewPlayoffsGame game)
        {
            return Ok(await _gameFacade.CreatePlayoffsGame(game));
        }

        [HttpPost("finish")]
        public async Task<IActionResult> FinishGame(GameResult result)
        {
            return Ok(await _gameFacade.FinishGame(result));
        }
    }
}
