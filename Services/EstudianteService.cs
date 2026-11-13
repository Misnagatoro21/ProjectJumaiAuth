using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repository;
using ProjectJumaiAuth.Servicces;

namespace ProjectJumaiAuth.Services
{
    public class EstudianteService(IEstudianteRepository repo, ICursoRepository repoCurso) : IEstudianteService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveestudianteDto dto)
        {
            var email = dto.Email.Trim();

            if (await repo.EmailExistsAsync(email))
                return (false, "Ya existe un correo asociado a un estudiante");

            if (await repoCurso.GetByIdAsync(dto.CursoId) == null)
                return (false, "El curso seleccionado no existe");

            await repo.CreateAsync(new Estudiante
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Email = email,
                CursoId = dto.CursoId,
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);
            if (!deleted)
                return (false, "Estudiante no encontrado");

            return (true, null);
        }

        public Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync()
        {
            // Pendiente: implementar cuando se defina qué campos tiene EstudianteTableDto
            throw new NotImplementedException();
        }

        public async Task<SaveestudianteDto?> GetByIdAsync(int id)
        {
            var estudiante = await repo.GetByIdAsync(id);
            if (estudiante == null)
                return null;

            return new SaveestudianteDto
            {
                Id = estudiante.Id,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                Email = estudiante.Email,
                CursoId = estudiante.CursoId,
            };
        }

        public Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTableRequestDto request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveestudianteDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Id inválido");

            var estudiante = await repo.GetByIdAsync(dto.Id.Value);
            if (estudiante == null)
                return (false, "Estudiante no encontrado");

            var email = dto.Email.Trim();

            if (await repo.EmailExistsAsync(email, dto.Id))
                return (false, "Ya existe otro estudiante con ese correo");

            if (await repoCurso.GetByIdAsync(dto.CursoId) == null)
                return (false, "El curso seleccionado no existe");

            estudiante.Nombre = dto.Nombre.Trim();
            estudiante.Apellido = dto.Apellido.Trim();
            estudiante.Email = email;
            estudiante.CursoId = dto.CursoId;

            await repo.UpdateAsync(estudiante);
            return (true, null);
        }
    }
}