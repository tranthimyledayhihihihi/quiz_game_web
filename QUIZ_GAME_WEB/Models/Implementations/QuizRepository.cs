// Models/Implementations/QuizRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository và thực thi IQuizRepository
    public class QuizRepository : GenericRepository<CauHoi>, IQuizRepository
    {
        // Constructor gọi base(context) để nhận DbContext
        public QuizRepository(QuizGameContext context) : base(context) { }

        // ===============================================
        // CÁC HÀM TRUY VẤN ĐẶC THÙ (Specialized Queries)
        // ===============================================

        public async Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.AsQueryable(); // Sử dụng DbSet<CauHoi> (tên số nhiều)

            // Lọc theo Chủ đề (nếu có)
            if (chuDeId.HasValue)
            {
                query = query.Where(q => q.ChuDeID == chuDeId.Value);
            }
            // Lọc theo Độ khó (nếu có)
            if (doKhoId.HasValue)
            {
                query = query.Where(q => q.DoKhoID == doKhoId.Value);
            }

            // Logic Lấy ngẫu nhiên (ORDER BY NEWID() trong SQL) và giới hạn số lượng
            return await query
                         .OrderBy(r => Guid.NewGuid())
                         .Take(count)
                         .ToListAsync();
        }

        public async Task<string?> GetCorrectAnswerAsync(int cauHoiId)
        {
            // Truy vấn trực tiếp để lấy đáp án đúng
            return await _context.CauHois
                                 .Where(q => q.CauHoiID == cauHoiId)
                                 .Select(q => q.DapAnDung)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChuDe>> GetAllTopicsAsync()
        {
            // Lấy danh sách Chủ đề
            return await _context.ChuDes.ToListAsync();
        }

        public async Task<IEnumerable<DoKho>> GetAllDifficultiesAsync()
        {
            // Lấy danh sách Độ khó
            return await _context.DoKhos.ToListAsync();
        }

        // ===============================================
        // CÁC HÀM THAO TÁC ADMIN (IMPLEMENTATIONS BỊ THIẾU)
        // ===============================================

        // Thêm Chủ đề mới (được gọi từ ContentManagementService.cs)
        public void AddTopic(ChuDe topic)
        {
            _context.ChuDes.Add(topic);
        }

        public async Task<int> CountActiveQuestionsAsync()
        {
            // Logic: Đếm tổng số câu hỏi đang hoạt động (Giả sử không có trạng thái ẩn)
            return await _context.CauHois.CountAsync();
        }
    }
}