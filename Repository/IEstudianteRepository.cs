using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repository
{
    public interface IEstudianteRepository
    {
        Task<Estudiante> CreateAsync(Estudiante estudiante);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<Estudiante?> GetByIdAsync(int id);
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTableRequestDto request);
        Task<bool> UpdateAsync(Estudiante estudiante);
    }
}