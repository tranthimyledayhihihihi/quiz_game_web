using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels; // Chứa LoginResponseModel
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography; // Cần cho hashing
using System.Text;
using System.Threading.Tasks;

// Lưu ý: Các DTOs (DangNhapModel, DangKyModel, v.v.) được giả định đã có sẵn.

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly QuizGameContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(QuizGameContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // ===============================================
    // 🔑 API 1: ĐĂNG NHẬP (LOGIN)
    // ===============================================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DangNhapModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // 1. TRUY CẬP DB TRỰC TIẾP VÀ LẤY THÔNG TIN
        var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

        if (user == null || !user.TrangThai)
        {
            return Unauthorized(new { message = "Thông tin đăng nhập không hợp lệ hoặc tài khoản bị khóa." });
        }

        // 2. XÁC THỰC MẬT KHẨU (LOGIC NGHIỆP VỤ)
        // CHÚ Ý: Đã thay thế bằng VerifyPassword (hàm giả lập)
        if (!VerifyPassword(model.MatKhau, user.MatKhau))
        {
            return Unauthorized(new { message = "Mật khẩu không đúng." });
        }

        // 3. PHÂN QUYỀN, TẠO TOKEN VÀ CẬP NHẬT DB
        string userRole = await GetUserRoleFromDatabase(user.UserID);
        string token = GenerateJwtToken(user, userRole);

        user.LanDangNhapCuoi = DateTime.Now;
        await _context.SaveChangesAsync(); // Lưu LanDangNhapCuoi

        return Ok(new LoginResponseModel
        {
            Token = token,
            HoTen = user.HoTen,
            VaiTro = userRole
        });
    }

    // ===============================================
    // 🔑 API 2: ĐĂNG KÝ (REGISTER)
    // ===============================================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DangKyModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // 1. KIỂM TRA TÊN ĐĂNG NHẬP/EMAIL ĐÃ TỒN TẠI
        if (await _context.NguoiDung.AnyAsync(u => u.TenDangNhap == model.TenDangNhap || u.Email == model.Email))
        {
            return Conflict(new { message = "Tên đăng nhập hoặc Email đã được sử dụng." });
        }

        // 2. TẠO USER MỚI (TRUY CẬP DB TRỰC TIẾP)
        var newUser = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = HashPassword(model.MatKhau), // Hash mật khẩu
            Email = model.Email,
            HoTen = model.HoTen,
            NgayDangKy = DateTime.Now,
            TrangThai = true // Mặc định là active
        };

        await _context.NguoiDung.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return StatusCode(201, new { message = "Đăng ký thành công." });
    }

    // ===============================================
    // 🔑 API 3: ĐỔI MẬT KHẨU (CHANGE PASSWORD)
    // ===============================================
    // Cần JWT Authorize để biết ai đang đổi mật khẩu
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        // Lấy UserID từ JWT Token (an toàn hơn là lấy từ Body)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        int userId = int.Parse(userIdClaim.Value);

        var user = await _context.NguoiDung.FindAsync(userId);
        if (user == null) return NotFound("Người dùng không tồn tại.");

        if (!VerifyPassword(model.CurrentPassword, user.MatKhau))
        {
            return Unauthorized("Mật khẩu hiện tại không đúng.");
        }

        // Cập nhật mật khẩu mới
        user.MatKhau = HashPassword(model.NewPassword);
        _context.NguoiDung.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Đổi mật khẩu thành công." });
    }


    // ===============================================
    // 🛠️ HÀM HỖ TRỢ (SECURITY & DATA ACCESS)
    // ===============================================

    private string HashPassword(string password)
    {
        // THỰC TẾ: Dùng BCrypt.Net.BCrypt.HashPassword(password)
        return $"hashed_{password}_password";
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        // THỰC TẾ: Dùng BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword)
        return hashedPassword == HashPassword(inputPassword);
    }

    private async Task<string> GetUserRoleFromDatabase(int userId)
    {
        // Logic truy vấn để xác định Vai trò (Admin/Player)
        var role = await (from a in _context.Admin
                          join r in _context.VaiTro on a.VaiTroID equals r.VaiTroID
                          where a.UserID == userId
                          select r.TenVaiTro)
                          .FirstOrDefaultAsync();

        return role ?? "Player";
    }

    private string GenerateJwtToken(NguoiDung user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.TenDangNhap),
            new Claim(ClaimTypes.Role, role)
        };

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}