using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Text.Json.Serialization; // ✅ Cần cho [JsonIgnore]

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class CauHoi
    {
        [Key]
        public int CauHoiID { get; set; }

        [Required]
        [ForeignKey(nameof(ChuDe))]
        public int ChuDeID { get; set; }

        [Required]
        [ForeignKey(nameof(DoKho))]
        public int DoKhoID { get; set; }

        [Required]
        [MaxLength(500)]
        public string NoiDung { get; set; } = null!;

        [MaxLength(255)]
        [Required]
        public string DapAnA { get; set; } = null!;
        [MaxLength(255)]
        [Required]
        public string DapAnB { get; set; } = null!;
        [MaxLength(255)]
        [Required]
        public string DapAnC { get; set; } = null!;
        [MaxLength(255)]
        [Required]
        public string DapAnD { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string DapAnDung { get; set; } = null!;

        [MaxLength(255)]
        public string? HinhAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // ===============================================
        // ✅ BỔ SUNG KHÓA NGOẠI CHO UGC (Quiz Tuy Chỉnh)
        // ===============================================

        /// <summary>
        /// Khóa Ngoại đến QuizTuyChinh nếu câu hỏi này là một phần của đề xuất UGC.
        /// Cho phép NULL vì câu hỏi cũng có thể là câu hỏi gốc.
        /// </summary>
        [ForeignKey(nameof(QuizTuyChinh))]
        public int? QuizTuyChinhID { get; set; }

        // ===============================================
        // Thuộc tính điều hướng (Navigation Properties)
        // ===============================================

        public virtual ChuDe ChuDe { get; set; } = null!;
        public virtual DoKho DoKho { get; set; } = null!;

        /// <summary>
        /// Thuộc tính điều hướng đến QuizTuyChinh mà câu hỏi này thuộc về.
        /// </summary>
        [JsonIgnore] // Ngăn chặn vòng lặp JSON
        public virtual QuizTuyChinh? QuizTuyChinh { get; set; } // 1:N với QuizTuyChinh

        // Quan hệ 1:N với CauSai (Log lỗi)
        public virtual ICollection<CauSai> CauSais { get; set; } = new HashSet<CauSai>();
    }
}