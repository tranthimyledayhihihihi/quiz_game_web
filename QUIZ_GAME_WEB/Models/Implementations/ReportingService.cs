// Models/Implementations/ReportingService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<BaoCaoModel> GenerateReportAsync(BaoCaoRequestModel request)
        {
            // Logic 1: Dùng IResultRepository để lấy dữ liệu thô
            var stats = await _unitOfWork.Results.GetUserDailyStatsAsync(request.UserID, request.NgayBatDau, request.NgayKetThuc);

            // Logic 2: Tính toán các chỉ số phức tạp (KPIs)
            int tongSoLanChoi = stats.Count();
            float tyLeDungTB = tongSoLanChoi > 0 ? stats.Average(s => s.SoCauTraLoiDung / (float)s.TongSoCauHoi) : 0;

            return new BaoCaoModel
            {
                TieuDe = $"Báo cáo hoạt động từ {request.NgayBatDau:dd/MM} đến {request.NgayKetThuc:dd/MM}",
                TongSoLanChoi = tongSoLanChoi,
                TyLeTraLoiDungTrungBinh = tyLeDungTB,
                ChiTietTheoNgay = stats.Select(s => new ChiTietNgayModel
                {
                    Ngay = s.Ngay,
                    DiemDatDuoc = s.TongDiem,
                    SoLuotChoi = s.SoLuotChoi
                }).ToList()
            };
        }

        public Task<ThongKeViewModel> GetUserPerformanceStatsAsync(int? chuDeId)
        {
            // Logic: Phân tích dữ liệu từ Result Repository để tìm hiệu suất theo chủ đề
            return Task.FromResult(new ThongKeViewModel());
        }
    }
}