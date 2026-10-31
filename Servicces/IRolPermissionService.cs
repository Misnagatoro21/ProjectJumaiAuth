using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Servicces
{
    public interface IRolPermissionService
    {
        Task<IReadOnlyList<IRolPermissionService>> GetAllPermissionAsync();
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<RoleWithPermissionDto?> GetRoleByIdAsync();
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToRoleAsync(int roleId, int permissionIds);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionIds);
        Task<UserPermissionDto?> GetUserPermissionAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionsToUserAsync(int userId, AssingUserPermission dto);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionIds);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(int userId, int roleId);
        Task<DataTableResponseDto<PermissionDto>> GetPermissionPagedAsync(DataTableRequestDto request);
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);

        Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<IReadOnlyList<UserListItemDto>> GetUserListAsync();







    }
}
