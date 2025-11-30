// Models/Interfaces/IUnitOfWork.cs
using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // CORE REPOSITORIES
        IUserRepository Users { get; }
        IQuizRepository Quiz { get; }
        IResultRepository Results { get; }
        ISystemRepository Systems { get; }
        IClientKeyRepository ClientKeys { get; }

        // SOCIAL & RANKING REPOSITORIES (Đã tách nhỏ)
        // Nếu ISocialRepository chỉ là base, bạn nên thêm các repo chuyên biệt này:

        // Cần cho CommentController
        ICommentRepository Comments { get; }

        // Cần cho Quản lý lịch sử đăng nhập
        ILoginSessionRepository LoginSessions { get; }

        // Cần cho tính năng Thành tựu
        IAchievementsRepository Achievements { get; }

        // Nếu bạn muốn giữ lại ISocialRepository cho các chức năng chung khác:
        ISocialRepository Social { get; }

        /// <summary>
        /// Lưu tất cả thay đổi từ các Repositories vào database.
        /// </summary>
        Task<int> CompleteAsync();
    }
}