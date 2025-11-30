// Models/Implementations/UnitOfWork.cs
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuizGameContext _context;

        // Khai báo Implementations (đổi từ Expression Body sang get; private set;)
        public IUserRepository Users { get; private set; }
        public IQuizRepository Quiz { get; private set; }
        public IResultRepository Results { get; private set; }
        public ISocialRepository Social { get; private set; }
        public ISystemRepository Systems { get; private set; }
        public IClientKeyRepository ClientKeys { get; private set; }

        // Khai báo các Repository mới (Expression Body không được dùng khi khởi tạo trong Constructor)
        public ICommentRepository Comments { get; private set; } // 👈 SỬA
        public ILoginSessionRepository LoginSessions { get; private set; } // 👈 SỬA
        public IAchievementsRepository Achievements { get; private set; } // 👈 SỬA

        public UnitOfWork(QuizGameContext context)
        {
            _context = context;
            // Khởi tạo các Implementations
            Users = new UserRepository(_context);
            Quiz = new QuizRepository(_context);
            Results = new ResultRepository(_context);
            Social = new SocialRepository(_context);
            Systems = new SystemRepository(_context);
            ClientKeys = new ClientKeyRepository(_context);

            // Khởi tạo các Repository mới
            Comments = new CommentRepository(_context); // 👈 BỔ SUNG
            LoginSessions = new LoginSessionRepository(_context); // 👈 BỔ SUNG
            Achievements = new AchievementsRepository(_context); // 👈 BỔ SUNG
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}