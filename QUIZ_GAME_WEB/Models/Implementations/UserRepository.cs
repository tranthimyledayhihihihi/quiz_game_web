using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Implementations;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels; // Cần cho UserProfileDto
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Giả định GenericRepository là lớp cơ sở bạn đã tạo
public class UserRepository : GenericRepository<NguoiDung>, IUserRepository
{
    // Truy cập Context thông qua thuộc tính Context của lớp cơ sở hoặc thuộc tính riêng
    private readonly QuizGameContext _context;

    public UserRepository(QuizGameContext context) : base(context) // Giả định constructor cơ sở
    {
        // Chú ý: Context này thường được truy cập thông qua lớp base để tránh lỗi "hiding inherited member"
        // Nhưng giữ nguyên cấu trúc _context = context theo yêu cầu.
        _context = context;
    }

    // ----------------------------------------------------
    // I. CÁC HÀM TRUY VẤN NGUOIDUNG CƠ BẢN
    // ----------------------------------------------------

    public async Task<NguoiDung?> GetByTenDangNhapAsync(string username)
    {
        return await _context.NguoiDungs
                                .FirstOrDefaultAsync(u => u.TenDangNhap == username);
    }

    public async Task<bool> IsUsernameOrEmailInUseAsync(string username, string email)
    {
        return await _context.NguoiDungs
                                .AnyAsync(u => u.TenDangNhap == username || u.Email == email);
    }

    public async Task<int> CountTotalUsersAsync()
    {
        return await _context.NguoiDungs.CountAsync();
    }

    public async Task<bool> IsEmailInUseAsync(string email)
    {
        return await _context.NguoiDungs
                                .AnyAsync(u => u.Email == email);
    }

    // ----------------------------------------------------
    // II. CÁC HÀM LIÊN QUAN ĐẾN ROLE & ADMIN
    // ----------------------------------------------------

    public void AddAdminEntry(Admin entry)
    {
        _context.Admins.Add(entry);
    }

    public async Task<Admin?> GetAdminEntryByUserIdAsync(int userId)
    {
        return await _context.Admins
                                .FirstOrDefaultAsync(a => a.UserID == userId);
    }

    public async Task<VaiTro?> GetRoleByIdAsync(int roleId)
    {
        return await _context.VaiTros.FindAsync(roleId);
    }

    // Lấy vai trò chính của Admin/Moderator (dựa trên bảng Admin)
    public async Task<VaiTro?> GetRoleByUserIdAsync(int userId)
    {
        var role = await (from a in _context.Admins
                          join r in _context.VaiTros on a.VaiTroID equals r.VaiTroID
                          where a.UserID == userId
                          select r)
                          .FirstOrDefaultAsync();
        return role;
    }

    public void Update(Admin adminEntry)
    {
        _context.Admins.Update(adminEntry);
    }

    // ----------------------------------------------------
    // III. CÁC HÀM TRUY VẤN PHỨC HỢP & SOCIAL
    // ----------------------------------------------------

    /// <summary>
    /// Lấy user + setting (CaiDatNguoiDung) + thông tin admin (bảng Admin)
    /// </summary>
    public async Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId)
    {
        return await _context.NguoiDungs
                                .Include(u => u.CaiDat)
                                .Include(u => u.Admin)
                                .FirstOrDefaultAsync(u => u.UserID == userId);
    }

    /// <summary>
    /// Lấy thông tin hồ sơ công khai của người dùng khác, ánh xạ sang DTO.
    /// </summary>
    /// Lấy bản ghi CaiDatNguoiDung theo UserID.
    /// </summary>
    public async Task<CaiDatNguoiDung?> GetCaiDatByUserIdAsync(int userId)
    {
        // Giả định DbSet là _context.CaiDatNguoiDungs
        return await _context.CaiDatNguoiDungs
                             .FirstOrDefaultAsync(c => c.UserID == userId);
    }

    /// <summary>
    /// Thêm bản ghi cài đặt mới.
    /// </summary>
    public void AddCaiDat(CaiDatNguoiDung setting)
    {
        _context.CaiDatNguoiDungs.Add(setting);
    }

    /// <summary>
    /// Cập nhật bản ghi cài đặt.
    /// </summary>
    public void UpdateCaiDat(CaiDatNguoiDung setting)
    {
        // EF Core sẽ theo dõi và cập nhật nếu entity đã được thay đổi.
        _context.CaiDatNguoiDungs.Update(setting);
    }
    public async Task<UserProfileDto?> GetPublicProfileAsync(int targetUserId)
    {
        // Tải các Navigation Properties cần thiết để tính toán
        var userQuery = _context.NguoiDungs
            .Include(u => u.BXHs) // Để tính điểm
            .Include(u => u.KetQuas) // Để tính số bài làm
            .Include(u => u.PhienDangNhaps) // Để kiểm tra đăng nhập cuối
            .AsNoTracking()
            .Where(u => u.UserID == targetUserId);

        var profile = await userQuery
            .Select(u => new UserProfileDto
            {
                UserID = u.UserID,
                TenDangNhap = u.TenDangNhap,
                HoTen = u.HoTen,
                AnhDaiDien = u.AnhDaiDien,
                NgayDangKy = u.NgayDangKy,

                // Tính toán các chỉ số thống kê cơ bản:
                TongSoDiem = u.BXHs.OrderByDescending(b => b.DiemThang).Select(b => b.DiemThang).FirstOrDefault(),
                TongSoQuizDaLam = u.KetQuas.Count(),

                // Các chỉ số Social (sẽ được tính trong SocialService/Repository, nhưng được placeholder ở đây):
               // SoNguoiTheoDoi = _context.TheoDois.Count(t => t.FolloweeID == targetUserId),
                //DangTheoDoi = _context.TheoDois.Count(t => t.FollowerID == targetUserId),
                IsFollowing = false // Luôn là false ở Repository, được set lại trong Controller
            })
            .FirstOrDefaultAsync();

        return profile;
    }
}