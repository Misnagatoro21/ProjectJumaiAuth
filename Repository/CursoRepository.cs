using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;

namespace ProjectJumaiAuth.Repository
{
    public class CursoRepository(AppDbContext context) : ICursoRepository
    {
        public async Task<Curso> CreateAsync(Curso curso)
        {
            var entry = await context.Cursos.AddAsync(curso);
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

        public async Task<DataTableResponseDto<CursoTableDto>> GetPageAsync(DataTableRequestDto request)
        {
            var query = context.Cursos.AsNoTracking().AsQueryable();

            var recordsTotal = await query.CountAsync();

            // Búsqueda
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(search) ||
                    (c.Descripcion != null && c.Descripcion.Contains(search)));
            }

            var recordsFiltered = await query.CountAsync();

            // Orden
            var order = request.Order.FirstOrDefault();
            var columnName = request.Columns
                .ElementAtOrDefault(order?.Column ?? 0)?.Data?.ToLower() ?? "nombre";
            var desc = string.Equals(order?.Dir, "desc", StringComparison.OrdinalIgnoreCase);

            query = (columnName, desc) switch
            {
                ("id", true) => query.OrderByDescending(c => c.Id),
                ("id", false) => query.OrderBy(c => c.Id),
                ("descripcion", true) => query.OrderByDescending(c => c.Descripcion),
                ("descripcion", false) => query.OrderBy(c => c.Descripcion),
                (_, true) => query.OrderByDescending(c => c.Nombre),
                _ => query.OrderBy(c => c.Nombre)
            };

            // Paginación única y correcta
            if (request.Length > 0)
            {
                query = query.Skip(request.Start).Take(request.Length);
            }
            else
            {
                query = query.Skip(request.Start).Take(10); // Valor por defecto si length es -1 o 0
            }

            var data = await query
     .Select(c => new ProjectJumaiAuth.DTOS.CursoTableDto
     {
         Id = c.Id,
         Nombre = c.Nombre,
         Descripcion = c.Descripcion,
         TotalEstudiantes = c.Estudiantes.Count
     })
     .ToListAsync();

            return new ProjectJumaiAuth.DTOS.DataTableResponseDto<ProjectJumaiAuth.DTOS.CursoTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

  

        public async Task<bool> NombreExisteAsync(string nombre, int? excludeId = null)
        {
            return await context.Cursos.AnyAsync(c =>
                c.Nombre == nombre && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> UpdateAsync(Curso curso)
        {
            context.Cursos.Update(curso);
            return await context.SaveChangesAsync() > 0;
        }
    }
}   