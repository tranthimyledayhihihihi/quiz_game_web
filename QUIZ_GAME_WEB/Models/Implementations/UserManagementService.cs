// Models/Implementations/AdminServices/UserManagementService.cs

using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // Quan trọng cho các hàm Async
using System; // Cần thiết cho NotImplementedException

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManagementService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // ----------------------------------------------------
        // 1. LẤY DANH SÁCH NGƯỜI DÙNG CHO ADMIN DASHBOARD
        // ----------------------------------------------------
        public async Task<IEnumerable<NguoiDungAdminViewModel>> GetAllUsersForAdminAsync()
        {
            // Bước 1: Lấy danh sách người dùng cơ bản (NguoiDung)
            // Giả định GetAllAsync() đã có trong UserRepository
            var users = await _unitOfWork.Users.GetAllAsync(); 

            // Bước 2: Chuyển đổi sang ViewModel và lấy thông tin Vai trò
            var userViewModels = new List<NguoiDungAdminViewModel>();

            foreach (var user in users)
            {
                // Giả định GetRoleByUserIdAsync trả về VaiTro (hoặc null/Player)
                var role = await _unitOfWork.Users.GetRoleByUserIdAsync(user.UserID);
                string roleName = role?.TenVaiTro ?? "Player"; // Mặc định là Player

                userViewModels.Add(new NguoiDungAdminViewModel
                {
                    UserID = user.UserID,
                    TenDangNhap = user.TenDangNhap,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    VaiTro = roleName,
                    NgayDangKy = user.NgayDangKy,
                    LanDangNhapCuoi = user.LanDangNhapCuoi,
                    TrangThai = user.TrangThai
                });
            }

            return userViewModels;
        }

        // ----------------------------------------------------
        // 2. LẤY CHI TIẾT NGƯỜI DÙNG
        // ----------------------------------------------------
        public async Task<NguoiDung?> GetUserDetailByIdAsync(int userId)
        {
            // Sử dụng Repository để lấy người dùng theo ID
            // Giả định GetByIdAsync đã có trong UserRepository
            var user = await _unitOfWork.Users.GetByIdAsync(userId); 
            return user;
        }

        // ----------------------------------------------------
        // 3. LẤY LỊCH SỬ ĐĂNG NHẬP
        // ----------------------------------------------------
        public async Task<IEnumerable<PhienDangNhap>> GetUserLoginHistoryAsync(int userId, int limit = 10)
        {
            // Giả định LoginSessions Repository có hàm GetByConditionAsync
            // Lọc theo UserID, sắp xếp giảm dần theo thời gian và giới hạn số lượng
            var history = await _unitOfWork.LoginSessions
                .GetByConditionAsync(
                    session => session.UserID == userId, // Condition
                    orderBy: sessions => sessions.OrderByDescending(s => s.ThoiGianDangNhap), // Order by
                    limit: limit // Limit
                ); 
            
            return history;
        }
        
        // ----------------------------------------------------
        // 4. KHÓA/MỞ KHÓA NGƯỜI DÙNG (TOGGLE LOCKOUT)
        // ----------------------------------------------------
        public async Task<bool> ToggleUserLockoutAsync(int userId, bool isLocked)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            // Logic nghiệp vụ: Không cho phép khóa SuperAdmin 
            var role = await _unitOfWork.Users.GetRoleByUserIdAsync(userId);
            if (role != null && role.TenVaiTro == "SuperAdmin") return false;

            user.TrangThai = !isLocked;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ----------------------------------------------------
        // 5. CẬP NHẬT VAI TRÒ NGƯỜI DÙNG (UPDATE ROLE)
        // ----------------------------------------------------
        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId)
        {
            // Logic 1: Kiểm tra User và Role hợp lệ
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var role = await _unitOfWork.Users.GetRoleByIdAsync(newRoleId);
            if (user == null || role == null) return false;

            // Logic 2: Tìm hoặc tạo entry trong bảng Admin
            var adminEntry = await _unitOfWork.Users.GetAdminEntryByUserIdAsync(userId);

            if (adminEntry == null)
            {
                // Tạo entry Admin mới nếu chưa có
                _unitOfWork.Users.AddAdminEntry(new Admin { UserID = userId, VaiTroID = newRoleId, TrangThai = true });
            }
            else
            {
                // Cập nhật Role nếu đã có
                adminEntry.VaiTroID = newRoleId;
                
                // ❌ KHÔNG GỌI UPDATE TRÊN USERS REPO (GÂY LỖI CHUYỂN ĐỔI KIỂU)
                // 💡 CHỈ CẦN SỬA ĐỔI ENTITY TRONG MEMORY. EF CORE SẼ TỰ THEO DÕI.
                // _unitOfWork.Users.Update(adminEntry); // <- ĐÃ BỎ DÒNG NÀY
            }

            // Gọi CompleteAsync để lưu các thay đổi đã được theo dõi (cả AddAdminEntry và sửa đổi adminEntry)
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ----------------------------------------------------
        // 6. CÁC HÀM CHƯA TRIỂN KHAI (Chỉ để giữ nguyên Interface)
        // ----------------------------------------------------
        /*
        public Task<IEnumerable<NguoiDungAdminViewModel>> GetAllUsersForAdminAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NguoiDung?> GetUserDetailByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PhienDangNhap>> GetUserLoginHistoryAsync(int userId, int limit = 10)
        {
            throw new NotImplementedException();
        }
        */
    }
}