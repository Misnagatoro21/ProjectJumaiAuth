using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repository
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetAllWithPermissionAsync();
        Task<Role?> GetByIdWithPermissionAsync(int Id);
        Task<Role> GetByNameAsync(string name);
        Task<Role> CreateAsync(string name);
        Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRolAsync(int roleId, int permissionId);
    }
}
