using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Controllers.Dto;
using ToughBattle.Database;
using ToughBattle.Models;
using ToughBattle.Models.Enums;

namespace ToughBattle.Facades
{
    public class GameFacade : IGameFacade
    {
        private readonly FoosballContext _db;

        public GameFacade(FoosballContext ctx)
        {
            _db = ctx;
        }

        public async Task<Game> GameInfo(int id)
        {
            return await _db.Games.Include(x => x.BP1)
                .Include(x => x.RP1)
                .Include(x => x.TournamentMatchup)
                .Include(x => x.Tournament).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Game> RunningGame()
        {
            return await _db.Games.Include(x => x.BP1)
                .Include(x => x.RP1)
                .Include(x => x.TournamentMatchup)
                .Include(x => x.Tournament).FirstOrDefaultAsync(x => x.EndDate == null);
        }

        public async Task<Game> CreateGame(NewGame game)
        {
                var gameItem = new Game{ PlayerType = game.PlayerType, StartDate = DateTime.Now };
                if (game.PlayerType == PlayerType.Pair)
                {
                    gameItem.BP1 = _db.Players.FirstOrDefault(x => x.Id == game.BlueTeam[0]);
                    gameItem.BP2 = _db.Players.FirstOrDefault(x => x.Id == game.BlueTeam[1]);
                    gameItem.RP1 = _db.Players.FirstOrDefault(x => x.Id == game.RedTeam[0]);
                    gameItem.RP2 = _db.Players.FirstOrDefault(x => x.Id == game.RedTeam[1]);
                }
                else
                {
                    gameItem.BP1 = _db.Players.FirstOrDefault(x => x.Id == game.BlueTeam[0]);
                    gameItem.RP1 = _db.Players.FirstOrDefault(x => x.Id == game.RedTeam[0]);
                }
                await _db.AddAsync(gameItem);
                await _db.SaveChangesAsync();
                return gameItem;
        }

        public async Task<Game> CreateTournamentGame(NewTournamentGame game)
        {
                var blue = await _db.Players.FirstOrDefaultAsync(x => x.Id == game.BlueTeam);
                var red = await _db.Players.FirstOrDefaultAsync(x => x.Id == game.RedTeam);
                var t = await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == game.TournamentId);
                var eligibility = await CheckIfGroupElligible(game.TournamentId, blue, red, _db);
                if (!eligibility)
                {
                    throw new ArgumentException();
                }
                var gameItem = new Game
                    {StartDate = DateTime.Now, PlayerType = PlayerType.Single, GameType = GameType.TournamentGroup, BP1 = blue, RP1 = red, Tournament = t};
                await _db.AddAsync(gameItem);
                await _db.SaveChangesAsync();
                return gameItem;
        }

        public async Task<Game> CreatePlayoffsGame(NewPlayoffsGame game)
        {
            var blue = await _db.Players.FirstOrDefaultAsync(x => x.Id == game.BluePlayer);
            var red = await _db.Players.FirstOrDefaultAsync(x => x.Id == game.RedPlayer);
            var m = await _db.Matchups.Include(x => x.Tournament).Include(x => x.T1P1).Include(x => x.T2P1).FirstOrDefaultAsync(x => x.Id == game.MatchupId);
            var eligibility = CheckIfPlayoffsElligible(m, blue, red);
            if (!eligibility)
            {
                throw new ArgumentException();
            }
            var gameItem = new Game
                { StartDate = DateTime.Now, PlayerType = PlayerType.Single, GameType = GameType.TournamentPlayoffs, BP1 = blue, RP1 = red, Tournament = m.Tournament, TournamentMatchup = m};

            await _db.AddAsync(gameItem);
            await _db.SaveChangesAsync();
            return gameItem;
        }

        public async Task<Game> FinishGame(GameResult gameResult)
        {
                var game = await _db.Games.Include(y => y.BP1)
                    .Include(z => z.RP1)
                    .Include(x => x.Tournament)
                    .Include(x => x.TournamentMatchup)
                    .Include(x => x.TournamentMatchup.NextMatchup)
                    .Include(x => x.TournamentMatchup.T1P1)
                    .Include(x => x.TournamentMatchup.T2P1)
                    .Include(x => x.TournamentMatchup.T1P1.Player)
                    .Include(x => x.TournamentMatchup.T2P1.Player)
                    .FirstOrDefaultAsync(x => x.Id == gameResult.GameId);
                UpdateGameStats(game, gameResult);
                switch (game?.GameType)
                {
                    case GameType.Free:
                        await game.UpdatePlayerStats(gameResult, _db);
                        break;
                    case GameType.TournamentGroup:
                        var tId = (await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == game.Tournament.Id)).Id;
                        await game.UpdateTournamentPlayerStats(gameResult, tId, _db);
                        break;
                    case GameType.TournamentPlayoffs:
                        await CalculateMatchup(game, _db);
                        break;
                }
                return game;
        }

        private async Task<bool> CheckIfGroupElligible(int tournamentId, Player BluePlayer, Player RedPlayer, FoosballContext db)
        {
            var blueTourney = await db.TournamentPlayers.FirstOrDefaultAsync(x => x.Tournament.Id == tournamentId && x.Player.Id == BluePlayer.Id);
            var redTourney = await db.TournamentPlayers.FirstOrDefaultAsync(x => x.Tournament.Id == tournamentId && x.Player.Id == RedPlayer.Id);
            if (blueTourney.GroupId != redTourney.GroupId)
            {
                return false;
            }

            var havePlayed =  await db.Games.Where(x => x.Tournament.Id == tournamentId && x.GameType == GameType.TournamentGroup)
                .AnyAsync(x => x.BP1.Id == BluePlayer.Id && x.RP1.Id == RedPlayer.Id && x.EndDate != null);
            return !havePlayed;
        }

        private bool CheckIfPlayoffsElligible(Matchup matchup, Player Blue, Player Red)
        {
            if (matchup.T1P1.Player.Id == Blue.Id && matchup.T2P1.Player.Id == Red.Id)
            {
                return true;
            }

            if (matchup.T1P1.Player.Id == Red.Id && matchup.T2P1.Player.Id == Blue.Id)
            {
                return true;
            }

            return false;
        }


        private async Task CalculateMatchup(Game game, FoosballContext db)
        {
            var matchup = game.TournamentMatchup;
            var winsRequired = WinsForPhase(game.TournamentMatchup.TournamentPhase);
            if (game.PlayerType == PlayerType.Single)
            {
                if (game.RedTeamScore > game.BlueTeamScore)
                {
                    await matchup.SetSingleWin(game.RP1, game.BP1, db, winsRequired);
                }
                else
                {
                    await matchup.SetSingleWin(game.BP1, game.RP1, db, winsRequired);
                }
            }
        }

        private void UpdateGameStats(Game game, GameResult result)
        {
            game.BlueTeamScore = result.BlueScore;
            game.RedTeamScore = result.RedScore;
            game.EndDate = DateTime.Now;
        }

        private int WinsForPhase(TournamentPhase phase)
        {
            switch (phase)
            {
                case TournamentPhase.Final: return 4;
                case TournamentPhase.SemiFinal: return 3;
                default: return 2;
            }
        }
    }
}
