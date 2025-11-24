// Models/Implementations/ResultRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Linq;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ResultRepository : GenericRepository<KetQua>, IResultRepository
    {
        public ResultRepository(QuizGameContext context) : base(context) { }

        public async Task<ChuoiNgay?> GetUserStreakAsync(int userId)
        {
            // Lấy Chuỗi Ngày gần nhất của user
            return await _context.ChuoiNgay.FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public async Task UpdateUserStreakAsync(int userId)
        {
            // Logic: Cần thực hiện logic nghiệp vụ trong Service, Repository chỉ thực hiện thao tác DB
            // Ở đây, giả lập cập nhật ChuoiNgay (đã được gọi bởi UnitOfWork)
            var streak = await GetUserStreakAsync(userId);
            if (streak != null)
            {
                streak.SoNgayLienTiep += 1;
                streak.NgayCapNhatCuoi = DateTime.Now;
                Update(streak);
            }
        }

        public async Task AddWrongAnswerAsync(int userId, int cauHoiId)
        {
            // Logic: Thêm một bản ghi câu trả lời sai vào DB
            var wrongAnswer = new CauSai
            {
                UserID = userId,
                CauHoiID = cauHoiId,
                NgaySai = DateTime.Now.Date
            };
            await _context.CauSai.AddAsync(wrongAnswer);
        }

        public async Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10)
        {
            // Logic: Lấy các câu sai gần nhất
            return await _context.CauSai
                                 .Where(c => c.UserID == userId)
                                 .OrderByDescending(c => c.NgaySai)
                                 .Take(limit)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            // Logic: Lấy thống kê hiệu suất theo khoảng thời gian
            return await _context.ThongKeNguoiDung
                                 .Where(t => t.UserID == userId && t.Ngay >= startDate.Date && t.Ngay <= endDate.Date)
                                 .OrderBy(t => t.Ngay)
                                 .ToListAsync();
        }
    }
}