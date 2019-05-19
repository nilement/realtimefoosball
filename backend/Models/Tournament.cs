using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ToughBattle.Models.Enums;

namespace ToughBattle.Models
{
    public class Tournament
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupCount { get; set; }
        public bool HasEnded { get; set; }
        public bool HasGroupStage { get; set; }
        public DateTime StartDate { get; set; }
        public int PlayoffMatchupsCount { get; set; }
        public List<Matchup> PlayoffTree { get; set; }
        public TournamentPhase StartingPhase { get; set; }
    }
}
