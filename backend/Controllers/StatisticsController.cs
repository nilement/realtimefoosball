using Microsoft.AspNetCore.Mvc;
using System;
using ToughBattle.Controllers.Dto;
using ToughBattle.Facades;
using ToughBattle.Models;

namespace ToughBattle.Controllers
{
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsFacade _statisticsFacade;
        public StatisticsController(IStatisticsFacade statisticsFacade)
        {
            _statisticsFacade = statisticsFacade;
        }

        [HttpGet]
        public IActionResult MostGoals()
        {
            return Ok(_statisticsFacade.GetTopGoalscorers());
        }

        [HttpGet]
        public IActionResult TopGoalscorersInDateRange(DateRange range)
        {
            return Ok(_statisticsFacade.GetTopGoalscorersDateRange(range.From, range.Until));
        }

        [HttpGet]
        public IActionResult FastestGoals()
        {
            return Ok();
        }
    }
}
