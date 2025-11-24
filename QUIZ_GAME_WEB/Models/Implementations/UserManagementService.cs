// Models/Implementations/AdminServices/UserManagementService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManagementService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // ... (Các hàm GetAllUsersForAdminAsync đã viết ở câu trả lời trước) ...

        public async Task<bool> ToggleUserLockoutAsync(int userId, bool isLocked)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            // Logic: Không cho phép khóa SuperAdmin (Logic nghiệp vụ)
            var role = await _unitOfWork.Users.GetRoleByUserIdAsync(userId);
            if (role != null && role.TenVaiTro == "SuperAdmin") return false;

            user.TrangThai = !isLocked;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId)
        {
            // Logic 1: Kiểm tra User và Role hợp lệ
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var role = await _unitOfWork.Users.GetRoleByIdAsync(newRoleId); // Giả sử có hàm GetRoleByIdAsync
            if (user == null || role == null) return false;

            // Logic 2: Tìm hoặc tạo entry trong bảng Admin
            var adminEntry = await _unitOfWork.Users.GetAdminEntryByUserIdAsync(userId); // Giả sử có hàm này

            if (adminEntry == null)
            {
                // Tạo entry Admin mới
                _unitOfWork.Users.AddAdminEntry(new Admin { UserID = userId, VaiTroID = newRoleId, TrangThai = true });
            }
            else
            {
                // Cập nhật Role
                adminEntry.VaiTroID = newRoleId;
                _unitOfWork.Users.Update(adminEntry);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ... (Thực thi các hàm còn lại)
    }
}