// Models/Quiz Models/CauHoi.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần thiết cho Navigation

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class CauHoi
    {
        [Key]
        public int CauHoiID { get; set; }

        [Required]
        [ForeignKey(nameof(ChuDe))] // Sử dụng nameof() là cách làm tốt hơn
        public int ChuDeID { get; set; }

        [Required]
        [ForeignKey(nameof(DoKho))]
        public int DoKhoID { get; set; }

        [Required]
        [MaxLength(500)]
        public string NoiDung { get; set; } = null!; // Khắc phục Non-nullable Warning

        [MaxLength(255)]
        [Required] // Giả định đáp án A, B, C, D là bắt buộc nếu có 4 lựa chọn
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
        public string DapAnDung { get; set; } = null!; // KHẮC PHỤC: Trong DB là NOT NULL

        [MaxLength(255)]
        public string? HinhAnh { get; set; } // Bổ sung: Dựa trên lỗi ContentManagementService

        public DateTime NgayTao { get; set; } = DateTime.Now; // Bổ sung: Cho Admin/Dashboard

        // Thuộc tính điều hướng (Navigation Properties)
        public virtual ChuDe ChuDe { get; set; } = null!;
        public virtual DoKho DoKho { get; set; } = null!;

        // Quan hệ 1:N với CauSai (Log lỗi)
        public virtual ICollection<CauSai> CauSais { get; set; } = new HashSet<CauSai>();
    }
}