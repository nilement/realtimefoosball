using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ToughBattle.Models
{
    public class Goal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Player Scorer { get; set; }
        public Player Receiver { get; set; }
        public Game Game { get; set; }
        public double Velocity { get; set; }
        public string VideoUrl { get; set; }
    }
}
