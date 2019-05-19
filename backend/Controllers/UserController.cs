using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToughBattle.Controllers.Dto;
using ToughBattle.Controllers.Dto.Google;
using ToughBattle.Services;

namespace ToughBattle.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public UserController(IConfiguration configuration, IHttpClientFactory httpClientFactory, IUserService userService)
        {
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _userService = userService;
        }

        public IConfiguration Configuration { get; }
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserService _userService;

        [HttpPost]
        public IActionResult Login()
        {
            return Ok(CreateToken("retard", "gl"));
        }

        //      https://accounts.google.com/o/oauth2/auth?response_type=code&redirect_uri=http://localhost:5001/api/user/googlecallback&scope=https://www.googleapis.com/auth/userinfo.email%20https://www.googleapis.com/auth/userinfo.profile&client_id=658769649312-kllbgvtueqlchuqls54aq0k5i12hobs3.apps.googleusercontent.com
        //      https://www.googleapis.com/oauth2/v2/userinfo?access_token=ya29.Glu9Bn09cDk6ix74Q91EvK3fEfmUK6cDdYdsaqTRVpgUX-AFe66y9_xhDdlDI86Bk9OOUxR07YuZ_6pxfdnEEoPDP0Rl8UkO01ocuFdOiDDd-YvFdbQrJ-1t0a5u
        [Route("googleCallback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            var googleUser = await GetGoogleInfo(code);
            var user = await _userService.RetrieveOrRegister(googleUser);
            var token = CreateToken(user.Id.ToString(), user.Role.ToString());
            return Redirect(Configuration.GetSection("Credentials:Google:GetbackUrl").Value + token);
        }

        private string CreateToken(string userid, string claim)
        {
            var signingKey = Encoding.ASCII.GetBytes(Configuration.GetSection("Credentials:App:Secret").Value);
            var expiryDuration = int.Parse(Configuration.GetSection("Credentials:App:Expiry").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,              // Not required as no third-party is involved
                Audience = null,            // Not required as no third-party is involved
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expiryDuration),
                Subject = new ClaimsIdentity(new List<Claim>{
                    new Claim("damn", "yes"),
                    new Claim("userid", userid),
                    new Claim(ClaimTypes.Role, claim),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);
            return token;
        }

        private async Task<GoogleEmailInfo> GetGoogleInfo(string code)
        {
            var values = new Dictionary<string, string>();
            values.Add("code", code);
            values.Add("client_id", Configuration.GetSection("Credentials:Google:ClientId").Value);
            values.Add("client_secret", Configuration.GetSection("Credentials:Google:Secret").Value);
            values.Add("grant_type", "authorization_code");
            values.Add("redirect_uri", Configuration.GetSection("Credentials:Google:RedirectUrl").Value);
            var googleRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v4/token") { Content = new FormUrlEncodedContent(values) };
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(googleRequest);
            var lul = JsonConvert.DeserializeObject<GoogleResponse>(await response.Content.ReadAsStringAsync());
            var detailsRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + lul.AccessToken);
            var details = await client.SendAsync(detailsRequest);
            return JsonConvert.DeserializeObject<GoogleEmailInfo>(await details.Content.ReadAsStringAsync());
        }
    }
}
