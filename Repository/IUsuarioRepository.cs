using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> AssingPermissiontoUsuarioAsync(int usuarioId, int permissionId, bool isGranted);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<IReadOnlyList<Usuario>> GetAllUsuariosAsync();
        Task<IReadOnlyList<Usuario>> GetAllUsuariosWhitRolesAsync();
        Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int id);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<Usuario?> GetUsuarioByUserName(string username);
        Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId);
        Task SaveAsync();
        Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId);
        bool ValidatePassword(Usuario usuario, string password);
    }
}