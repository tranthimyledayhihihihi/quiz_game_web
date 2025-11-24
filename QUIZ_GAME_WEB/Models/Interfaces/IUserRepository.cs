// Models/Interfaces/IUserRepository.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUserRepository : IGenericRepository<NguoiDung>
    {
        Task<NguoiDung?> GetByTenDangNhapAsync(string username);
        Task<VaiTro?> GetRoleByUserIdAsync(int userId);
        Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId);
        Task<int> CountTotalUsersAsync();
        Task<bool> IsEmailInUseAsync(string email);
    }
}