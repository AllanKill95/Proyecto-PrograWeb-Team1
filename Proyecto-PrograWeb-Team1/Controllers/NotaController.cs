using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;

namespace Proyecto_PrograWeb_Team1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todo este controller requiere token válido
    public class NotaController : ControllerBase
    {
        private readonly NotaService _notaService;

        public NotaController(NotaService notaService)
        {
            _notaService = notaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearNotaDto dto)
        {
            try
            {
                // Sacamos el UserId del token JWT, no del body
                // Así el usuario no puede crear experimentos a nombre de otro
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var nota = await _notaService.Create(dto, userId);
                return Ok(nota);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNota()
        {
            try
            {
                // Igual, el UserId viene del token no de la URL
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var nota = await _notaService.GetByUser(userId);
                return Ok(nota);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    
}