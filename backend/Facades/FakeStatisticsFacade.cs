using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public class FakeStatisticsFacade : IStatisticsFacade
    {
        public List<Goalscorer> GetTopGoalscorers()
        {
            var players = GetPlayers();
            var list = new List<Goalscorer>();
            for (int i = 0; i < players.Count; i++)
            {
                var scorer = new Goalscorer { Player = players[i], Goals = new Random().Next(10, 100) };
                list.Add(scorer);
            }
            return list;
        }

        public List<Goalscorer> GetTopGoalscorersDateRange(DateTime from, DateTime until)
        {
            var players = GetPlayers();
            var list = new List<Goalscorer>();
            for (int i = 0; i < players.Count; i++)
            {
                var scorer = new Goalscorer { Player = players[i], Goals = new Random().Next(10, 100) };
                list.Add(scorer);
            }
            return list;
        }

        private List<Player> GetPlayers()
        {
            return new List<Player>{
                new Player{ Name = "Matas", Id = 0, AvatarUrl="matas.jpg", Wins=5,Losses=1 },
                new Player { Name = "Žygimantas", Id = 1, AvatarUrl="zygis.jpg", Wins=5,Losses=1 },
                new Player { Name = "Donatas", Id = 2, AvatarUrl="donce.jpg", Wins=5,Losses=1 },
                new Player { Name = "Audrius", Id = 3, AvatarUrl="audrius.jpg", Wins=5, Losses=1 }
            };
        }
    }
}
