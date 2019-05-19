using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ToughBattle.Database;
using ToughBattle.Models.Enums;

namespace ToughBattle.Models
{
    [DataContract]
    public class Matchup
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Tournament Tournament { get; set; }
        [DataMember]
        public TournamentPhase TournamentPhase { get; set; }
        public Matchup NextMatchup { get; set; }
        public bool AdvanceToUpper { get; set; }
        public bool Finished { get; set; }
        [DataMember]
        public int Wins1 { get; set; }
        [DataMember]
        public int Wins2 { get; set; }
        public GroupPlacePair UpperPair { get; set; }
        public GroupPlacePair LowerPair { get; set; }
        public bool Merged { get; set; }
        [DataMember]
        public TournamentPlayer T1P1 { get; set; }
        public TournamentPlayer T1P2 { get; set; }
        [DataMember]
        public TournamentPlayer T2P1 { get; set; }
        public TournamentPlayer T2P2 { get; set; }
        public async Task SetSingleWin(Player winner, Player loser, FoosballContext db, int winsRequired)
        {
            if (T1P1.Player.Id == winner.Id)
            {
                Wins1 += 1;
                if (Wins1 == winsRequired)
                {
                    MoveToNextStage(T1P1);
                }
                await db.SaveChangesAsync();
            }
            else
            {
                Wins2 += 1;
                if (Wins2 == winsRequired)
                {
                    MoveToNextStage(T2P1);
                }
                await db.SaveChangesAsync();
            }
        }

        private void MoveToNextStage(TournamentPlayer player)
        {
            if (TournamentPhase == TournamentPhase.Final)
            {

            }
            if (AdvanceToUpper)
            {
                NextMatchup.T1P1 = player;
            }
            else
            {
                NextMatchup.T2P1 = player;
            }
        }
    }

    public class GroupPlacePair
    {
        public int Id { get; set; }
        public int Group { get; set; }
        public int Place { get; set; }
    }
}
