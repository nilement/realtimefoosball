using System.Collections.Generic;
using ToughBattle.Models;

namespace ToughBattle.Controllers.Dto
{
    public class TournamentInfo
    {
        public Tournament Tournament { get; set; }
        public List<TournamentGroup> Groups { get; set; }
        public List<Matchup> Matchups { get; set; }
    }
}
