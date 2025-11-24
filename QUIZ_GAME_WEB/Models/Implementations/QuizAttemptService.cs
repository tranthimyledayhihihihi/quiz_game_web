// Models/Implementations/QuizAttemptService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class QuizAttemptService : IQuizAttemptService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizAttemptService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // #1. Bắt đầu phiên làm bài
        public Task<int> StartNewQuizAttemptAsync(int userId, GameStartOptions options)
        {
            // Logic: Dùng IQuizRepository để lấy câu hỏi ngẫu nhiên và tạo phiên mới
            return Task.FromResult(1001); // Giả lập ID phiên làm bài mới
        }

        // #2. Gửi câu trả lời
        public async Task<bool> SubmitAnswerAsync(AnswerSubmitModel answer)
        {
            var correctAnswer = await _unitOfWork.Quiz.GetCorrectAnswerAsync(answer.CauHoiID);
            bool isCorrect = correctAnswer?.Equals(answer.DapAnDaChon, StringComparison.OrdinalIgnoreCase) ?? false;

            if (!isCorrect)
            {
                // Logic: Ghi nhận câu trả lời sai
                await _unitOfWork.Results.AddWrongAnswerAsync(1, answer.CauHoiID); // Giả định userId=1
            }

            // Logic: Ghi nhận câu trả lời đã nộp
            // ...

            await _unitOfWork.CompleteAsync();
            return true;
        }

        // #3. Kết thúc và tính điểm
        public Task<KetQuaViewModel> EndAttemptAndCalculateResultAsync(int attemptId)
        {
            // Logic 1: Tính toán kết quả thực tế
            int correctAnswers = 7;
            int totalQuestions = 10;

            // Logic 2: Lưu KetQua Entity (qua IResultRepository)
            // ...

            // Logic 3: Trả về ViewModel
            return Task.FromResult(new KetQuaViewModel
            {
                KetQuaID = attemptId,
                DiemDatDuoc = 150,
                SoCauTraLoiDung = correctAnswers,
                TongCauHoi = totalQuestions,
                TenNguoiDung = "User ABC"
            });
        }

        // #4. Lấy câu hỏi tiếp theo (PHƯƠNG THỨC NÀY KHẮC PHỤC LỖI)
        public Task<CauHoiViewModel?> GetNextQuestionAsync(int attemptId)
        {
            // Logic: Lấy câu hỏi tiếp theo trong phiên làm bài 
            return Task.FromResult<CauHoiViewModel?>(null);
        }
    }
}