using System.Collections.Generic;
using ToughBattle.Models;

namespace ToughBattle.Controllers.Dto
{
    public class NewGame
    {
        public PlayerType PlayerType { get; set; }
        public List<int> BlueTeam { get; set; }
        public List<int> RedTeam { get; set; }
    }
}
