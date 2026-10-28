
using ProjectJumaiAuth.Models;
using System.Collections.Generic;

namespace ProjectJumaiAuth.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync();
        Task<Role?> GetByIdWithPermissionsAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<Role> CreateAsync(string name);
        Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);

    }
}
