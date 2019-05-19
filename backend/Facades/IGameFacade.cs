using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughBattle.Controllers.Dto;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public interface IGameFacade
    {
        Task<Game> CreateGame(NewGame newGame);
        Task<Game> GameInfo(int GameId);
        Task<Game> RunningGame();
        Task<Game> CreateTournamentGame(NewTournamentGame game);
        Task<Game> CreatePlayoffsGame(NewPlayoffsGame game);
        Task<Game> FinishGame(GameResult gameResult);
    }
}
