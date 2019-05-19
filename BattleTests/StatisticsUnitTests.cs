using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Database;
using ToughBattle.Facades;
using ToughBattle.Models;
using Xunit;


namespace BattleTests
{
    public class StatisticsUnitTests
    {
        [Fact]
        public async void TopGoalscorerAllTime()
        {
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            Player p1 = new Player
            {
                Name = "p1"
            };
            Player p2 = new Player
            {
                Name = "p2"
            };
            await dbCtx.AddAsync(p1);
            await dbCtx.AddAsync(p2);
            Game g1 = new Game
            {
                BP1 = p1,
                BlueTeamScore = 10,
                RP1 = p2,
                RedTeamScore = 5,
                StartDate = DateTime.Today.AddDays(-1).AddHours(-1),
                EndDate = DateTime.Today.AddDays(-1)
            };
            Game g2 = new Game
            {
                BP1 = p1,
                BlueTeamScore = 10,
                RP1 = p2,
                RedTeamScore = 8,
                StartDate = DateTime.Now.AddDays(-14).AddHours(-1),
                EndDate = DateTime.Now.AddDays(-14)
            };

            await dbCtx.AddAsync(g1);
            await dbCtx.AddAsync(g2);
            await dbCtx.SaveChangesAsync();
            var statisticsFacade = new StatisticsFacade(dbCtx);

            var scorers = statisticsFacade.GetTopGoalscorers();

            //assert
            var scorer1 = scorers.FirstOrDefault();
            var scorer2 = scorers.LastOrDefault();
            Assert.Equal(p1.Id, scorer1.Player.Id);
            Assert.Equal(20, scorer1.Goals);
            Assert.Equal(p2.Id, scorer2.Player.Id);
            Assert.Equal(13, scorer2.Goals);

        }

        [Fact]
        public async void TopGoalscorerThisWeek()
        {
            DbContextOptions<FoosballContext> opt = new DbContextOptionsBuilder<FoosballContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var dbCtx = new FoosballContext(opt);
            Player p1 = new Player
            {
                Name = "p1"
            };
            Player p2 = new Player
            {
                Name = "p2"
            };
            await dbCtx.AddAsync(p1);
            await dbCtx.AddAsync(p2);
            Game g1 = new Game
            {
                BP1 = p1,
                BlueTeamScore = 10,
                RP1 = p2,
                RedTeamScore = 5,
                StartDate = DateTime.Today.AddDays(-1).AddHours(-1),
                EndDate = DateTime.Today.AddDays(-1)
            };
            Game g2 = new Game
            {
                BP1 = p1,
                BlueTeamScore = 10,
                RP1 = p2,
                RedTeamScore = 8,
                StartDate = DateTime.Now.AddDays(-14).AddHours(-1),
                EndDate = DateTime.Now.AddDays(-14)
            };

            await dbCtx.AddAsync(g1);
            await dbCtx.AddAsync(g2);
            await dbCtx.SaveChangesAsync();
            var statisticsFacade = new StatisticsFacade(dbCtx);

            //act
            var scorers = statisticsFacade.GetTopGoalscorersDateRange(DateTime.Now.AddDays(-7), DateTime.Now);

            //assert
            var scorer1 = scorers.FirstOrDefault();
            var scorer2 = scorers.LastOrDefault();
            Assert.Equal(p1.Id, scorer1.Player.Id);
            Assert.Equal(10, scorer1.Goals);
            Assert.Equal(p2.Id, scorer2.Player.Id);
            Assert.Equal(5, scorer2.Goals);
        }
    }
}
