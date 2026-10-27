using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Controllers
{
    public record CrearTareaDto([Required, StringLength(200)] string Titulo);

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TareasController(AppDbContext db)
        {
            _db = db;
        }

        private bool TryGetUsuarioId(out int usuarioId)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out usuarioId);
        }

        [HttpGet]
        public async Task<IActionResult> MisTareas()
        {
            if (!TryGetUsuarioId(out var usuarioId))
                return Unauthorized();

            var tareas = await _db.Tareas
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => new { t.Id, t.Titulo, t.Completada })
                .ToListAsync();
            return Ok(tareas);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearTareaDto dto)
        {
            if (!TryGetUsuarioId(out var usuarioId))
                return Unauthorized();

            var tarea = new Tarea { Titulo = dto.Titulo, UsuarioId = usuarioId };
            _db.Tareas.Add(tarea);
            await _db.SaveChangesAsync();
            return Ok(new { tarea.Id, tarea.Titulo, tarea.Completada });
        }

        [HttpGet("todas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Todas()
        {
            var tareas = await _db.Tareas
                .Select(t => new { t.Id, t.Titulo, t.Completada, t.UsuarioId })
                .ToListAsync();
            return Ok(tareas);
        }
    }
}