
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;
using ProjectJumaiAuth.Services ;

namespace ProjectJumaiAuth.Services
{
    public class CursoService(ICursoRepository repo) : ICursoService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCursoDto dto)
        {
            if (await repo.NombreExisteAsync(dto.Nombre.Trim()))
                return (false, "El nombre del curso ya existe");
            await repo.CreateAsync(new Curso
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion?.Trim(),
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasAnyEstudentAsync(id))
                return (false, "No se puede eliminar el curso denido a que tiene estudiantes asignados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Curso no encontrado");
        }

        public async Task<IReadOnlyList<CursoTableDto>> GetAllAsync()
        {
            var cursos = await repo.GetAllAsync();
            return cursos.Select(c => new CursoTableDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
            }).ToList();
        }

        public async Task<SaveCursoDto?> GetByIdAsync(int id)
        {
            var curso = await repo.GetByIdAsync(id);
            if (curso == null) return null;
            return new SaveCursoDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                Descripcion = curso.Descripcion,
            };
        }

        public Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTableRequestDto request)
        => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCursoDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Id invalido");

            var curso = await repo.GetByIdAsync(dto.Id.Value);
            if (curso == null)
                return (false, "Curso no encontrado");

            if (await repo.NombreExisteAsync(dto.Nombre.Trim(), dto.Id))
                return (false, "Ya existe un curso con ese nombre");
            curso.Nombre = dto.Nombre;
            curso.Descripcion = dto.Descripcion;
            await repo.UpdateAsync(curso);
            return (true, null);
        }
    }
}
