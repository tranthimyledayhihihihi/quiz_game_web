// Models/InputModels/AnswerSubmitModel.cs (ĐÃ SỬA LỖI LOGIC VÀ ĐỒNG BỘ)
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class AnswerSubmitModel
    {
        // 1. BỔ SUNG: ID Phiên làm bài (Bắt buộc cho logic Service)
        [Required(ErrorMessage = "ID phiên làm bài là bắt buộc.")]
        public int QuizAttemptID { get; set; }

        [Required]
        public int CauHoiID { get; set; }

        // 2. ĐỔI TÊN: Thay DapAnChon thành DapAnDaChon để khớp với Service
        [Required(ErrorMessage = "Đáp án đã chọn là bắt buộc.")]
        [StringLength(10)]
        public string DapAnDaChon { get; set; } = null!; // Ví dụ: "A", "B", "C", "D"
    }
}