// Models/Interfaces/IUnitOfWork.cs
namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Khai báo tất cả các Repositories chuyên biệt
        IUserRepository Users { get; }
        IQuizRepository Quiz { get; }
        IResultRepository Results { get; }
        ISocialRepository Social { get; }
        ISystemRepository Systems { get; }
        IClientKeyRepository ClientKeys { get; }

        Task<int> CompleteAsync(); // Hàm lưu thay đổi vào DB
    }
}