using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToughBattle.Controllers.Dto.Google;
using ToughBattle.Database;
using ToughBattle.Models;

namespace ToughBattle.Services
{
    public class UserService : IUserService
    {
        private readonly FoosballContext _db;

        public UserService(FoosballContext ctx)
        {
            _db = ctx;
        }
        public async Task<User> RetrieveOrRegister(GoogleEmailInfo info)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.GoogleId == info.Id);
            if (user == null)
            {
                var player = new Player
                    {AvatarUrl = info.Picture, Wins = 0, Losses = 0, Name = GetNameFromEmail(info.Email)};
                var newUser = new User {Email = info.Email, GoogleId = info.Id, Player = player};
                _db.Add(player);
                _db.Add(newUser);
                _db.SaveChanges();
                return newUser;
            }

            return user;
        }

        private string GetNameFromEmail(string email)
        {
            return email.Split("@")[0];
        }
    }
}
