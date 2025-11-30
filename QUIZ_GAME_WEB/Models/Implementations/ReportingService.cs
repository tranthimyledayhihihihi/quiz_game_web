// Models/Implementations/ReportingService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần cho ThongKeNguoiDung Entity
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Cần cho DateTime

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Giả định: Các Models phụ trợ (BaoCaoModel, ChiTietNgayModel, ThongKeViewModel) nằm trong ViewModels

        public ReportingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<BaoCaoModel> GenerateReportAsync(BaoCaoRequestModel request)
        {
            // Lấy dữ liệu thống kê theo ngày (ThongKeNguoiDung Entity)
            // Giả định hàm GetUserDailyStatsAsync lấy ThongKeNguoiDung Entity
            var stats = await _unitOfWork.Results.GetUserDailyStatsAsync(request.UserID, request.NgayBatDau, request.NgayKetThuc);

            // Logic 1: Tính toán tổng hợp
            int tongSoTran = stats.Sum(s => s.SoTran);
            float tongSoCauDung = stats.Sum(s => s.SoCauDung);
            // Giả định mỗi trận có 10 câu
            float tongSoCauHoi = tongSoTran * 10;
            float tyLeDungTB = tongSoCauHoi > 0 ? tongSoCauDung / tongSoCauHoi : 0;

            // Logic 2: Ánh xạ sang BaoCaoModel
            return new BaoCaoModel
            {
                TieuDe = $"Báo cáo hoạt động từ {request.NgayBatDau:dd/MM} đến {request.NgayKetThuc:dd/MM}",

                // Khắc phục lỗi Property Missing trong BaoCaoModel.cs
                TongSoLanChoi = tongSoTran,
                TyLeTraLoiDungTrungBinh = tyLeDungTB,

                // Ánh xạ chi tiết (Giả sử ChiTietNgayModel có sẵn)
                ChiTietTheoNgay = stats.Select(s => new ChiTietNgayModel
                {
                    Ngay = s.Ngay,
                    // Giả định ThongKeNguoiDung có trường DiemTrungBinh
                    DiemDatDuoc = s.DiemTrungBinh,
                    SoLuotChoi = s.SoTran
                }).ToList()
            };
        }

        public async Task<ThongKeViewModel> GetUserPerformanceStatsAsync(int? chuDeId)
        {
            // Logic: Phân tích hiệu suất người dùng (Ví dụ: So sánh tỉ lệ đúng của user với TB cộng đồng)
            // Lấy dữ liệu từ Result Repository

            // Giả lập trả về ViewModel
            return new ThongKeViewModel
            {
                TenThongKe = "Hiệu suất chính",
                Labels = new List<string> { "Tổng", "Tuần", "Tháng" },
                DataSeries = new List<ThongKeSeries> {
                    new ThongKeSeries { TenSeries = "Tỉ lệ đúng (%)", Values = new List<float> { 0.75f, 0.80f, 0.70f } }
                }
            };
        }
    }
}