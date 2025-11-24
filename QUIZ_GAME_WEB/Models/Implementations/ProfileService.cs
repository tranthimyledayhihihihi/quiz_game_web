// Models/Implementations/ProfileService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> UpdateProfileAsync(int userId, ProfileUpdateModel model)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            // Logic: Cập nhật thông tin cơ bản
            user.HoTen = model.HoTen;
            user.Email = model.Email;

            // Logic: Kiểm tra và Hash mật khẩu mới nếu có
            if (!string.IsNullOrEmpty(model.MatKhauMoi))
            {
                user.MatKhau = HashPassword(model.MatKhauMoi); // Sử dụng hàm HashPassword nội bộ
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateUserSettingAsync(int userId, bool amThanh, string ngonNgu)
        {
            var user = await _unitOfWork.Users.GetUserWithSettingsAndAdminInfoAsync(userId);
            if (user == null) return false;

            // Logic: Kiểm tra và tạo/cập nhật CaiDat Entity
            if (user.CaiDat == null)
            {
                user.CaiDat = new Models.CoreEntities.CaiDatNguoiDung { UserID = userId };
                // Cần thêm user.CaiDat vào DbSet nếu nó chưa được theo dõi
            }

            user.CaiDat.AmThanh = amThanh;
            user.CaiDat.NgonNgu = ngonNgu;

            _unitOfWork.Users.Update(user); // Cập nhật user sẽ cập nhật CaiDat do Entity Framework tracking
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Tái sử dụng hàm HashPassword từ AuthService
        private string HashPassword(string password)
        {
            // Dùng logic HashPassword tương tự như trong AuthService
            return $"HASHED_PASSWORD_{password}";
        }
    }
}