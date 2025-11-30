using QUIZ_GAME_WEB.Models.CoreEntities; // Cần cho Entity PhienDangNhap
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Kế thừa các hàm CRUD cơ bản từ IGenericRepository<PhienDangNhap>
    public interface ILoginSessionRepository : IGenericRepository<PhienDangNhap>
    {
        // ------------------------------------------------------------------
        // CÁC HÀM CHUYÊN BIỆT
        // ------------------------------------------------------------------

        /// <summary>
        /// Lấy lịch sử đăng nhập gần nhất của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="limit">Số lượng bản ghi tối đa muốn lấy.</param>
        /// <returns>Danh sách các phiên đăng nhập gần nhất.</returns>
        Task<IEnumerable<PhienDangNhap>> GetLoginHistoryAsync(int userId, int limit);

        /// <summary>
        /// Lấy phiên đăng nhập cuối cùng (gần nhất) của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Phiên đăng nhập cuối cùng hoặc null.</returns>
        Task<PhienDangNhap?> GetLastLoginSessionAsync(int userId);

        /// <summary>
        /// Đếm tổng số lần đăng nhập thành công của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Tổng số phiên đăng nhập.</returns>
        Task<int> CountUserLoginAttemptsAsync(int userId);
        Task<IEnumerable<PhienDangNhap>> GetByConditionAsync(Func<object, bool> value, Func<object, object> orderBy, int limit);

        // ❌ ĐÃ XÓA HÀM GetByConditionAsync GÂY LỖI BIÊN DỊCH VÀ SAI LOGIC EF CORE
    }
}