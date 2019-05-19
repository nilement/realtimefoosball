using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Controllers.Dto;
using ToughBattle.Database;
using ToughBattle.Facades;
using ToughBattle.Models;
using ToughBattle.Models.Enums;
using Xunit;

namespace BattleTests
{
    public class GameUnitTests
    {
        [Fact]
        public async Task StartSingleFreeGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1};
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2};

            var blueTeam = new List<int> { p1.Id };
            var redTeam = new List<int> { p2.Id };

            await dbCtx.AddAsync(p1);
            await dbCtx.AddAsync(p2);
            await dbCtx.SaveChangesAsync();

            var newgame = new NewGame {BlueTeam = blueTeam, RedTeam = redTeam, PlayerType = PlayerType.Single};

            // Act
            var newGame = await gameFacade.CreateGame(newgame);

            // Assert
            var game = await dbCtx.Games.Include(x=> x.BP1).Include(x => x.RP1).FirstOrDefaultAsync(x => x.Id == newGame.Id);
            Assert.NotNull(game);
            Assert.Equal(p1.Id, game.BP1.Id);
            Assert.Equal(p2.Id, game.RP1.Id);
            Assert.False(game.EndDate.HasValue);

        }

        [Fact]
        public async Task FinishFreeGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);

            var p1 = new Player {Name = "p1", Wins = 0, Losses = 0};
            var p2 = new Player {Name = "p2", Wins = 0, Losses = 0};

            var game = new Game {GameType = GameType.Free, BP1 = p1, RP1 = p2};

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(game);

            var gameId = game.Id;
            var result = new GameResult {BlueScore = 10, GameId = gameId, RedScore = 9};
            dbCtx.SaveChanges();

            // Act 
            await gameFacade.FinishGame(result);

            // Assert
            var finishedGame = dbCtx.Games.Include(x => x.BP1).Include(x => x.RP1).FirstOrDefault(x => x.Id == gameId);
            var gamep1 = dbCtx.Players.FirstOrDefault(x => x.Id == finishedGame.BP1.Id);
            var gamep2 = dbCtx.Players.FirstOrDefault(x => x.Id == finishedGame.RP1.Id);

            Assert.True(game.EndDate.HasValue);
            Assert.Equal(1, gamep1.Wins);
            Assert.Equal(1, gamep2.Losses);
        }

        [Fact]
        public async Task StartSingleTournamentGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 4 };


            var tournament = new Tournament { GroupCount = 1, HasGroupStage = true };
            var players = new List<int> { 1, 2, 3, 4 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);

//            var blueTeam = new List<int> { p1.Id };
//            var redTeam = new List<int> { p2.Id };

            var nGame = new NewTournamentGame
                {BlueTeam = p1.Id, RedTeam = p2.Id, TournamentId = info.Tournament.Id};

            // Act
            var game = await gameFacade.CreateTournamentGame(nGame);

            // Assert
            var tGame = dbCtx.Games.Include(x => x.Tournament).FirstOrDefault(x => x.Id == game.Id);
            Assert.NotNull(tGame);
            Assert.Null(tGame.EndDate);
            Assert.Equal(tournament.Id, tGame.Tournament.Id);
            Assert.Equal(p1.Id, tGame.BP1.Id);
            Assert.Equal(p2.Id, tGame.RP1.Id);
        }

        [Fact]
        public async Task FinishSingleTournamentGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };


            var tournament = new Tournament { GroupCount = 1, HasGroupStage = true };
            var players = new List<int> { 1, 2, 3, 4 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);

            //            var blueTeam = new List<int> { p1.Id };
            //            var redTeam = new List<int> { p2.Id };

            var nGame = new NewTournamentGame
                { BlueTeam = p1.Id, RedTeam = p2.Id, TournamentId = info.Tournament.Id };

            // Act
            var game = await gameFacade.CreateTournamentGame(nGame);
            var result = new GameResult { BlueScore = 10, GameId = game.Id, RedScore = 9 };
            var finish = await  gameFacade.FinishGame(result);

            // Assert
            var tGame = dbCtx.Games.Include(x => x.Tournament).FirstOrDefault(x => x.Id == game.Id);
            var tourney = dbCtx.Tournaments.FirstOrDefault(x => x.Id == tournament.Id);
            var tPlayer1 = dbCtx.TournamentPlayers.FirstOrDefault(x => x.Player.Id == p1.Id && x.Tournament.Id == tourney.Id);
            var tPlayer2 = dbCtx.TournamentPlayers.FirstOrDefault(x => x.Player.Id == p2.Id && x.Tournament.Id == tourney.Id);

            Assert.Equal(1, tPlayer1.Wins);
            Assert.Equal(1, tPlayer2.Losses);
        }

        [Fact]
        public async Task StartSinglePlayoffsGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };


            var tournament = new Tournament { GroupCount = 1, HasGroupStage = false };
            var players = new List<int> { 1, 2, 3, 4 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);
            var matchup = dbCtx.Matchups.Include(x => x.T1P1.Player).Include(x => x.T2P1.Player).FirstOrDefault(x => x.Tournament.Id == info.Tournament.Id && x.TournamentPhase == TournamentPhase.SemiFinal);

            // Act
            var newPgame = new NewPlayoffsGame {BluePlayer = matchup.T1P1.Player.Id, RedPlayer = matchup.T2P1.Player.Id, MatchupId = matchup.Id};
            var game = await gameFacade.CreatePlayoffsGame(newPgame);

            // Assert

            var pgame = dbCtx.Games.FirstOrDefault(x =>
                x.BP1.Id == newPgame.BluePlayer && x.RP1.Id == newPgame.RedPlayer);

            Assert.NotNull(pgame);
            Assert.Null(pgame.EndDate);
        }

        [Fact]
        public async Task FinishSinglePlayoffsGameTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var gameFacade = new GameFacade(dbCtx);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };


            var tournament = new Tournament { GroupCount = 1, HasGroupStage = false };
            var players = new List<int> { 1, 2, 3, 4 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);
            var matchup = dbCtx.Matchups.Include(x => x.T1P1.Player).Include(x => x.T2P1.Player).FirstOrDefault(x => x.Tournament.Id == info.Tournament.Id && x.TournamentPhase == TournamentPhase.SemiFinal);

            // Act
            var newPgame = new NewPlayoffsGame { BluePlayer = matchup.T1P1.Player.Id, RedPlayer = matchup.T2P1.Player.Id, MatchupId = matchup.Id };
            var game = await gameFacade.CreatePlayoffsGame(newPgame);

            var result = new GameResult {BlueScore = 10, RedScore = 8, GameId = game.Id};
            var finish = await gameFacade.FinishGame(result);
            // Assert

            var pgame = dbCtx.Games.FirstOrDefault(x => x.BP1.Id == newPgame.BluePlayer && x.RP1.Id == newPgame.RedPlayer);
            var matchupRepick = dbCtx.Matchups.FirstOrDefault(x => x.Id == matchup.Id);

            Assert.NotNull(pgame.EndDate);
            Assert.Equal(1, matchupRepick.Wins1);
            Assert.Equal(0, matchupRepick.Wins2);
        }

        [Fact]
        public async Task FinishSinglePlayoffsGameWithAdvanceTest()
        {

        }




    }
}
     