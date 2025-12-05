using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.InputModels;
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
        // Giả định _context được truy cập từ lớp cơ sở (GenericRepository)
        public QuizRepository(QuizGameContext context) : base(context) { }

        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN & CHUYÊN BIỆT
        // ===============================================

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

        // ===============================================
        // II. CÁC HÀM THAO TÁC (CRUD & Transaction Support)
        // ===============================================

        public void AddTopic(ChuDe topic) => _context.ChuDes.Add(topic);
        public async Task AddQuizTuyChinhAsync(QuizTuyChinh quiz) => await _context.QuizTuyChinhs.AddAsync(quiz);
        public async Task AddQuizAttemptAsync(QuizAttempt attempt) => await _context.QuizAttempts.AddAsync(attempt);
        public async Task SaveQuizAttemptAsync(QuizAttempt attempt) => _context.QuizAttempts.Update(attempt);
        public async Task AddQuizChiaSeAsync(QuizChiaSe share) => await _context.QuizChiaSes.AddAsync(share); // ✅ Thao tác Share

        // ===============================================
        // III. CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API
        // ===============================================

        public async Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(
            int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsQueryable();

            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);

            return await query
                         .OrderBy(r => Guid.NewGuid())
                         .Take(count)
                         .Select(q => new CauHoiInfoDto
                         {
                             CauHoiID = q.CauHoiID,
                             NoiDung = q.NoiDung,
                             DapAnA = q.DapAnA,
                             DapAnB = q.DapAnB,
                             DapAnC = q.DapAnC,
                             DapAnD = q.DapAnD,
                             HinhAnh = q.HinhAnh,
                             ChuDeID = q.ChuDeID,
                             TenChuDe = q.ChuDe!.TenChuDe,
                             DoKhoID = q.DoKhoID,
                             TenDoKho = q.DoKho!.TenDoKho,
                             DiemThuong = q.DoKho.DiemThuong
                         })
                         .ToListAsync();
        }

        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null)
        {
            var query = _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => q.NoiDung.Contains(keyword) || q.DapAnA.Contains(keyword) || q.DapAnB.Contains(keyword) ||
                                         q.DapAnC.Contains(keyword) || q.DapAnD.Contains(keyword));
            }
            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);

            var totalCount = await query.CountAsync();

            var questions = await query
                .OrderBy(q => q.CauHoiID)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(q => new CauHoiInfoDto
                {
                    CauHoiID = q.CauHoiID,
                    NoiDung = q.NoiDung,
                    DapAnA = q.DapAnA,
                    DapAnB = q.DapAnB,
                    DapAnC = q.DapAnC,
                    DapAnD = q.DapAnD,
                    HinhAnh = q.HinhAnh,
                    ChuDeID = q.ChuDeID,
                    TenChuDe = q.ChuDe!.TenChuDe,
                    DoKhoID = q.DoKhoID,
                    TenDoKho = q.DoKho!.TenDoKho,
                    DiemThuong = q.DoKho.DiemThuong
                })
                .ToListAsync();

            return (questions, totalCount);
        }

        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetIncorrectQuestionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            var incorrectQuestionIds = _context.CauSais.Where(cs => cs.UserID == userId).Select(cs => cs.CauHoiID).Distinct().AsQueryable();
            var totalCount = await incorrectQuestionIds.CountAsync();
            var pagedQuestionIds = await incorrectQuestionIds.OrderBy(id => id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var questions = await _context.CauHois
                .Where(q => pagedQuestionIds.Contains(q.CauHoiID))
                .Include(q => q.ChuDe).Include(q => q.DoKho)
                .Select(q => new CauHoiInfoDto
                {
                    CauHoiID = q.CauHoiID,
                    NoiDung = q.NoiDung,
                    DapAnA = q.DapAnA,
                    DapAnB = q.DapAnB,
                    DapAnC = q.DapAnC,
                    DapAnD = q.DapAnD,
                    HinhAnh = q.HinhAnh,
                    ChuDeID = q.ChuDeID,
                    TenChuDe = q.ChuDe!.TenChuDe,
                    DoKhoID = q.DoKhoID,
                    TenDoKho = q.DoKho!.TenDoKho,
                    DiemThuong = q.DoKho.DiemThuong
                })
                .ToListAsync();

            return (questions, totalCount);
        }

        public async Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync()
        {
            return await _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsNoTracking().ToListAsync();
        }

        public async Task<int> CountAllCauHoisAsync() => await _context.CauHois.CountAsync();
        public async Task<int> CountActiveQuestionsAsync() => await _context.CauHois.CountAsync();

        // ===============================================
        // IV. ✅ HÀM UGC (User-Generated Content) & SHARE
        // ===============================================

        public async Task<(IEnumerable<QuizTuyChinhDto> Quizzes, int TotalCount)> GetQuizSubmissionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            var query = _context.QuizTuyChinhs
                .Where(q => q.UserID == userId)
                .OrderByDescending(q => q.NgayTao)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var quizzes = await query
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(q => new QuizTuyChinhDto
                {
                    QuizTuyChinhID = q.QuizTuyChinhID,
                    TenQuiz = q.TenQuiz,
                    MoTa = q.MoTa,
                    NgayTao = q.NgayTao,
                    TrangThai = q.TrangThai,
                    SoCauHoi = q.CauHois.Count()
                })
                .ToListAsync();

            return (quizzes, totalCount);
        }

        public async Task<QuizTuyChinh?> GetQuizSubmissionByIdAsync(int quizId)
        {
            return await _context.QuizTuyChinhs
                .Include(q => q.CauHois).ThenInclude(c => c.ChuDe)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId);
        }

        public async Task<QuizTuyChinh> SubmitNewQuizAsync(int userId, QuizSubmissionModel submission)
        {
            var newQuiz = new QuizTuyChinh
            {
                UserID = userId,
                TenQuiz = submission.TenQuiz,
                MoTa = submission.MoTa,
                NgayTao = DateTime.Now,
                TrangThai = "Pending"
            };

            await _context.QuizTuyChinhs.AddAsync(newQuiz);
            await _context.SaveChangesAsync();

            var questionsToSubmit = submission.Questions.Select(qModel => new CauHoi
            {
                ChuDeID = qModel.ChuDeID,
                DoKhoID = qModel.DoKhoID,
                NoiDung = qModel.NoiDung,
                DapAnA = qModel.DapAnA,
                DapAnB = qModel.DapAnB,
                DapAnC = qModel.DapAnC,
                DapAnD = qModel.DapAnD,
                DapAnDung = qModel.DapAnDung,
                HinhAnh = qModel.HinhAnh,
                NgayTao = DateTime.Now,
                QuizTuyChinhID = newQuiz.QuizTuyChinhID // Gán FK
            }).ToList();

            await _context.CauHois.AddRangeAsync(questionsToSubmit);
            return newQuiz;
        }

        public async Task<bool> DeleteQuizSubmissionAsync(int quizId, int userId)
        {
            var quizToDelete = await _context.QuizTuyChinhs
                .Include(q => q.CauHois)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId && q.UserID == userId);

            if (quizToDelete == null || quizToDelete.TrangThai != "Pending")
            {
                return false;
            }

            _context.CauHois.RemoveRange(quizToDelete.CauHois);
            _context.QuizTuyChinhs.Remove(quizToDelete);
            return true;
        }

        // ===============================================
        // V. HÀM SHARE (QuizChiaSeController)
        // ===============================================

        public async Task<bool> CheckQuizOwnershipAndExistenceAsync(int quizId, int userId)
        {
            return await _context.QuizTuyChinhs
                .AnyAsync(q => q.QuizTuyChinhID == quizId && q.UserID == userId);
        }

        public async Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesBySenderAsync(int userId)
        {
            var query = _context.QuizChiaSes
                .Where(share => share.UserGuiID == userId)
                .OrderByDescending(share => share.NgayChiaSe)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var shares = await query
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserNhanID, receiver => receiver.UserID, (joined, receiver) => new QuizShareDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    QuizTuyChinhID = joined.s.QuizTuyChinhID,
                    TenQuiz = joined.q.TenQuiz,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiGui = _context.NguoiDungs.First(u => u.UserID == joined.s.UserGuiID).HoTen,
                    TenNguoiNhan = receiver.HoTen
                })
                .ToListAsync();

            return (shares, totalCount);
        }
        public async Task<QuizNgayDetailsDto?> GetTodayQuizDetailsAsync()
        {
            // Lấy QuizNgay dựa trên ngày hiện tại (DateTime.Today)
            var todayQuiz = await _context.QuizNgays
                .Where(qn => qn.Ngay.Date == DateTime.Today.Date)
                .Include(qn => qn.CauHoi) // Giả định QuizNgay có Navigation Property CauHoi
                    .ThenInclude(ch => ch!.DoKho) // Include Độ khó để lấy chi tiết
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (todayQuiz == null)
            {
                return null;
            }

            // Ánh xạ sang DTO (Giả định QuizNgayDetailsDto chứa CauHoiID và nội dung chi tiết)
            return new QuizNgayDetailsDto
            {
                QuizNgayID = todayQuiz.QuizNgayID,
                Ngay = todayQuiz.Ngay,
                CauHoiID = (int)todayQuiz.CauHoiID,
                NoiDungCauHoi = todayQuiz.CauHoi!.NoiDung,
                TenDoKho = todayQuiz.CauHoi.DoKho!.TenDoKho,
                DiemThuong = todayQuiz.CauHoi.DoKho.DiemThuong
                // Cần bổ sung các trường khác của câu hỏi (A, B, C, D) trong DTO này
            };
        }
        public async Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesByReceiverAsync(int userId)
        {
            var query = _context.QuizChiaSes
                .Where(share => share.UserNhanID == userId)
                .OrderByDescending(share => share.NgayChiaSe)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var shares = await query
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserGuiID, sender => sender.UserID, (joined, sender) => new QuizShareDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    QuizTuyChinhID = joined.s.QuizTuyChinhID,
                    TenQuiz = joined.q.TenQuiz,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiGui = sender.HoTen,
                    TenNguoiNhan = _context.NguoiDungs.First(u => u.UserID == joined.s.UserNhanID).HoTen
                })
                .ToListAsync();

            return (shares, totalCount);
        }

        public async Task<QuizShareDetailDto?> GetShareDetailByIdAsync(int shareId)
        {
            var shareDetail = await _context.QuizChiaSes
                .Where(s => s.QuizChiaSeID == shareId)
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserGuiID, sender => sender.UserID, (joined, sender) => new { joined.s, joined.q, sender })
                .Join(_context.NguoiDungs, joined => joined.s.UserNhanID, receiver => receiver.UserID, (joined, receiver) => new QuizShareDetailDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    TenNguoiGui = joined.sender.HoTen,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiNhan = receiver.HoTen,
                    QuizMetadata = new QuizTuyChinhMetadataDto
                    {
                        QuizTuyChinhID = joined.q.QuizTuyChinhID,
                        TenQuiz = joined.q.TenQuiz,
                        MoTa = joined.q.MoTa,
                        TrangThai = joined.q.TrangThai,
                        TongSoCauHoi = _context.CauHois.Count(c => c.QuizTuyChinhID == joined.q.QuizTuyChinhID)
                    }
                })
                .FirstOrDefaultAsync();

            return shareDetail;
        }

        // ===============================================
        // VI. HÀM TRUY VẤN KHÁC (Giữ lại)
        // ===============================================
        public async Task<IEnumerable<DoKho>> GetAllDifficultiesAsync() => await _context.DoKhos.ToListAsync();
    }
}