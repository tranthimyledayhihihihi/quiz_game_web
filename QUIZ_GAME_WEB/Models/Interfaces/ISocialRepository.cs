// Models/Interfaces/ISocialRepository.cs
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface ISocialRepository : IGenericRepository<BXH>
    {
        // Logic Xếp Hạng (Ranking)
        Task<IEnumerable<BXH>> GetTopRankingAsync(string type, int topCount); // Ví dụ: type="Tuan", "Thang"
        Task<BXH?> GetUserRankingEntryAsync(int userId);

        // Logic Online Status
        Task UpdateOrInsertOnlineStatusAsync(int userId, string status);
        Task<int> CountUsersOnlineAsync();

        // Logic Tương tác (Nếu có Comment)
        Task AddCommentAsync(Comment comment);
    }
}