using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Linq;
using System.Threading.Tasks; // 👈 ĐÃ THÊM
using System; // 👈 ĐÃ THÊM
using System.Collections.Generic; // 👈 ĐÃ THÊM

// ĐỊNH NGHĨA VIEW MODEL GIẢ ĐỊNH CHO HIỆU SUẤT TỐT HƠN
// Bạn cần tạo file này trong Models/ViewModels hoặc nơi phù hợp
public class OnlineUserViewModel
{
    public int UserID { get; set; }
    public string HoTen { get; set; }
    public string TrangThai { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }
}


namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/social/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ActivityController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // 1. LẤY NGƯỜI DÙNG ONLINE
        // ===============================================
        // GET: api/social/Activity/OnlineUsers
        // Đã thay đổi kiểu trả về sang ViewModel (hoặc bạn có thể tự định nghĩa DTO)
        [HttpGet("OnlineUsers")]
        public async Task<ActionResult<IEnumerable<OnlineUserViewModel>>> GetOnlineUsers()
        {
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            // Sử dụng Projection để tối ưu hóa hiệu suất
            var onlineUsers = await _context.NguoiDungOnlines
                .Where(u => u.TrangThai == "Online" && u.ThoiGianCapNhat >= fiveMinutesAgo)
                .Select(u => new OnlineUserViewModel
                {
                    UserID = u.UserID,
                    HoTen = u.NguoiDung.HoTen, // Giả định NguoiDung là Navigation Property
                    TrangThai = u.TrangThai,
                    ThoiGianCapNhat = u.ThoiGianCapNhat
                })
                .ToListAsync();

            return Ok(onlineUsers);
        }

        // ===============================================
        // 2. CẬP NHẬT TRẠNG THÁI HOẠT ĐỘNG
        // ===============================================
        // POST: api/social/Activity/UpdateStatus/{userId}
        [HttpPost("UpdateStatus/{userId}")]
        public async Task<IActionResult> UpdateUserStatus(int userId, [FromQuery] string status = "Online")
        {
            var onlineRecord = await _context.NguoiDungOnlines
                .SingleOrDefaultAsync(o => o.UserID == userId);

            if (onlineRecord == null)
            {
                // Thêm mới nếu chưa có
                onlineRecord = new NguoiDungOnline
                {
                    UserID = userId,
                    TrangThai = status,
                    ThoiGianCapNhat = DateTime.Now
                };
                // Dùng Add/Update thay vì AddAsync/UpdateAsync vì không có hàm Async cho những hàm này của DbContext
                _context.NguoiDungOnlines.Add(onlineRecord);
            }
            else
            {
                // Cập nhật
                onlineRecord.TrangThai = status;
                onlineRecord.ThoiGianCapNhat = DateTime.Now;
                _context.NguoiDungOnlines.Update(onlineRecord);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ===============================================
        // 3. CẬP NHẬT CHUỖI NGÀY CHƠI (STREAK)
        // ===============================================
        // POST: api/social/Activity/UpdateStreak/{userId}
        [HttpPost("UpdateStreak/{userId}")]
        public async Task<IActionResult> UpdateUserStreak(int userId)
        {
            var chuoiNgay = await _context.ChuoiNgays.SingleOrDefaultAsync(c => c.UserID == userId);
            var today = DateTime.Today;

            if (chuoiNgay == null)
            {
                // Thêm mới nếu lần chơi đầu tiên
                _context.ChuoiNgays.Add(new ChuoiNgay { UserID = userId, SoNgayLienTiep = 1, NgayCapNhatCuoi = today });
            }
            else
            {
                var lastUpdateDay = chuoiNgay.NgayCapNhatCuoi.Date;

                if (lastUpdateDay == today)
                {
                    // Đã chơi hôm nay, không làm gì
                    return Ok(new { Message = "Streak đã được ghi nhận hôm nay." });
                }
                else if (lastUpdateDay == today.AddDays(-1))
                {
                    // Chơi liên tiếp
                    chuoiNgay.SoNgayLienTiep += 1;
                    chuoiNgay.NgayCapNhatCuoi = today;
                    _context.ChuoiNgays.Update(chuoiNgay);
                }
                else
                {
                    // Bị đứt chuỗi (nghỉ 2 ngày trở lên)
                    chuoiNgay.SoNgayLienTiep = 1;
                    chuoiNgay.NgayCapNhatCuoi = today;
                    _context.ChuoiNgays.Update(chuoiNgay);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Chuỗi ngày chơi đã cập nhật." });
        }
    }
}