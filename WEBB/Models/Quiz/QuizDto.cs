using System.Collections.Generic;

namespace WEBB.Models.Quiz
{
    public class QuizDto
    {
        public int QuizID { get; set; }
        public string TenQuiz { get; set; }
        
        // ✅ TÁI SỬ DỤNG CauHoiDto TẠI ĐÂY
        public List<CauHoiDto> CauHois { get; set; } 
    }
}