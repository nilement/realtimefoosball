using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ToughBattle.Controllers.Dto;
using ToughBattle.Facades;
using ToughBattle.Models;

namespace ToughBattle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : Controller
    {
        private readonly ITournamentsFacade _tournamentsFacade;
        private readonly IConfiguration _configuration;

        public TournamentController(ITournamentsFacade tournamentsFacade, IConfiguration configuration)
        {
            _tournamentsFacade = tournamentsFacade;
            _configuration = configuration;
        }
        [HttpGet("running")]
        public async Task<IActionResult> RunningTournaments()
        {
            var lul = _configuration.GetSection("Database:ConnectionString").Value;
            return Ok(await _tournamentsFacade.RunningTournaments());
        }

        [Authorize(Roles=Role.User+ "," + Role.Admin)]
        [HttpGet("{tournamentId}")]
        public async Task<IActionResult> TournamentDetails(int tournamentId)
        {
            return Ok(await _tournamentsFacade.TournamentDetails(tournamentId));
        }

        [HttpGet("{tournamentId}/groups")]
        public async Task<IActionResult> TournamentGroups(int tournamentId)
        {
            return Ok(await _tournamentsFacade.TournamentGroups(tournamentId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody]NewTournament newTournament)
        {
            return Ok(await _tournamentsFacade.CreateTournament(newTournament.Tournament, newTournament.Players));
        }

        [HttpPost("moveToPlayoffs")]
        public async Task<IActionResult> MoveToPlayoffs(PlayoffMove tournamentPlayerId)
        {
            return Ok(await _tournamentsFacade.PlayerToPlayoffs(tournamentPlayerId.TournamentPlayerId));
        }
    }
}
