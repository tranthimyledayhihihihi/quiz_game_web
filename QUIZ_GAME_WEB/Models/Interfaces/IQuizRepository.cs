// Models/Interfaces/IQuizRepository.cs
using QUIZ_GAME_WEB.Models.QuizModels;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizRepository : IGenericRepository<CauHoi>
    {
        Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId);
        Task<string?> GetCorrectAnswerAsync(int cauHoiId);
        Task<IEnumerable<ChuDe>> GetAllTopicsAsync();

        // BỔ SUNG CHỮ KÝ HÀM NÀY ĐỂ KHẮC PHỤC LỖI TRONG SERVICE
        void AddTopic(ChuDe topic);
    }
}