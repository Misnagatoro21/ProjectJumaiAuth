
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Repositories
{
    public interface IPermissionRepository
    {
        Task<DataTableResponseDto<PermissionDto>> GetPagedAsync(DataTableRequestDto request);

        Task<IReadOnlyList<Permiso>> GetAllAsync();
        Task<Permiso?> GetByIdAsync(int id);
        Task<Permiso?> GetByNameAsync(string name);

        Task<Permiso> CreateAsync(string name, string? description);
        Task<bool> UpdateAsync(Permiso permission);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsAssignedToUsuarioAsync(int permissionId);

    }
}
