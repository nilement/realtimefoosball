using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Database;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public class PlayersFacade : IPlayersFacade
    {
        private readonly FoosballContext _db;

        public PlayersFacade(FoosballContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Player>> GetPlayers()
        {
            return await _db.Players.ToListAsync();
        }
    }
}
