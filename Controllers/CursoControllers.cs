using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Services;

namespace ProjectJumaiAuth.Controllers
{
    public class CursoControllers(ICursoService _cursoService) : Controller
    {
        [Authorize(Policy = "Cursos.Create")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "Cursos.Create")]
        [HttpPost]
        public async Task<IActionResult> GetData(DataTableRequestDto request)
        {
            var result = await _cursoService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "Cursos.Ver")]
        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _cursoService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Curso not found" });
            return Ok(result);
        }
        [Authorize(Policy = "Cursos.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(SaveCursoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "Invalid curso data" });
            var (Success, ErrorMessage) = await _cursoService.CreateAsync(dto);
            return Success ?
                Ok(new { Success = true, Message = "Curso created successfully" }) :
                BadRequest(new { Success = false, Message = ErrorMessage });
        }
        [Authorize(Policy = "Cursos.Actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(SaveCursoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "Invalid curso data" });

            var (Success, ErrorMessage) = await _cursoService.UpdateAsync(dto);
            return Success?
                Ok(new { Success = true, Message = "Curso updated successfully" }) :
                BadRequest(new { Success = false, Message = ErrorMessage });
        }
        [Authorize(Policy = "Cursos.Eliminar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (Success, ErrorMessage) = await _cursoService.DeleteAsync(id);
            return Success ?
                Ok(new { Success = true, Message = "Curso deleted successfully" }) :
                BadRequest(new { Success = false, Message = ErrorMessage });
        }
    }
}
