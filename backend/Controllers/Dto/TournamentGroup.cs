using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using ToughBattle.Models;

namespace ToughBattle.Controllers.Dto
{
    public class TournamentGroup
    {
        public int Number;
        public List<TournamentPlayer> TournamentPlayers;
    }
}
