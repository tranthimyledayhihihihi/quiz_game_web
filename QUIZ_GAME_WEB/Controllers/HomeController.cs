using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Threading.Tasks;
using System; // 👈 ĐÃ THÊM: Cần thiết cho việc bắt lỗi Exception

// Namespace đã được đặt về QUIZ_GAME_WEB.Controllers
namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public HomeController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // 1. KIỂM TRA TRẠNG THÁI API (Smoke Test)
        // ===============================================
        // GET: api/Home
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Status = "Running",
                Service = "QuizGame API",
                Version = "1.0"
            });
        }

        // ===============================================
        // 2. KIỂM TRA KẾT NỐI DATABASE (Health Check)
        // ===============================================
        // GET: api/Home/HealthCheck
        [HttpGet("HealthCheck")]
        public async Task<IActionResult> DatabaseHealthCheck()
        {
            try
            {
                // Tùy chọn 1: Dùng CanConnectAsync() - cách hiện đại và an toàn hơn.
                // Tránh mở và đóng kết nối thủ công như OpenConnectionAsync/CloseConnectionAsync
                if (await _context.Database.CanConnectAsync())
                {
                    return Ok(new
                    {
                        DatabaseStatus = "OK",
                        Message = "API và Database đều hoạt động."
                    });
                }

                // Nếu CanConnectAsync trả về false
                return StatusCode(503, new // 503 Service Unavailable
                {
                    DatabaseStatus = "Error",
                    Message = "Database không phản hồi hoặc không khả dụng."
                });
            }
            catch (Exception ex) // Đã sửa lỗi thiếu 'using System;'
            {
                // Trả về lỗi 500 nếu có exception khi cố gắng kết nối
                return StatusCode(500, new
                {
                    DatabaseStatus = "Fatal Error",
                    Message = "Lỗi nghiêm trọng khi kiểm tra kết nối database.",
                    Detail = ex.Message
                });
            }
        }

        // ===============================================
        // 3. ENDPOINT MẶC ĐỊNH CHO ROOT PATH
        // ===============================================
        // GET: /
        [HttpGet("/")]
        public IActionResult GetRoot()
        {
            // Trả về kết quả của GetStatus
            return RedirectToAction(nameof(GetStatus));
        }
    }
}