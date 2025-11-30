using QUIZ_GAME_WEB.Models.Social_RankingModels; // Cần cho Entity Comment
using QUIZ_GAME_WEB.Models.Interfaces; // Cần cho IGenericRepository (Giả định)
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Giả định IGenericRepository đã được định nghĩa và chứa các hàm CRUD cơ bản
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        /// <summary>
        /// Lấy danh sách bình luận (Comment) theo loại đối tượng và ID đối tượng được tham chiếu.
        /// </summary>
        /// <param name="entityType">Loại đối tượng (ví dụ: "Quiz", "Question").</param>
        /// <param name="relatedEntityId">ID của đối tượng được bình luận.</param>
        /// <returns>Danh sách các bình luận liên quan, sắp xếp theo ngày tạo mới nhất.</returns>
        Task<IEnumerable<Comment>> GetCommentsByEntityAsync(string entityType, int relatedEntityId);

        /// <summary>
        /// Đếm tổng số bình luận cho một đối tượng cụ thể.
        /// </summary>
        /// <param name="entityType">Loại đối tượng.</param>
        /// <param name="relatedEntityId">ID của đối tượng.</param>
        /// <returns>Tổng số bình luận.</returns>
        Task<int> CountCommentsForEntityAsync(string entityType, int relatedEntityId);
    }
}