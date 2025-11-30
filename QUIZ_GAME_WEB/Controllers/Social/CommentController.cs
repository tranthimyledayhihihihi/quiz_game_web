using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Social_RankingModels;
using QUIZ_GAME_WEB.Models.InputModels; // Cần cho các Input Model
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; // Cần cho IEnumerable

namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/social/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu xác thực JWT cho tất cả các hành động cần tương tác
    public class CommentController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public CommentController(QuizGameContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------------
        // 1. GET: Lấy danh sách bình luận (Cho một Quiz, Câu hỏi, v.v.)
        // ------------------------------------------------------------------
        // {entityType}: Loại đối tượng (ví dụ: 'Quiz', 'Question')
        // {entityId}: ID của đối tượng
        [HttpGet("{entityType}/{entityId}")]
        [AllowAnonymous] // Cho phép người dùng chưa đăng nhập xem bình luận
        public async Task<IActionResult> GetComments(string entityType, int entityId)
        {
            // SỬA: Dùng EntityType và RelatedEntityID
            var comments = await _context.Comments
                .Where(c => c.EntityType == entityType && c.RelatedEntityID == entityId)
                .OrderByDescending(c => c.NgayTao)
                .Select(c => new
                {
                    c.CommentID,
                    c.NoiDung,
                    c.NgayTao,
                    // Giả định DbSet NguoiDung có tên là NguoiDungs
                    UserTenDangNhap = c.User.TenDangNhap
                })
                .ToListAsync();

            if (!comments.Any())
            {
                return NotFound("Không tìm thấy bình luận nào cho đối tượng này.");
            }

            return Ok(comments);
        }

        // ------------------------------------------------------------------
        // 2. POST: Tạo bình luận mới
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var newComment = new Comment
            {
                UserID = userId,
                // SỬA: Đồng bộ tên thuộc tính
                EntityType = model.EntityType,
                RelatedEntityID = model.RelatedEntityID,
                NoiDung = model.NoiDung,
                NgayTao = DateTime.Now,
            };

            // SỬA: Đã thêm using System.Threading.Tasks;
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { Message = "Bình luận đã được tạo.", CommentId = newComment.CommentID });
        }


        // ------------------------------------------------------------------
        // 3. PUT: Cập nhật nội dung bình luận
        // ------------------------------------------------------------------
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] CommentUpdateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound("Không tìm thấy bình luận.");
            }

            // Chỉ cho phép chủ sở hữu bình luận sửa
            if (comment.UserID != userId)
            {
                return Forbid(); // 403 Forbidden
            }

            comment.NoiDung = model.NoiDung;
            // Trong Entity Model bạn không có NgayCapNhat, nhưng nên có để theo dõi
            // Nếu muốn thêm, bạn cần cập nhật lại Entity Model
            // comment.NgayCapNhat = DateTime.Now; 

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Bình luận đã được cập nhật." });
        }

        // ------------------------------------------------------------------
        // 4. DELETE: Xóa bình luận
        // ------------------------------------------------------------------
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound("Không tìm thấy bình luận.");
            }

            // Chỉ cho phép chủ sở hữu xóa (hoặc Admin/Moderator)
            if (comment.UserID != userId)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}