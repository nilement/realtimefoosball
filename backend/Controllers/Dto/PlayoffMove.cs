using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToughBattle.Controllers.Dto
{
    public class PlayoffMove
    {
        public int TournamentPlayerId { get; set; }
    }

    public class NewPlayoffsGame
    {
        public int MatchupId { get; set; }
        public int RedPlayer { get; set; }
        public int BluePlayer { get; set; }
    }
}
