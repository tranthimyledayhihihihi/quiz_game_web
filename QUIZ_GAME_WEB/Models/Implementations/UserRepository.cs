// Models/Implementations/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Linq;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<NguoiDung> (KHẮC PHỤC LỖI IMPLEMENTATION)
    public class UserRepository : GenericRepository<NguoiDung>, IUserRepository
    {
        // Constructor phải gọi base(context) để GenericRepository nhận context
        public UserRepository(QuizGameContext context) : base(context) { }

        // Khắc phục lỗi Method Not Implemented: GetByTenDangNhapAsync
        public async Task<NguoiDung?> GetByTenDangNhapAsync(string username)
        {
            return await _context.NguoiDung.FirstOrDefaultAsync(u => u.TenDangNhap == username);
        }

        // Khắc phục lỗi Method Not Implemented: GetRoleByUserIdAsync
        public async Task<VaiTro?> GetRoleByUserIdAsync(int userId)
        {
            // Truy vấn qua bảng Admin và VaiTro
            return await (from a in _context.Admin
                          join r in _context.VaiTro on a.VaiTroID equals r.VaiTroID
                          where a.UserID == userId
                          select r).FirstOrDefaultAsync();
        }

        // Khắc phục lỗi Method Not Implemented: CountTotalUsersAsync
        public async Task<int> CountTotalUsersAsync() => await _context.NguoiDung.CountAsync();

        // Khắc phục lỗi Method Not Implemented: IsEmailInUseAsync
        public async Task<bool> IsEmailInUseAsync(string email) => await _context.NguoiDung.AnyAsync(u => u.Email == email);

        // Khắc phục lỗi Method Not Implemented: GetUserWithSettingsAndAdminInfoAsync
        public async Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId)
        {
            return await _context.NguoiDung
                                 .Include(u => u.CaiDat)
                                 .Include(u => u.AdminInfo)
                                 .FirstOrDefaultAsync(u => u.UserID == userId);
        }
    }
}