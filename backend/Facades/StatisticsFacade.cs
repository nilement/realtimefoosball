using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Database;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public class StatisticsFacade : IStatisticsFacade
    {
        private readonly FoosballContext _db;
        public StatisticsFacade(FoosballContext dbContext)
        {
            _db = dbContext;
        }

        public List<Goalscorer> GetTopGoalscorers()
        {
            var games = _db.Games.Include(x => x.BP1).Include(x => x.RP1).Where(x => x.EndDate.HasValue);
            var goalscorers = getGoalscorersFromGames(games);
            return goalscorers.OrderByDescending(x => x.Goals).ToList();
        }

        public List<Goalscorer> GetTopGoalscorersDateRange(DateTime @from, DateTime until)
        {
            var games = _db.Games.Include(x => x.BP1).Include(x => x.RP1)
                .Where(x => x.StartDate > from && x.StartDate < until && x.EndDate.HasValue);
            var goalscorers = getGoalscorersFromGames(games);
            return goalscorers.OrderByDescending(x => x.Goals).ToList();
        }

        private List<Goalscorer> getGoalscorersFromGames(IQueryable<Game> games)
        {
            var goalscorers = new List<Goalscorer>();
            foreach (var game in games)
            {
                var bluePlayer = goalscorers.FirstOrDefault(x => x.Player.Id == game.BP1.Id);
                if (bluePlayer != null)
                {
                    bluePlayer.Goals += game.BlueTeamScore;
                }
                else
                {
                    var gs = new Goalscorer
                    {
                        Goals = game.BlueTeamScore,
                        Player = game.BP1
                    };
                    goalscorers.Add(gs);
                }
                var redPlayer = goalscorers.FirstOrDefault(x => x.Player.Id == game.RP1.Id);
                if (redPlayer != null)
                {
                    redPlayer.Goals += game.RedTeamScore;
                }
                else
                {
                    var gs = new Goalscorer
                    {
                        Goals = game.RedTeamScore,
                        Player = game.RP1
                    };
                    goalscorers.Add(gs);
                }
            }

            return goalscorers;
        }
    }
}
