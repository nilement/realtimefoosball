using System.Threading.Tasks;
using ToughBattle.Controllers.Dto.Google;
using ToughBattle.Models;

namespace ToughBattle.Services
{
    public interface IUserService
    {
        Task<User> RetrieveOrRegister(GoogleEmailInfo googleId);
    }
}
