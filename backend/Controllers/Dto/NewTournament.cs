using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughBattle.Models;

namespace ToughBattle.Controllers
{
    public class NewTournament
    {
        public Tournament Tournament { get; set; }
        public List<int> Players { get; set; }
    }
}
