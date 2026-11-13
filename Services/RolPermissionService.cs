using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Mejoras;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repositories;
using ProjectJumaiAuth.Servicces;

namespace ProjectJumaiAuth.Services
{
    public class RolPermissionService(
        IRoleRepository roleRepo, IPermissionRepository permissionRepo, IUsuarioRepository userRepo) : IRolPermissionServices   
    {


        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionsToRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByIdWithPermissionsAsync(roleId);
            if (role == null)
                return (false, "Rol no encontrado");

            var permission = await permissionRepo.GetByIdAsync(permissionId);
            if (permission == null)
                return (false, "Permiso no encontrado");

            var assigned = await roleRepo.AssingPermissionToRoleAsync(permissionId, roleId);
            if (!assigned)
                return (false, "El permiso ya está asignado al rol");

            return (true, null);
        }


        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionsToUserAsync(int userId, AssingUserPermission dto)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);
            if (user == null)
                return (false, "Usuario no encontrado");

            var permission = await permissionRepo.GetByIdAsync(dto.PermissionId);
            if (permission == null)
                return (false, "Permiso no encontrado");

            await userRepo.AssingPermissiontoUsuarioAsync(dto.PermissionId, userId, dto.IsGranted);
            return (true, null);
        }



        public async Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto)
        {
            var name = dto.Name.Trim().ToLowerInvariant();
            var existing = await permissionRepo.GetByNameAsync(name);
            if (existing != null)
                return (false, null, "Ya existe un permiso con ese nombre");

            var permission = await permissionRepo.CreateAsync(name, dto.Description);
            return (true, new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Nombre,
                Description = permission.Descripcion,
            }, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Id de permiso inválido");

            var permission = await permissionRepo.GetByIdAsync(dto.Id.Value);
            if (permission == null)
                return (false, "Permiso no encontrado");

            var name = dto.Name.Trim().ToLowerInvariant();
            var duplicate = await permissionRepo.GetByNameAsync(name);
            if (duplicate != null && duplicate.Id != permission.Id)
                return (false, "Ya existe un permiso con ese nombre");

            permission.Nombre = name;
            permission.Descripcion = dto.Description?.Trim();
            await permissionRepo.UpdateAsync(permission);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return (false, "Permiso no encontrado");

            if (await permissionRepo.IsAssignedToUsuarioAsync(id))
                return (false, "No se puede eliminar el permiso porque está asignado a uno o más usuarios");

            await permissionRepo.DeleteAsync(id);
            return (true, null);
        }

        // Interfaz: GetAllPermissionAsync()
        public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionAsync()
        {
            var permissions = await permissionRepo.GetAllAsync();
            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Nombre,
                Description = p.Descripcion,
            }).ToList();
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return null;

            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Nombre,
                Description = permission.Descripcion,
            };
        }

        public async Task<DataTableResponseDto<PermissionDto>> GetPermissionPagedAsync(DataTableRequestDto request)
        {
            return await permissionRepo.GetPagedAsync(request);
        }

        // ---------- Roles ----------

        public async Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto)
        {
            var name = dto.Name.Trim();
            var existing = await roleRepo.GetByNameAsync(name);
            if (existing != null)
                return (false, null, "Ya existe un rol con ese nombre");

            var role = await roleRepo.CreateAsync(name);
            return (true, new RoleWithPermissionDto
            {
                Id = role.Id,
                Name = role.Nombre,
                Permissions = []
            }, null);
        }

        public async Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllWithPermissionsAsync();
            return roles.Select(MapRole).ToList();
        }


        public async Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId)

        {
            var role = await roleRepo.GetByIdWithPermissionsAsync(roleId);
            return role == null ? null : MapRole(role);
        }

        public async Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var removed = await roleRepo.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!removed)
                return (false, "El rol no tiene asignado este permiso");

            return (true, null);
        }

        private static RoleWithPermissionDto MapRole(Role role) => new()
        {
            Id = role.Id,
            Name = role.Nombre,
            Permissions = role.RolePermisos.Select(rp => new PermissionDto
            {
                Id = rp.PermisoId,
                Name = rp.Permiso.Nombre,
                Description = rp.Permiso.Descripcion,
            }).OrderBy(p => p.Name).ToList(),
        };

        // ---------- Usuarios ----------

        // Interfaz: GetUserListAsync()
        public async Task<IReadOnlyList<UserListItemDto>> GetUserListAsync()
        {
            var users = await userRepo.GetAllUsuariosAsync();
            return users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                UserName = u.NombreUsuario,
                Email = u.Email,
                RoleName = u.Rol?.Nombre ?? string.Empty
            }).ToList();
        }

        // Interfaz: GetUserPermissionAsync(int)
        public async Task<UserPermissionDto?> GetUserPermissionAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);
            if (user == null)
                return null;

            return new UserPermissionDto
            {
                UserId = user.Id,
                UserName = user.NombreUsuario,
                Email = user.Email,
                RoleId = user.RolId,                         // VERIFICAR (RolId / RoleId)
                RoleName = user.Rol?.Nombre ?? string.Empty,
                EffectivePermission = EffectivePermission.FromUser(user).ToList(),
                DirectPermission = user.UsuarioPermisos.Select(up => new UserPermissionOverrideDto
                {
                    PermissionId = up.PermisoId,             // VERIFICAR
                    PermissionName = up.Permiso.Nombre,      // VERIFICAR
                    IsGranted = up.IsGranted                 // VERIFICAR
                }).ToList(),
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var removed = await userRepo.RemovePermissionFromUsuarioAsync(userId, permissionId);
            if (!removed)
                return (false, "El usuario no tiene asignado este permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId)
        {
            var updated = await userRepo.UpdateUsuarioRolAsync(userId, roleId);
            if (!updated)
                return (false, "Usuario o rol no encontrado");

            return (true, null);
        }
    }
}