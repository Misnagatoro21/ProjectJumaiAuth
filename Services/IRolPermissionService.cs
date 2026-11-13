using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Services
{
    public interface IRolPermissionServices
    {
        // Permisos
        Task<DataTableResponseDto<PermissionDto>> GetPermissionPagedAsync(DataTableRequestDto request);
        Task<IReadOnlyList<PermissionDto>> GetAllPermissionAsync();
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);

        // Roles
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId);
        Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToRoleAsync(int roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId);

        // Usuarios
        Task<IReadOnlyList<UserListItemDto>> GetUserListAsync();
        Task<UserPermissionDto?> GetUserPermissionAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToUserAsync(int userId, AssingUserPermission dto);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
    }
}