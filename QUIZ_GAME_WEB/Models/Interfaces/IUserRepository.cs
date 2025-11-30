// Models/Interfaces/IUserRepository.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Kế thừa các hàm CRUD cơ bản (ví dụ: Add, Update, Remove) từ IGenericRepository<NguoiDung>
    public interface IUserRepository : IGenericRepository<NguoiDung>
    {
        // ----------------------------------------------------
        // CÁC HÀM TRUY VẤN NGUOITDUNG
        // ----------------------------------------------------
        Task<NguoiDung?> GetByTenDangNhapAsync(string username);
        Task<bool> IsUsernameOrEmailInUseAsync(string username, string email);
        Task<int> CountTotalUsersAsync();

        // ----------------------------------------------------
        // CÁC HÀM LIÊN QUAN ĐẾN ROLE & ADMIN (Giữ lại trong User Repo)
        // ----------------------------------------------------

        // Lấy vai trò của người dùng
        Task<VaiTro?> GetRoleByUserIdAsync(int userId);

        // Lấy thông tin vai trò theo ID (cho UpdateUserRole)
        Task<VaiTro?> GetRoleByIdAsync(int roleId);

        // Lấy bản ghi Admin Entry theo UserID
        Task<Admin?> GetAdminEntryByUserIdAsync(int userId);

        // HÀM THAO TÁC ADMIN (Giải quyết lỗi 'AddAdminEntry' not found)
        /// <summary>
        /// Thêm một bản ghi Admin mới vào bảng Admin.
        /// Sử dụng _context.Admins.Add(...) trong triển khai.
        /// </summary>
        void AddAdminEntry(Admin entry); // Khai báo là void hoặc Task (nếu dùng AddAsync)

        // ----------------------------------------------------
        // CÁC HÀM TRUY VẤN PHỨC HỢP
        // ----------------------------------------------------
        Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId);
        void Update(Admin adminEntry);
        Task<bool> IsEmailInUseAsync(string email);
    }
}