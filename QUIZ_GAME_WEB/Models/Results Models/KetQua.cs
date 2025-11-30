using QUIZ_GAME_WEB.Models.CoreEntities;
using System;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class KetQua
    {
        public int KetQuaID { get; set; }
        public int UserID { get; set; }
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }
        public string TrangThaiKetQua { get; set; } = null!; // đổi tên
        public DateTime ThoiGian { get; set; }

        // Navigation property
        public NguoiDung User { get; set; } = null!;
    }
}
