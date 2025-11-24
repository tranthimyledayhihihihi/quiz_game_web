// Models/Implementations/GenericRepository.cs (ĐÃ SỬA LỖI GENERIC VÀ CONSTRUCTOR)
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // Cần thiết
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Linq.Expressions;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Cần đảm bảo rằng QuizGameContext được tìm thấy
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Khắc phục lỗi _context not found
        protected readonly QuizGameContext _context;

        // Khắc phục lỗi GenericRepository<> does not contain a constructor that takes 1 arguments
        public GenericRepository(QuizGameContext context)
        {
            _context = context;
        }

        // Khắc phục lỗi Method Not Implemented
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public void Add(T entity) => _context.Set<T>().Add(entity); // Hàm đồng bộ (nếu cần)
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).ToListAsync();
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _context.Set<T>().CountAsync()
                : await _context.Set<T>().CountAsync(predicate);
        }
    }
}