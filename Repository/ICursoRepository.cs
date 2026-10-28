using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repository
{
    public interface ICursoRepository
    {
        Task<Curso> CreateAsync(Curso curso);
        Task<bool> DeleteAsync(int id);
        Task<IReadOnlyList<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTableRequestDto request);
        Task<bool> HasAnyEstudentAsync(int id);
        Task<bool> NombreExisteAsync(string nombre, int? excludeId = null);
        Task<bool> UpdateAsync(Curso curso);
    }
}
