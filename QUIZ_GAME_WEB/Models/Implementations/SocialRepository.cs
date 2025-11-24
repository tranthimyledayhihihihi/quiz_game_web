// Models/Implementations/SocialRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.Social_RankingModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class SocialRepository : GenericRepository<BXH>, ISocialRepository
    {
        public SocialRepository(QuizGameContext context) : base(context) { }

        public async Task<IEnumerable<BXH>> GetTopRankingAsync(string type, int topCount)
        {
            // Logic: Lấy Top BXH theo loại (Tuan/Thang)
            IQueryable<BXH> query = _context.BXH;

            if (type.Equals("Tuan", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(b => b.DiemTuan).ThenBy(b => b.HangTuan);
            }
            else if (type.Equals("Thang", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(b => b.DiemThang).ThenBy(b => b.HangThang);
            }
            // Include User để hiển thị tên
            return await query.Include(b => b.User).Take(topCount).ToListAsync();
        }

        public async Task<BXH?> GetUserRankingEntryAsync(int userId)
        {
            return await _context.BXH.FirstOrDefaultAsync(b => b.UserID == userId);
        }

        public async Task UpdateOrInsertOnlineStatusAsync(int userId, string status)
        {
            // Logic: Cập nhật thời gian online cuối cùng
            var onlineEntry = await _context.NguoiDungOnline.FirstOrDefaultAsync(o => o.UserID == userId);
            if (onlineEntry == null)
            {
                await _context.NguoiDungOnline.AddAsync(new NguoiDungOnline { UserID = userId, TrangThai = status, ThoiGianCapNhat = DateTime.Now });
            }
            else
            {
                onlineEntry.TrangThai = status;
                onlineEntry.ThoiGianCapNhat = DateTime.Now;
                _context.NguoiDungOnline.Update(onlineEntry);
            }
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment); // Giả sử có DbSet<Comment>
        }

        public async Task<int> CountUsersOnlineAsync()
        {
            // Giả sử "Online" được định nghĩa là trong 5 phút gần nhất
            var threshold = DateTime.Now.AddMinutes(-5);
            return await _context.NguoiDungOnline.CountAsync(o => o.ThoiGianCapNhat >= threshold);
        }
    }
}