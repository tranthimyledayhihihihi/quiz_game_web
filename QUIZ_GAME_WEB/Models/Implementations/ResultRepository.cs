// Models/Implementations/ResultRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Linq;
using System.Threading.Tasks; // Cần thiết
using System; // 👈 ĐÃ THÊM: Cần cho DateTime
using System.Collections.Generic; // Cần cho IEnumerable

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<KetQua> và triển khai IResultRepository
    public class ResultRepository : GenericRepository<KetQua>, IResultRepository
    {
        // Truy cập Context thông qua thuộc tính Context của lớp cơ sở hoặc thuộc tính riêng
        private readonly QuizGameContext _context;

        // Constructor
        public ResultRepository(QuizGameContext context) : base(context)
        {
            // Gán context nếu GenericRepository không tự gán hoặc cần truy cập DbSet khác
            _context = context;
        }

        public async Task<ChuoiNgay?> GetUserStreakAsync(int userId)
        {
            // SỬA TÊN DbSet: ChuoiNgay -> ChuoiNgays
            return await _context.ChuoiNgays.FirstOrDefaultAsync(c => c.UserID == userId);
        }

        // CHỈ CẦN KHAI BÁO CÁC HÀM CƠ BẢN ĐỂ LẤY HOẶC THÊM ENTITY, 
        // Logic Update (tăng streak) NÊN NẰM Ở Service.
        // Tuy nhiên, nếu hàm này là yêu cầu của IResultRepository, ta chỉ giữ lại logic thao tác DB:
        // Đã bỏ hàm UpdateUserStreakAsync khỏi Repository vì nó chứa logic nghiệp vụ.

        public async Task AddWrongAnswerAsync(int userId, int cauHoiId)
        {
            var wrongAnswer = new CauSai
            {
                UserID = userId,
                CauHoiID = cauHoiId,
                NgaySai = DateTime.Now.Date
            };
            // SỬA TÊN DbSet: CauSai -> CauSais
            await _context.CauSais.AddAsync(wrongAnswer);
            // Lưu ý: Không gọi SaveChangesAsync ở đây; để UnitOfWork (Service) gọi CompleteAsync()
        }

        public async Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10)
        {
            // SỬA TÊN DbSet: CauSai -> CauSais
            return await _context.CauSais
                                 .Where(c => c.UserID == userId)
                                 .OrderByDescending(c => c.NgaySai)
                                 .Take(limit)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            // SỬA TÊN DbSet: ThongKeNguoiDung -> ThongKeNguoiDungs
            return await _context.ThongKeNguoiDungs
                                 .Where(t => t.UserID == userId && t.Ngay >= startDate.Date && t.Ngay <= endDate.Date)
                                 .OrderBy(t => t.Ngay)
                                 .ToListAsync();
        }

        // Cần thêm triển khai cho hàm IResultRepository thứ hai có tham số nullable nếu nó có trong Interface
        // Nếu không có, hàm này là trùng lặp và nên bị loại bỏ khỏi Interface.
        public Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userID, DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            // Triển khai hàm này bằng cách kiểm tra giá trị null và gọi hàm trên
            if (ngayBatDau.HasValue && ngayKetThuc.HasValue)
            {
                return GetUserDailyStatsAsync(userID, ngayBatDau.Value, ngayKetThuc.Value);
            }
            // Logic mặc định nếu không có ngày (ví dụ: lấy 30 ngày gần nhất)
            DateTime defaultStart = DateTime.Today.AddDays(-30);
            DateTime defaultEnd = DateTime.Today;
            return GetUserDailyStatsAsync(userID, defaultStart, defaultEnd);
        }

        public Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task<ChuoiNgay?> IResultRepository.GetUserStreakAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task IResultRepository.AddWrongAnswerAsync(int userId, int cauHoiId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<CauSai>> IResultRepository.GetRecentWrongAnswersAsync(int userId, int limit)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ThongKeNguoiDung>> IResultRepository.GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ThanhTuu>> IResultRepository.GetUserAchievementsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task<ThuongNgay?> IResultRepository.GetDailyRewardByDateAsync(int userId, DateTime today)
        {
            throw new NotImplementedException();
        }

        void IResultRepository.AddDailyReward(ThuongNgay newReward)
        {
            throw new NotImplementedException();
        }

        void IResultRepository.AddStreak(ChuoiNgay chuoiNgay)
        {
            throw new NotImplementedException();
        }

        void IResultRepository.Update(ChuoiNgay streak)
        {
            throw new NotImplementedException();
        }
    }
}