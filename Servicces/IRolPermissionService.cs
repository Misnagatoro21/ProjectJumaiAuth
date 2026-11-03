using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Servicces
{
    public interface IRolPermissionServices
    {
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToRoleAsync(Guid roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToUserAsync(string userId, AssingUserPermission dto);
        Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);
        Task<IReadOnlyList<PermissionDto>> GetAllPermissionAsync();
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<DataTableResponseDto<PermissionDto>> GetPermissionPagedAsync(DataTableRequestDto request);
        Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<RoleWithPermissionDto?> GetRoleByIdAsync(Guid roleId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<IReadOnlyList<UserListItemDto>> GetUserListAsync();
        Task<UserPermissionDto?> GetUserPermissionAsync(string userId);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
    }
}