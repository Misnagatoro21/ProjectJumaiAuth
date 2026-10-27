using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;

namespace ProjectJumaiAuth.Repository
{
    public class RolRepository(AppDbContext context) : IRoleRepository
    {
        // --- Implementación obligatoria de IRoleRepository ---

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await context.Roles
                .Include(r => r.RolePermisos)
                    .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == roleName);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await context.Roles
                .Include(r => r.RolePermisos)
                    .ThenInclude(rp => rp.Permiso)
                .ToListAsync();
        }

        public async Task AddRoleAsync(Role role)
        {
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            context.Roles.Update(role);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await context.Roles.FindAsync(id);
            if (role != null)
            {
                context.Roles.Remove(role);
                await context.SaveChangesAsync();
            }
        }

        // --- Tus métodos personalizados adicionales ---

        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId)
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
            return await GetRoleByIdAsync(id);
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId
                                        && rp.PermisoId == permissionId);

            if (rolePermission is null) return false;

            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
            return true;
        }
    }
}