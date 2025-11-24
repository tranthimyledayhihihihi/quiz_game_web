// Models/Implementations/QuizRepository.cs (ĐÃ SỬA LỖI)
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // Cần thiết để tìm thấy QuizGameContext
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels; // Cần thiết cho CauHoi, ChuDe, DoKho
using System.Linq;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<CauHoi>
    public class QuizRepository : GenericRepository<CauHoi>, IQuizRepository
    {
        // Constructor phải gọi base(context) để GenericRepository nhận _context
        public QuizRepository(QuizGameContext context) : base(context) { }

        // Khắc phục lỗi thiếu hàm GetCorrectAnswerAsync
        public async Task<string?> GetCorrectAnswerAsync(int cauHoiId)
        {
            // LỖI DBSET (nếu có): Kiểm tra QuizGameContext có DbSet<CauHoi> không
            return await _context.CauHoi
                                 .Where(q => q.CauHoiID == cauHoiId)
                                 .Select(q => q.DapAnDung)
                                 .FirstOrDefaultAsync();
        }

        // Khắc phục lỗi thiếu hàm GetAllTopicsAsync
        public async Task<IEnumerable<ChuDe>> GetAllTopicsAsync() => await _context.ChuDe.ToListAsync();

        // Khắc phục lỗi thiếu hàm GetAllDifficultiesAsync
        public async Task<IEnumerable<DoKho>> GetAllDifficultiesAsync() => await _context.DoKho.ToListAsync();

        // Khắc phục lỗi thiếu hàm GetRandomQuestionsAsync
        public async Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHoi.AsQueryable();

            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);

            // Logic thực thi random
            return await query
                         .OrderBy(r => Guid.NewGuid())
                         .Take(count)
                         .ToListAsync();
        }

        // BỔ SUNG: Hàm AddTopic bị thiếu mà ContentManagementService cần
        public void AddTopic(ChuDe topic)
        {
            _context.ChuDe.Add(topic);
        }
    }
}