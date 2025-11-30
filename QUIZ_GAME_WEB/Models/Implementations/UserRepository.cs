// Models/Implementations/UserRepository.cs
using Microsoft.EntityFrameworkCore; // Cần cho các hàm Async và truy vấn
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Implementations;
using QUIZ_GAME_WEB.Models.Interfaces;
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
        _context = context;
    }

    // ----------------------------------------------------
    // CÁC HÀM TRIỂN KHAI CHO ADMIN/ROLE
    // ----------------------------------------------------

    // TRIỂN KHAI HÀM ADDADMINENTRY (Giải quyết lỗi 'Admin' to 'NguoiDung')
    public void AddAdminEntry(Admin entry)
    {
        // SỬA LỖI: Thao tác trực tiếp trên DbSet<Admin>
        _context.Admins.Add(entry);
        // Lưu ý: DbContext.Add() không phải là hàm async, nên không cần await và hàm là void
    }

    // TRIỂN KHAI HÀM GET ADMIN ENTRY
    public async Task<Admin?> GetAdminEntryByUserIdAsync(int userId)
    {
        // Thao tác trực tiếp trên DbSet<Admin>
        return await _context.Admins
                             .FirstOrDefaultAsync(a => a.UserID == userId);
    }

    // TRIỂN KHAI HÀM GET ROLE
    public async Task<VaiTro?> GetRoleByIdAsync(int roleId)
    {
        // Thao tác trực tiếp trên DbSet<VaiTro>
        return await _context.VaiTros.FindAsync(roleId);
    }

    // TRIỂN KHAI HÀM GET ROLE BY USER ID
    public async Task<VaiTro?> GetRoleByUserIdAsync(int userId)
    {
        // Thao tác JOIN giữa Admins và VaiTros
        var role = await (from a in _context.Admins
                          join r in _context.VaiTros on a.VaiTroID equals r.VaiTroID
                          where a.UserID == userId
                          select r)
                          .FirstOrDefaultAsync();
        return role;
    }

    // ----------------------------------------------------
    // CÁC HÀM TRIỂN KHAI CHO NGUOIDUNG (Cần giữ lại)
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

    public Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public void Update(Admin adminEntry)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEmailInUseAsync(string email)
    {
        throw new NotImplementedException();
    }

    // ... (Thêm triển khai cho GetUserWithSettingsAndAdminInfoAsync và các hàm IGenericRepository<NguoiDung> khác)
}