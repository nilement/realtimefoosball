using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughBattle.Controllers.Dto;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public interface ITournamentsFacade
    {
        Task<List<Tournament>> RunningTournaments();
        Task<TournamentInfo> CreateTournament(Tournament tournament, List<int> players);
        Task<List<TournamentGroup>> TournamentGroups(int id);
        Task<List<TournamentPlayer>> TournamentsPlayers(int tournamentId);
        Task<TournamentInfo> TournamentDetails(int tournamentId);
        Task<Matchup> PlayerToPlayoffs(int playerId);
    }
}
