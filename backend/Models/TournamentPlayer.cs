using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ToughBattle.Models
{
    [DataContract]
    public class TournamentPlayer
    {
        public Tournament Tournament { get; set; }
        [DataMember]
        public Player Player { get; set; }
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public int Wins { get; set; }
        [DataMember]
        public int Losses { get; set; }
    }
}
