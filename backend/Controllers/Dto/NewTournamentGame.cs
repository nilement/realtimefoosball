using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToughBattle.Controllers.Dto
{
    public class NewTournamentGame
    {
        public int TournamentId { get; set; }
        public int BlueTeam { get; set; }
        public int RedTeam { get; set; }
    }
}
