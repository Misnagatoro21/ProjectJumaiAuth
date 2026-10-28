
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;

namespace ProjectJumaiAuth.Repositories
{
    public class CursoRepository(AppDbContext context) : ICursoRepository
    {
        public async Task<Curso> CreateAsync(Curso curso)
        {
            var entry = await context.AddAsync(curso);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var curso = await context.Cursos.FindAsync(Id);
            if (curso == null) return false;

            context.Cursos.Remove(curso);
            await context.SaveChangesAsync();
            return true;

        }
        public async Task<IReadOnlyList<Curso>> GetAllAsync()
        {
            return await context.Cursos.OrderBy(c => c.Nombre).ToListAsync();

        }

        public async Task<Curso?> GetByIdAsync(int Id)
        {
            return await context.Cursos.FindAsync(Id);
        }

        public async Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTableRequestDto request)
        {
            var query = context.Cursos.AsQueryable();
            var recordsTotal = await query.CountAsync();
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                || (c.Descripcion != null && c.Descripcion.Contains(search)));
            }
            var recordsFiltered = await query.CountAsync();
            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var ColumnName = request.Columns.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = ColumnName.ToLowerInvariant() switch
            {
                "descripcion" => orderDir == "desc" ? query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Descripcion),
                "totalestudiantes" => orderDir == "desc" ? query.OrderByDescending(c => c.Estudiantes.Count) : query.OrderBy(c => c.Estudiantes.Count),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)

            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new CursoTableDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    TotalEstudiantes = c.Estudiantes.Count
                }).ToListAsync();
            return new DataTableResponseDto<CursoTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> HasAnyEstudentAsync(int Id)
        {
            return await context.Cursos.AnyAsync(c => c.Id == Id);
        }

        public async Task<bool> NombreExisteAsync(string nombre, int? excludeId = null)
        {
            return await context.Cursos.AnyAsync(
                c => c.Nombre == nombre && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> UpdateAsync(Curso curso)
        {
            context.Cursos.Update(curso);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
