using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Repository
{
    public interface IPermissionRepository
    {
        Task<DataTableResponseDto<PermissionDto>> GetPageAsync(DataTableRequestDto request);
        Task<PermissionDto?> GetByIdAsync(int Id);
        Task<PermissionDto?> GetByNameAsync(string Name);
        Task<PermissionDto?> CreateAsync(string Name, string? Description);
        Task<bool> UpdateAsync(PermissionDto permission);
        Task<bool> DeleteteAsync(int id);
        Task<bool> IsAssingnedToUsuarioasync(int PermissionId);
    }
}
