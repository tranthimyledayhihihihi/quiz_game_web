// Models/Implementations/LoginSessionRepository.cs

using Microsoft.EntityFrameworkCore; // Cần cho ToListAsync, CountAsync, FirstOrDefaultAsync
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // Cần cho PhienDangNhap
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Linq; // 👈 CẦN THIẾT cho Where, OrderByDescending
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Giả định GenericRepository đã được tạo và có constructor phù hợp
    public class LoginSessionRepository : GenericRepository<PhienDangNhap>, ILoginSessionRepository
    {
        // Khai báo lại _context để dễ dàng truy cập DbSet trong Repository
        private readonly QuizGameContext _context;

        public LoginSessionRepository(QuizGameContext context) : base(context)
        {
            _context = context; // Gán Context
        }

        // ------------------------------------------------------------------
        // TRIỂN KHAI GET LOGIN HISTORY (Đã sửa lỗi OrderByDescending và UserID)
        // ------------------------------------------------------------------
        public async Task<IEnumerable<PhienDangNhap>> GetLoginHistoryAsync(int userId, int limit)
        {
            // SỬA LỖI: Sử dụng tên DbSet đúng (PhienDangNhaps) và gọi ToListAsync
            return await _context.PhienDangNhaps
                         // Lỗi UserID được khắc phục vì using System.Linq; đã được thêm
                         .Where(p => p.UserID == userId)
                         .OrderByDescending(p => p.ThoiGianDangNhap) // Lỗi OrderByDescending được khắc phục
                         .Take(limit)
                         .ToListAsync();
        }

        // ------------------------------------------------------------------
        // TRIỂN KHAI CÁC HÀM CÒN LẠI
        // ------------------------------------------------------------------

        public async Task<PhienDangNhap?> GetLastLoginSessionAsync(int userId)
        {
            return await _context.PhienDangNhaps
                                 .Where(p => p.UserID == userId)
                                 .OrderByDescending(p => p.ThoiGianDangNhap)
                                 .FirstOrDefaultAsync();
        }

        public async Task<int> CountUserLoginAttemptsAsync(int userId)
        {
            return await _context.PhienDangNhaps.CountAsync(p => p.UserID == userId);
        }
    }
}