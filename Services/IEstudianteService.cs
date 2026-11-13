using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Servicces
{
    public interface IEstudianteService
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTableRequestDto request);
        Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync();
        Task<SaveestudianteDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveestudianteDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveestudianteDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}