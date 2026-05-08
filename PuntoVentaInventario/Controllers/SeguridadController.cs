using Microsoft.AspNetCore.Mvc;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/seguridad")]
    public class SeguridadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SeguridadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("validar-pin")]
        public ActionResult<ValidarPinResponseDto> ValidarPin([FromBody] ValidarPinRequestDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Pin))
                {
                    return BadRequest(new ValidarPinResponseDto
                    {
                        Autorizado = false,
                        Mensaje = "El pin es obligatorio."
                    });
                }

                var pinConfigurado = _configuration.GetValue<string>("Security:VistaPin");

                if (string.IsNullOrWhiteSpace(pinConfigurado))
                {
                    return StatusCode(500, new ValidarPinResponseDto
                    {
                        Autorizado = false,
                        Mensaje = "No hay un pin configurado."
                    });
                }

                if (dto.Pin.Trim() != pinConfigurado.Trim())
                {
                    return Unauthorized(new ValidarPinResponseDto
                    {
                        Autorizado = false,
                        Mensaje = "Pin incorrecto."
                    });
                }

                return Ok(new ValidarPinResponseDto
                {
                    Autorizado = true,
                    Mensaje = "Acceso concedido."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ValidarPinResponseDto
                {
                    Autorizado = false,
                    Mensaje = $"Error al validar el pin: {ex.Message}"
                });
            }
        }
    }
}
