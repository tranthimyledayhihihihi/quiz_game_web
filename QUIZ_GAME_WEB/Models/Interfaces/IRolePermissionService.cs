using QUIZ_GAME_WEB.Models.CoreEntities;

public interface IRolePermissionService
{
    Task<VaiTro?> GetUserRoleAsync(int userId);
    Task<IEnumerable<Quyen>> GetPermissionsByUserIdAsync(int userId);
    Task<IEnumerable<Quyen>> GetPermissionsByRoleIdAsync(int roleId);
    Task<bool> AssignRoleToUserAsync(int userId, int roleId);
    Task<bool> UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds);
}
