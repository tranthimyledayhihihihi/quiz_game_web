// Models/Interfaces/IResultRepository.cs
using QUIZ_GAME_WEB.Models.ResultsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IResultRepository : IGenericRepository<KetQua>
    {
        // Logic cho Chuỗi Ngày (Streak)
        Task<ChuoiNgay?> GetUserStreakAsync(int userId);

        // Logic cho Câu Sai (Mistake Log)
        Task AddWrongAnswerAsync(int userId, int cauHoiId);
        Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10);

        // Logic Thống kê/Báo cáo
        Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId);

        // ----------------------------------------------------
        // SỬA LỖI NẶNG NHẤT: Bổ sung kiểu trả về ThuongNgay?
        // ----------------------------------------------------
        /// <summary>
        /// Lấy bản ghi thưởng hàng ngày theo UserID và ngày.
        /// </summary>
        Task<ThuongNgay?> GetDailyRewardByDateAsync(int userId, DateTime today); // 👈 ĐÃ SỬA KIỂU TRẢ VỀ

        // Các hàm không phải async nên là void:
        void AddDailyReward(ThuongNgay newReward);
        void AddStreak(ChuoiNgay chuoiNgay);
        void Update(ChuoiNgay streak);
    }
}