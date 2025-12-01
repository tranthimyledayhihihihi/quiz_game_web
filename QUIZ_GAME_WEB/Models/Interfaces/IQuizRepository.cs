using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.InputModels; // ✅ CẦN THIẾT
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizRepository : IGenericRepository<CauHoi>
    {
        // ... (I. CÁC HÀM TRUY VẤN CƠ BẢN) ...
        Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId);
        Task<string?> GetCorrectAnswerAsync(int cauHoiId);
        Task<IEnumerable<ChuDe>> GetAllTopicsAsync();

        // ... (II. CÁC HÀM THAO TÁC) ...
        void AddTopic(ChuDe topic);
        Task AddQuizTuyChinhAsync(QuizTuyChinh quiz);
        Task AddQuizAttemptAsync(QuizAttempt attempt);
        Task SaveQuizAttemptAsync(QuizAttempt attempt);

        // ... (III. CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API) ...
        Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(int count, int? chuDeId, int? doKhoId);
        Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null);
        Task<int> CountAllCauHoisAsync();
        Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync();
        Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetIncorrectQuestionsByUserIdAsync(
            int userId, int pageNumber, int pageSize);
        Task<int> CountActiveQuestionsAsync();

        // ===============================================
        // IV. ✅ CÁC HÀM QUẢN LÝ QUIZ TÙY CHỈNH (UGC)
        // ===============================================

        Task<(IEnumerable<QuizTuyChinhDto> Quizzes, int TotalCount)> GetQuizSubmissionsByUserIdAsync(
            int userId, int pageNumber, int pageSize);

        Task<QuizTuyChinh?> GetQuizSubmissionByIdAsync(int quizId);

        Task<QuizTuyChinh> SubmitNewQuizAsync(int userId, QuizSubmissionModel submission);

        Task<bool> DeleteQuizSubmissionAsync(int quizId, int userId);
    }
}