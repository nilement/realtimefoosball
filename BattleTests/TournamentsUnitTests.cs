using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Controllers;
using ToughBattle.Database;
using ToughBattle.Facades;
using ToughBattle.Models;
using ToughBattle.Models.Enums;
using Xunit;

namespace BattleTests
{
    public class TournamentsUnitTests
    {
        [Fact]
        public async Task CreateNoGroup2PlayersTournamentUnitTest()
        {
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };

            var tournament = new Tournament { HasGroupStage = false };
            var players = new List<int> {1, 2};

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);

            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(TournamentPhase.Final, tourney.StartingPhase);
        }

        [Fact]
        public async Task CreateNoGroup4PlayersTournamentUnitTest()
        {
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };

            var tournament = new Tournament { HasGroupStage = false };
            var players = new List<int> { 1, 2, 3, 4 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);

            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(TournamentPhase.SemiFinal, tourney.StartingPhase);
        }

        [Fact]
        public async Task CreateNoGroup8PlayersTournamentUnitTest()
        {
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };
            var p5 = new Player { Name = "p5", Wins = 0, Losses = 0, Id = 5 };
            var p6 = new Player { Name = "p6", Wins = 0, Losses = 0, Id = 6 };
            var p7 = new Player { Name = "p7", Wins = 0, Losses = 0, Id = 7 };
            var p8 = new Player { Name = "p8", Wins = 0, Losses = 0, Id = 8 };

            var tournament = new Tournament { HasGroupStage = false };
            var players = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            dbCtx.Add(p5);
            dbCtx.Add(p6);
            dbCtx.Add(p7);
            dbCtx.Add(p8);
            await dbCtx.SaveChangesAsync();
            var info = await tournamentFacade.CreateTournament(tournament, players);

            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(TournamentPhase.QuarterFinal, tourney.StartingPhase);
        }

        [Fact]
        public async Task CreateOneGroup6PlayersTournamentUnitTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };
            var p5 = new Player { Name = "p5", Wins = 0, Losses = 0, Id = 5 };
            var p6 = new Player { Name = "p6", Wins = 0, Losses = 0, Id = 6 };

            var tournament = new Tournament { GroupCount = 1, HasGroupStage = true};
            var players = new List<int> {1, 2, 3, 4, 5, 6};

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            dbCtx.Add(p5);
            dbCtx.Add(p6);
            await dbCtx.SaveChangesAsync();

            // Act
            var info = await tournamentFacade.CreateTournament(tournament, players);

            // Assert
            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal( 1, tourney.GroupCount);
            Assert.Equal(TournamentPhase.SemiFinal, tourney.StartingPhase);
            Assert.Equal(3, tourney.PlayoffTree.Count);
        }

        [Fact]
        public async Task CreateOneGroup9PlayersTournamentUnitTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            var tournament = new Tournament { GroupCount = 1, HasGroupStage = true };
            var players = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            await Arrange9Players(dbCtx);

            // Act
            var info = await tournamentFacade.CreateTournament(tournament, players);

            // Assert
            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(1, tourney.GroupCount);
            Assert.Equal(TournamentPhase.SemiFinal, tourney.StartingPhase);
            Assert.Equal(3, tourney.PlayoffTree.Count);
        }

        [Fact]
        public async Task CreateTwoGroup9PlayersTournamentUnitTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            await Arrange9Players(dbCtx);

            var tournament = new Tournament { GroupCount = 2, HasGroupStage = true };
            var players = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            var info = await tournamentFacade.CreateTournament(tournament, players);

            // Assert
            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(2, tourney.GroupCount);
            Assert.Equal(TournamentPhase.SemiFinal, tourney.StartingPhase);
            Assert.Equal(3, tourney.PlayoffTree.Count);
        }

        [Fact]
        public async Task CreateTwoGroup10PlayersTournamentUnitTest()
        {
            // Arrange
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            var tournamentFacade = new TournamentsFacade(dbCtx);

            await Arrange9Players(dbCtx);

            var tournament = new Tournament { GroupCount = 2, HasGroupStage = true };
            var players = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var info = await tournamentFacade.CreateTournament(tournament, players);

            // Assert
            var tourney = await dbCtx.Tournaments.FirstOrDefaultAsync(x => x.Id == info.Tournament.Id);
            Assert.Equal(2, tourney.GroupCount);
            Assert.Equal(TournamentPhase.QuarterFinal, tourney.StartingPhase);
            Assert.Equal(7, tourney.PlayoffTree.Count);
        }

        private async Task Arrange9Players(FoosballContext dbCtx)
        {
            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };
            var p5 = new Player { Name = "p5", Wins = 0, Losses = 0, Id = 5 };
            var p6 = new Player { Name = "p6", Wins = 0, Losses = 0, Id = 6 };
            var p7 = new Player { Name = "p7", Wins = 0, Losses = 0, Id = 7 };
            var p8 = new Player { Name = "p8", Wins = 0, Losses = 0, Id = 8 };
            var p9 = new Player { Name = "p9", Wins = 0, Losses = 0, Id = 9 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            dbCtx.Add(p5);
            dbCtx.Add(p6);
            dbCtx.Add(p7);
            dbCtx.Add(p8);
            dbCtx.Add(p9);
            await dbCtx.SaveChangesAsync();
        }

        private async Task Arrange10Players(FoosballContext dbCtx)
        {
            var p1 = new Player { Name = "p1", Wins = 0, Losses = 0, Id = 1 };
            var p2 = new Player { Name = "p2", Wins = 0, Losses = 0, Id = 2 };
            var p3 = new Player { Name = "p3", Wins = 0, Losses = 0, Id = 3 };
            var p4 = new Player { Name = "p4", Wins = 0, Losses = 0, Id = 4 };
            var p5 = new Player { Name = "p5", Wins = 0, Losses = 0, Id = 5 };
            var p6 = new Player { Name = "p6", Wins = 0, Losses = 0, Id = 6 };
            var p7 = new Player { Name = "p7", Wins = 0, Losses = 0, Id = 7 };
            var p8 = new Player { Name = "p8", Wins = 0, Losses = 0, Id = 8 };
            var p9 = new Player { Name = "p9", Wins = 0, Losses = 0, Id = 9 };
            var p10 = new Player { Name = "p9", Wins = 0, Losses = 0, Id = 10 };

            dbCtx.Add(p1);
            dbCtx.Add(p2);
            dbCtx.Add(p3);
            dbCtx.Add(p4);
            dbCtx.Add(p5);
            dbCtx.Add(p6);
            dbCtx.Add(p7);
            dbCtx.Add(p8);
            dbCtx.Add(p9);
            dbCtx.Add(p10);
            await dbCtx.SaveChangesAsync();
        }
    }
}
