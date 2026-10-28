
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repositories;

namespace ProjectJumaiAuth.Repositories
{
    public class RolRepository(AppDbContext context) : IRoleRepository
    {
        public async Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId)
        {
            var exists = await context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermisoId == permissionId);

            if (exists) return false;
            context.RolePermissions.Add(new RolePermiso
            {
                RoleId = roleId,
                PermisoId = permissionId
            });
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Role> CreateAsync(string name)
        {
            var role = new Role { Nombre = name.Trim() };
            var entry = await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync()
        {
            return await context.Roles
                .Include(r => r.RolePermisos)
                .ThenInclude(rp => rp.Permiso)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await context.Roles
              .Include(r => r.RolePermisos)
              .ThenInclude(rp => rp.Permiso)
              .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<Role?> GetByIdWithPermissionsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await context.Roles.FirstOrDefaultAsync(r => r.Nombre == name);
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId
                && rp.PermisoId == permissionId);

            if (rolePermission == null)
                return false;
            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
