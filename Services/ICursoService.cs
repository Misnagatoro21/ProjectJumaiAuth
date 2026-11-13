using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Services
{
    public interface ICursoService
    {
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCursoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
        Task<IReadOnlyList<CursoTableDto>> GetAllAsync();
        Task<SaveCursoDto?> GetByIdAsync(int id);
        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTableRequestDto request);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCursoDto dto);
    }
}