// Models/Implementations/ApiKeyValidationService.cs
using QUIZ_GAME_WEB.Models.Interfaces;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ApiKeyValidationService : IApiKeyValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiKeyValidationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> IsValidApiKey(string userApiKey)
        {
            // Logic: Gọi Repository để kiểm tra Key
            var clientKey = await _unitOfWork.ClientKeys.GetByKeyValueAsync(userApiKey);

            // Logic nghiệp vụ: Kiểm tra Key có tồn tại, có active và chưa hết hạn không
            if (clientKey == null || !clientKey.IsActive)
            {
                return false;
            }

            if (clientKey.NgayHetHan.HasValue && clientKey.NgayHetHan.Value < DateTime.Now)
            {
                return false; // Key đã hết hạn
            }

            return true;
        }
    }
}