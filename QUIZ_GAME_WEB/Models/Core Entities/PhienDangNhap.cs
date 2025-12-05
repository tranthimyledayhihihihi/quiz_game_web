using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("PhienDangNhap")]
    public class PhienDangNhap
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int UserID { get; set; }

        [MaxLength(500)]
        public string? Token { get; set; }

        public DateTime ThoiGianDangNhap { get; set; } = DateTime.Now;
        public DateTime? ThoiGianHetHan { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
