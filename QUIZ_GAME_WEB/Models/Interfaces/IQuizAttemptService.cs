// Models/Interfaces/IQuizAttemptService.cs
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Đặt trong thư mục Interfaces/QuizServices (hoặc Interfaces nếu bạn không dùng thư mục con)
    public interface IQuizAttemptService
    {
        // Bắt đầu một lần làm bài Quiz mới
        Task<int> StartNewQuizAttemptAsync(int userId, GameStartOptions options);

        // Xử lý nộp đáp án cho một câu hỏi
        Task<bool> SubmitAnswerAsync(AnswerSubmitModel answer);

        // Kết thúc Quiz và tính toán kết quả cuối cùng
        Task<KetQuaViewModel> EndAttemptAndCalculateResultAsync(int attemptId);

        // Lấy câu hỏi tiếp theo trong phiên làm bài
        Task<CauHoiViewModel?> GetNextQuestionAsync(int attemptId);
    }
}