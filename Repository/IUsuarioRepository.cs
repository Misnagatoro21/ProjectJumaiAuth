using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;

namespace ProjectJumaiAuth.Repository
{
    public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
    {
        private IQueryable<Usuario> UserWithPermission =>
        context.Usuarios
        .Include(u => u.Rol)
        .ThenInclude(r => r.RolePermisos)
        .ThenInclude(rp => rp.Permiso)
        .ThenInclude(up => up.UsuarioPermisos)
        .ThenInclude(up => up.Permiso);

        public async Task<bool> AssingPermissiontoUsuarioAsync(int usuarioId, int permissionId, bool isGranted)
        {
            var userPermission = await context.Usuarios
                .FirstOrDefaultAsync(up => up.Usuarios == usuarioId && up.UsuarioPermisos == permissionId);
            if (userPermission != null)
            {
                userPermission.IsGranted = isGranted;
            }
            else
            {
                context.Usu.Add(new UsuarioPermiso
                {
                    UsuarioId = usuarioId,
                    PermisoId = permissionId,
                    IsGranted = isGranted
                });
            }

            return true;
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            var entry = await context.Usuarios.AddAsync(usuario);
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuariosAsync()
        {
            return await context.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.NombreUsuario)
            .ToListAsync();
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuariosWhitRolesAsync()
        {
            return await context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.NombreUsuario)
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int Id)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<Usuario?> GetUsuarioByEmail(string Email)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.Email == Email);
        }


        public async Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<Usuario?> GetUsuarioByUserName(string Username)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.NombreUsuario == Username);
        }

        public async Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId)
        {
            var userPermission = await context.UsuarioPermisos
                .FirstOrDefaultAsync(up => up.UsuarioId == usuarioId && up.PermisoId == permissionId);
            if (userPermission == null) return false;

            context.UsuarioPermisos.Remove(userPermission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateUsuarioRolAsync(int UsuarioId, int RoleId)
        {
            var user = await context.Usuarios.FindAsync(UsuarioId);
            if (user == null) return false;

            var roleExists = await context.Roles.AnyAsync(r => r.Id == RoleId);
            if (!roleExists) return false;
            user.Rol = RoleId;

            return true;
        }


        public bool ValidatePassword(Usuario usuario, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, usuario.PasswordHash);
        }


        public Task<Usuario?> GetUsuarioByIdWithPermissionAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}