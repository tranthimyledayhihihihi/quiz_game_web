// Models/Interfaces/IClientKeyRepository.cs
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IClientKeyRepository : IGenericRepository<ClientKey>
    {
        Task<ClientKey?> GetByKeyValueAsync(string keyValue);
    }
}