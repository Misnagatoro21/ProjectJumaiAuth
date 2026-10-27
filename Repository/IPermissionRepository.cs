using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Repository;
using System.Security;


namespace ProjectJumaiAuth.Repository
{
    public class PermissionRepository(AppDbContext context) : IPermissionRepository
    {
        public async Task<Permiso> CreateAsync(string name, string? description)
        {
            var permission = new Permiso
            {
                Nombre = name.Trim().ToLowerInvariant(),
                Descripcion = description?.Trim()
            };
            var entry = await context.AddAsync(permission);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await context.Permisos.FindAsync(id);
            if (permission == null) return false;
            context.Permisos.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Permiso>> GetAllAsync()
        {
            return await context.Permisos   
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task<Permiso?> GetByIdAsync(int Id)
        {
            return await context.Permisos.FindAsync(Id);
        }

        public async Task<Permiso?> GetByNameAsync(string name)
        {
            return await context.Permisos
                .FirstOrDefaultAsync(x => x.Nombre == name);
        }

        public async Task<DataTableResponseDto<PermissionDto>> GetPagedAsync(DataTableRequestDto request)
        {
            var query = context.Permisos.AsQueryable();
            var recordsTotal = await query.CountAsync();
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                || (c.Descripcion != null && c.Descripcion.Contains(search)
                ));
            }
            var recordsFiltered = await query.CountAsync();
            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var ColumnName = request.Columns.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";


            query = ColumnName.ToLowerInvariant() switch
            {
                "descripcion" => orderDir == "desc" ? query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Descripcion),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new PermissionDto
                {
                    Id = c.Id,
                    Name = c.Nombre,
                    Description = c.Descripcion,

                }).ToListAsync();
            return new DataTableResponseDto<PermissionDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public Task<bool> IsAssignedToUsuarioAsync(int permissionId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> isAssingnedToUsuarioAsync(int permissionId)
        {
            return await context.UsuarioPermisos.AnyAsync(up => up.PermisoId == permissionId);
        }

        public async Task<bool> UpdateAsync(Permiso permission)
        {
            context.Permisos.Update(permission);
            await context.SaveChangesAsync();
            return true;
        }

    }
}