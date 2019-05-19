using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Database;

namespace ToughBattle.Models
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RedTeamScore { get; set; }
        public int BlueTeamScore { get; set; }
        public PlayerType PlayerType { get; set; }
        public GameType GameType { get; set; }
        public Matchup TournamentMatchup { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Player RP1 { get; set; }
        public Player RP2 { get; set; }
        public Player BP1 { get; set; }
        public Player BP2 { get; set; }
        public Tournament Tournament { get; set; }

        public async Task UpdateTournamentPlayerStats(GameResult result, int TournamentId, FoosballContext db)
        {
            var redPlayer = await db.TournamentPlayers.FirstOrDefaultAsync(x => x.Player.Id == RP1.Id && x.Tournament.Id == TournamentId);
            var bluePlayer = await db.TournamentPlayers.FirstOrDefaultAsync(x => x.Player.Id == BP1.Id && x.Tournament.Id == TournamentId);
            if (result.RedScore > result.BlueScore)
            {

                redPlayer.Wins += 1;
                bluePlayer.Losses += 1;
                await db.SaveChangesAsync();
            }
            else
            {
                redPlayer.Losses += 1;
                bluePlayer.Wins += 1;
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdatePlayerStats(GameResult result, FoosballContext db)
        {
            if (PlayerType == PlayerType.Single)
            {
                var redPlayer = await db.Players.FirstOrDefaultAsync(x => x.Id == RP1.Id);
                var bluePlayer = await db.Players.FirstOrDefaultAsync(x => x.Id == BP1.Id);
                if (result.RedScore > result.BlueScore)
                {
                    
                    redPlayer.Wins += 1;
                    bluePlayer.Losses += 1;
                    await db.SaveChangesAsync();
                }
                else
                {
                    redPlayer.Losses += 1;
                    bluePlayer.Wins += 1;
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                var redPlayer1 = await db.Players.FirstOrDefaultAsync(x => x.Id == RP1.Id);
                var redPlayer2 = await db.Players.FirstOrDefaultAsync(x => x.Id == RP2.Id);
                var bluePlayer1 = await db.Players.FirstOrDefaultAsync(x => x.Id == BP1.Id);
                var bluePlayer2 = await db.Players.FirstOrDefaultAsync(x => x.Id == BP2.Id);
                if (result.RedScore > result.BlueScore)
                {
                    redPlayer1.Wins += 1;
                    redPlayer2.Wins += 1;
                    bluePlayer1.Losses += 1;
                    bluePlayer2.Losses += 1;
                    await db.SaveChangesAsync();
                }
                else
                {
                    redPlayer1.Losses += 1;
                    redPlayer2.Losses += 1;
                    bluePlayer1.Wins += 1;
                    bluePlayer2.Wins += 1;
                    await db.SaveChangesAsync();
                }
            }
        }
    }


    public enum PlayerType{
        Single = 0,
        Pair = 1
    }

    public enum GameType
    {
        Free = 0,
        TournamentGroup = 1,
        TournamentPlayoffs = 2
    }


}