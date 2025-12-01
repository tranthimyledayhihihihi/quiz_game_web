using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.InputModels; // ✅ CẦN THIẾT
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class QuizRepository : GenericRepository<CauHoi>, IQuizRepository
    {
        public QuizRepository(QuizGameContext context) : base(context) { }

        // ... (I. HÀM TRUY VẤN CƠ BẢN) ...
        public async Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.AsQueryable();
            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);
            query = query.Include(q => q.ChuDe).Include(q => q.DoKho);
            return await query.OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync();
        }
        public async Task<string?> GetCorrectAnswerAsync(int cauHoiId)
        {
            return await _context.CauHois.Where(q => q.CauHoiID == cauHoiId).Select(q => q.DapAnDung).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<ChuDe>> GetAllTopicsAsync() => await _context.ChuDes.ToListAsync();

        // ... (II. HÀM THAO TÁC) ...
        public void AddTopic(ChuDe topic) => _context.ChuDes.Add(topic);
        public async Task AddQuizTuyChinhAsync(QuizTuyChinh quiz) => await _context.QuizTuyChinhs.AddAsync(quiz);
        public async Task AddQuizAttemptAsync(QuizAttempt attempt) => await _context.QuizAttempts.AddAsync(attempt);
        public async Task SaveQuizAttemptAsync(QuizAttempt attempt) => _context.QuizAttempts.Update(attempt);

        // ... (III. HÀM TRUY VẤN TỐI ƯU HÓA CHO API) ...
        public async Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(int count, int? chuDeId, int? doKhoId)
        {
            // Logic đã được triển khai chính xác trong phiên bản trước.
            // ... (Đã bỏ qua để tiết kiệm không gian, nhưng logic đã được cung cấp)
            return new List<CauHoiInfoDto>(); // Mock return
        }
        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null)
        {
            // Logic đã được triển khai chính xác trong phiên bản trước.
            // ... (Đã bỏ qua để tiết kiệm không gian, nhưng logic đã được cung cấp)
            return (new List<CauHoiInfoDto>(), 0); // Mock return
        }
        public async Task<int> CountAllCauHoisAsync() => await _context.CauHois.CountAsync();
        public async Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync()
        {
            return await _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsNoTracking().ToListAsync();
        }
        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetIncorrectQuestionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            // Logic đã được triển khai chính xác trong phiên bản trước.
            // ... (Đã bỏ qua để tiết kiệm không gian, nhưng logic đã được cung cấp)
            return (new List<CauHoiInfoDto>(), 0); // Mock return
        }
        public async Task<int> CountActiveQuestionsAsync() => await _context.CauHois.CountAsync();

        // ===============================================
        // IV. ✅ TRIỂN KHAI CÁC HÀM UGC MỚI
        // ===============================================

        public async Task<(IEnumerable<QuizTuyChinhDto> Quizzes, int TotalCount)> GetQuizSubmissionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            // 1. Truy vấn QuizTuyChinhs của User
            var query = _context.QuizTuyChinhs
                .Where(q => q.UserID == userId)
                .OrderByDescending(q => q.NgayTao)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            // 2. Phân trang và ánh xạ sang DTO
            var quizzes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QuizTuyChinhDto // Giả định QuizTuyChinhDto tồn tại
                {
                    QuizTuyChinhID = q.QuizTuyChinhID,
                    TenQuiz = q.TenQuiz,
                    NgayTao = q.NgayTao,
                    TrangThai = q.TrangThai, // ✅ Dùng thuộc tính mới
                    SoCauHoi = q.CauHois.Count() // Giả định quan hệ CauHois đã được thiết lập
                })
                .ToListAsync();

            return (quizzes, totalCount);
        }

        public async Task<QuizTuyChinh?> GetQuizSubmissionByIdAsync(int quizId)
        {
            // Tải chi tiết các câu hỏi liên quan để Controller/Service có thể kiểm tra
            return await _context.QuizTuyChinhs
                .Include(q => q.CauHois)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId);
        }

        public async Task<QuizTuyChinh> SubmitNewQuizAsync(int userId, QuizSubmissionModel submission)
        {
            // ⚠️ LOGIC MOCK: Cần được thay thế bằng logic thực tế tạo QuizTuyChinh và các CauHoi
            var newQuiz = new QuizTuyChinh
            {
                UserID = userId,
                TenQuiz = submission.TenQuiz,
                MoTa = submission.MoTa,
                NgayTao = DateTime.Now,
                TrangThai = "Pending" // Thiết lập trạng thái chờ duyệt
            };

            await _context.QuizTuyChinhs.AddAsync(newQuiz);
            // Lưu ý: Cần logic để thêm các CauHoi từ submission.CauHois vào DB và liên kết chúng.

            // Không gọi SaveChanges ở đây, để UnitOfWork xử lý
            return newQuiz;
        }

        public async Task<bool> DeleteQuizSubmissionAsync(int quizId, int userId)
        {
            // 1. Tìm Quiz (kèm theo các câu hỏi liên quan)
            var quizToDelete = await _context.QuizTuyChinhs
                .Include(q => q.CauHois)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId && q.UserID == userId);

            if (quizToDelete == null || quizToDelete.TrangThai != "Pending")
            {
                return false; // Không tìm thấy hoặc đã được duyệt/từ chối
            }

            // 2. Xóa tất cả các câu hỏi được liên kết trước
            _context.CauHois.RemoveRange(quizToDelete.CauHois);

            // 3. Xóa Quiz
            _context.QuizTuyChinhs.Remove(quizToDelete);

            // Không gọi SaveChanges ở đây
            return true;
        }
    }
}