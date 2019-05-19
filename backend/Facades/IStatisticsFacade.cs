using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToughBattle.Models;

namespace ToughBattle.Facades
{
    public interface IStatisticsFacade
    {
        List<Goalscorer> GetTopGoalscorers();
        List<Goalscorer> GetTopGoalscorersDateRange(DateTime from, DateTime until);
    }
}
