using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Servicces
{
    public interface IEstudianteService
    {
           
        Task<DataTableResponseDto<EstudianteTableDto>> GetPageAsync(DataTableRequestDto request);
        Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync();
        Task<SaveCursoDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> Createasync(SaveCursoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCursoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int dto);
    }
}

