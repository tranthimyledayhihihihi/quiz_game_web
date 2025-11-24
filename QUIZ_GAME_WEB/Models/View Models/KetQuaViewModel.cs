// Models/ViewModels/KetQuaViewModel.cs
using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    // Model này dùng để hiển thị kết quả sau khi chơi
    public class KetQuaViewModel
    {
        public int KetQuaID { get; set; }
        public int UserID { get; set; }
        public string TenNguoiDung { get; set; } = null!;

        // KHẮC PHỤC LỖI SERVICE: Dùng tên đồng nghĩa hoặc bổ sung
        public int DiemDatDuoc { get; set; } // Bổ sung để khớp với Service
        public int SoCauTraLoiDung { get; set; } // Bổ sung để khớp với Service

        // Thuộc tính cơ sở (có thể dùng thay thế)
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }
        public DateTime ThoiGian { get; set; }
        public string ThongBao { get; set; } = null!; // "Chúc mừng bạn đã thắng!"

        // Có thể thêm chi tiết các câu trả lời sai
        public List<CauSaiChiTietModel> ChiTietSai { get; set; } = new List<CauSaiChiTietModel>();
    }

    // Model phụ trợ cho chi tiết câu sai (nếu chưa có)
    public class CauSaiChiTietModel
    {
        public string NoiDungCauHoi { get; set; } = null!;
        public string DapAnChinhXac { get; set; } = null!;
        public string DapAnDaChon { get; set; } = null!;
    }
}