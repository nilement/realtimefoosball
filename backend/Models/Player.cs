using System.ComponentModel.DataAnnotations.Schema;

namespace ToughBattle.Models
{
    public class Player{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get;set; }
        public string Name { get;set; }
        public string AvatarUrl { get;set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}