using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Repository
{
    public interface IEstudiantRepository
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPageAsync(DataTableRequestDto request);
        Task<Estudiante?> GetByIdAsync(int Id);
        Task<bool> EmailExistsAsync(string email, int? excluded = null);
        Task<Curso> CreateAsync(Estudiante estudiante);
        Task<bool> UpdateAsync(Estudiante estudiante);
        Task<bool> DeleteAsync(int Id);
    }
}
