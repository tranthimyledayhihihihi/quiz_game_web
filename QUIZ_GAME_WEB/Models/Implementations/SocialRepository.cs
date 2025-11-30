// Models/Implementations/SocialRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class SocialRepository : GenericRepository<BXH>, ISocialRepository
    {
        private readonly QuizGameContext _context;

        public SocialRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // 1. LẤY TOP BXH
        // ----------------------------------------------------
        public async Task<IEnumerable<BXH>> GetTopRankingAsync(string type, int topCount)
        {
            IQueryable<BXH> query = _context.BXHs;

            if (type.Equals("Tuan", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(b => b.DiemTuan).ThenBy(b => b.HangTuan);
            }
            else if (type.Equals("Thang", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(b => b.DiemThang).ThenBy(b => b.HangThang);
            }

            // SỬA LỖI CS1061: Thay User bằng NguoiDung
            return await query.Include(b => b.NguoiDung).Take(topCount).ToListAsync();
        }

        // ----------------------------------------------------
        // 2. LẤY ENTRY BXH CỦA USER
        // ----------------------------------------------------
        public async Task<BXH?> GetUserRankingEntryAsync(int userId)
        {
            return await _context.BXHs.FirstOrDefaultAsync(b => b.UserID == userId);
        }

        // ----------------------------------------------------
        // 3. CẬP NHẬT/THÊM TRẠNG THÁI ONLINE
        // ----------------------------------------------------
        public async Task UpdateOrInsertOnlineStatusAsync(int userId, string status)
        {
            var onlineEntry = await _context.NguoiDungOnlines.FirstOrDefaultAsync(o => o.UserID == userId);

            if (onlineEntry == null)
            {
                await _context.NguoiDungOnlines.AddAsync(new NguoiDungOnline { UserID = userId, TrangThai = status, ThoiGianCapNhat = DateTime.Now });
            }
            else
            {
                onlineEntry.TrangThai = status;
                onlineEntry.ThoiGianCapNhat = DateTime.Now;
                _context.NguoiDungOnlines.Update(onlineEntry);
            }
        }

        // ----------------------------------------------------
        // 4. THÊM BÌNH LUẬN
        // ----------------------------------------------------
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        // ----------------------------------------------------
        // 5. ĐẾM SỐ NGƯỜI DÙNG ONLINE
        // ----------------------------------------------------
        public async Task<int> CountUsersOnlineAsync()
        {
            var threshold = DateTime.Now.AddMinutes(-5);
            return await _context.NguoiDungOnlines.CountAsync(o => o.ThoiGianCapNhat >= threshold);
        }
    }
}