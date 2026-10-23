using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Models.ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioByUserName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int Id);
        Task<Usuario> CreateAsync(Usuario usuario);
        bool ValidarPassword(Usuario usuario, string password);
        Task SaveAsync();
        Task<Usuario> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<bool> AssingPermissionToUsuarioAsync(int usuarioId, int permissionId, bool isGranted);
        Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId);
        Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId);
        Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync();
    }
}
