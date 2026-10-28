using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;

namespace ProjectJumaiAuth.Repositories
{
    public class EstudianteRepository(AppDbContext context) : IEstudianteRepository
    {
        public async Task<Estudiante> CreateAsync(Estudiante estudiante)
        {
            var entry = await context.Estudiantes.AddAsync(estudiante);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var estudiante = await context.Estudiantes.FindAsync(id);
            if (estudiante == null) return false;

            context.Estudiantes.Remove(estudiante);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            return await context.Estudiantes
                .AnyAsync(e => e.Email == email &&
                              (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public async Task<Estudiante?> GetByIdAsync(int id)
        {
            return await context.Estudiantes
                .Include(e => e.Curso)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTableRequestDto request)
        {
            IQueryable<Estudiante> query = context.Estudiantes.AsNoTracking();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Nombre.Contains(search) ||
                    e.Apellido.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Curso.Nombre.Contains(search));
            }

            var recordsFiltered = await query.CountAsync();

            var order = request.Order?.FirstOrDefault();
            var orderColumn = order?.Column ?? 0;
            var desc = string.Equals(order?.Dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.Columns?.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "apellido" => desc ? query.OrderByDescending(e => e.Apellido) : query.OrderBy(e => e.Apellido),
                "email" => desc ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "cursonombre" => desc ? query.OrderByDescending(e => e.Curso.Nombre) : query.OrderBy(e => e.Curso.Nombre),
                "id" => desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
                _ => desc ? query.OrderByDescending(e => e.Nombre) : query.OrderBy(e => e.Nombre)
            };

            // Desempate para que la paginación sea estable
            query = ((IOrderedQueryable<Estudiante>)query).ThenBy(e => e.Id);

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(e => new EstudianteTableDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Email = e.Email,
                    CursoId = e.CursoId,
                    CursoNombre = e.Curso.Nombre
                })
                .ToListAsync();

            return new DataTableResponseDto<EstudianteTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> UpdateAsync(Estudiante estudiante)
        {
            context.Estudiantes.Update(estudiante);
            return await context.SaveChangesAsync() > 0;
        }
    }
}