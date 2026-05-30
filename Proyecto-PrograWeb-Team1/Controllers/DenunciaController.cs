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
    public class DenunciaController : ControllerBase
    {
        private readonly DenunciaServices _denunciaService;

        public DenunciaController(DenunciaServices denunciaService)
        {
            _denunciaService = denunciaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearDenunciaDto dto)
        {
            try
            {
                // Sacamos el UserId del token JWT, no del body
                // Así el usuario no puede crear experimentos a nombre de otro
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var denuncia = await _denunciaService.Create(dto, userId);
                return Ok(denuncia);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDenuncia()
        {
            try
            {
                // Igual, el UserId viene del token no de la URL
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var denuncia = await _denunciaService.GetByUser(userId);
                return Ok(denuncia);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}