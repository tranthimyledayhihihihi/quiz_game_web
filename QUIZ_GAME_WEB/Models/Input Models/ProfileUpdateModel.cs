// Models/InputModels/ProfileUpdateModel.cs
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class ProfileUpdateModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        public string? HoTen { get; set; } // Cho phép cập nhật HoTen

        [MaxLength(255)]
        public string? AnhDaiDien { get; set; }

        // ====== THUỘC TÍNH BỔ SUNG ĐỂ KHẮC PHỤC LỖI SERVICE ======

        // 1. Email (Khắc phục lỗi ProfileService.cs)
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        // 2. Mật khẩu mới (Khắc phục lỗi ProfileService.cs)
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string? MatKhauMoi { get; set; }
    }
}