using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Controllers
{
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

        private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> MisTareas()
        {
            var tareas = await _db.Tareas
                .Where(t => t.UsuarioId == UsuarioId)
                .Select(t => new { t.Id, t.Titulo, t.Completada })
                .ToListAsync();
            return Ok(tareas);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tarea dto)
        {
            var tarea = new Tarea { Titulo = dto.Titulo, UsuarioId = UsuarioId };
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