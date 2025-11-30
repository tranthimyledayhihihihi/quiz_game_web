// Models/Implementations/CommentRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Giả định CommentRepository kế thừa GenericRepository<Comment>
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly QuizGameContext _context;

        public CommentRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // Triển khai hàm chuyên biệt từ ICommentRepository
        public async Task<IEnumerable<Comment>> GetCommentsByEntityAsync(string entityType, int relatedEntityId)
        {
            return await _context.Comments
                .Where(c => c.EntityType == entityType && c.RelatedEntityID == relatedEntityId)
                .OrderByDescending(c => c.NgayTao)
                .ToListAsync();
        }

        public async Task<int> CountCommentsForEntityAsync(string entityType, int relatedEntityId)
        {
            return await _context.Comments
                .CountAsync(c => c.EntityType == entityType && c.RelatedEntityID == relatedEntityId);
        }
    }
}