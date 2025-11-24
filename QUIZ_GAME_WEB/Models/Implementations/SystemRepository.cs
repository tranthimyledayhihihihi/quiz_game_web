// Models/Implementations/SystemRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class SystemRepository : GenericRepository<SystemSetting>, ISystemRepository
    {
        public SystemRepository(QuizGameContext context) : base(context) { }

        public async Task<string?> GetSettingValueByKeyAsync(string key)
        {
            return await _context.SystemSetting
                                 .Where(s => s.Key == key)
                                 .Select(s => s.Value)
                                 .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateSettingValueAsync(string key, string value)
        {
            var setting = await _context.SystemSetting.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null) return false;

            setting.Value = value;
            _context.SystemSetting.Update(setting);
            // Lưu ý: Cần CompleteAsync() từ UnitOfWork để lưu thay đổi
            return true;
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            return await _context.SystemSetting.ToListAsync();
        }
    }
}