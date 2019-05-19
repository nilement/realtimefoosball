using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ToughBattle.Controllers.Dto.Google
{
    public class GoogleEmailInfo
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("verified_email")] public bool VerifiedEmail { get; set; }
        [JsonProperty("picture")] public string Picture { get; set; }
        [JsonProperty("hd")] public string Hd { get; set; }
    }
}
