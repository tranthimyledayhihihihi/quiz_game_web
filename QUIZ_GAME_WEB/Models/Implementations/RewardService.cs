// Models/Implementations/RewardService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class RewardService : IRewardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RewardService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId)
        {
            // Logic: Lấy danh sách thành tựu mà user đã đạt được từ bảng liên quan
            // Giả sử có bảng UserAchievement
            var userAchievements = await _unitOfWork.Results.GetUserAchievementsAsync(userId);
            return userAchievements;
        }

        public async Task<bool> CheckAndAwardDailyRewardAsync(int userId)
        {
            // Logic 1: Kiểm tra xem user đã nhận thưởng hôm nay chưa
            var today = DateTime.Now.Date;
            var rewardReceived = await _unitOfWork.Results.GetDailyRewardByDateAsync(userId, today); // Giả sử IResultRepository có hàm này

            if (rewardReceived != null)
            {
                return false; // Đã nhận rồi
            }

            // Logic 2: Trao thưởng (Thêm bản ghi ThuongNgay vào DB)
            var newReward = new ThuongNgay
            {
                UserID = userId,
                NgayNhan = today,
                PhanThuong = "100 điểm",
                DiemThuong = 100,
                TrangThaiNhan = true
            };

            _unitOfWork.Results.AddDailyReward(newReward); // Giả sử IResultRepository có hàm này
            await _unitOfWork.CompleteAsync();

            // Logic 3: Cập nhật Streak (Chuỗi Ngày)
            await UpdateUserStreak(userId);

            return true;
        }

        private async Task UpdateUserStreak(int userId)
        {
            // Logic: Kiểm tra ngày cuối cùng user đăng nhập/chơi
            var streak = await _unitOfWork.Results.GetUserStreakAsync(userId);

            if (streak == null)
            {
                // Tạo mới streak
                _unitOfWork.Results.AddStreak(new ChuoiNgay { UserID = userId, SoNgayLienTiep = 1, NgayCapNhatCuoi = DateTime.Now });
            }
            else if (streak.NgayCapNhatCuoi.Date == DateTime.Now.Date.AddDays(-1))
            {
                // Tiếp tục streak
                streak.SoNgayLienTiep++;
                streak.NgayCapNhatCuoi = DateTime.Now;
                _unitOfWork.Results.Update(streak); // Dùng Generic Update
            }
            // else: Streak bị reset (Logic này thường được thực hiện tự động hoặc phức tạp hơn)
        }
    }
}