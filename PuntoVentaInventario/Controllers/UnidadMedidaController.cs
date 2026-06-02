using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadMedidaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnidadMedidaController> _logger;

        public UnidadMedidaController(AppDbContext context, ILogger<UnidadMedidaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = Permissions.UnidadesMedida.ActivosVer)]
        [HttpGet("listar_unidades_medida_activas")]
        public async Task<IActionResult> GetUnidadesMedidaActivas()
        {
            try
            {
                var unidadesMedida = await _context.UnidadesMedida
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new UnidadMedidaResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Clave = p.Clave,
                        PermiteDecimales = p.PermiteDecimales
                    })
                    .ToListAsync();
                return Ok(unidadesMedida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener unidades de medida activas");
                return StatusCode(500, new { mensaje = "Error interno al obtener las unidades de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.InactivosVer)]
        [HttpGet("listar_unidades_medida_inactivas")]
        public async Task<IActionResult> GetUnidadesMedidaInactivas()
        {
            try
            {
                var unidadesMedida = await _context.UnidadesMedida
                    .Where(p => !p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new UnidadMedidaResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Clave = p.Clave,
                        PermiteDecimales = p.PermiteDecimales
                    })
                    .ToListAsync();
                return Ok(unidadesMedida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener unidades de medida inactivas");
                return StatusCode(500, new { mensaje = "Error interno al obtener las unidades de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.Ver)]
        [HttpGet("obtener_unidad_medida/{id:int}")]
        public async Task<IActionResult> GetUnidadMedidaPorId(int id)
        {
            try
            {
                var unidadesMedida = await _context.UnidadesMedida
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.Activo)
                    .Select(p => new UnidadMedidaResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Clave = p.Clave,
                        PermiteDecimales = p.PermiteDecimales
                    })
                    .FirstOrDefaultAsync();

                if (unidadesMedida == null)
                    return NotFound(new { mensaje = "Unidad Medida no encontrado" });

                return Ok(unidadesMedida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la unidad de medida por ID");
                return StatusCode(500, new { mensaje = "Error interno al obtener la unidad de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.Crear)]
        [HttpPost("crear_unidad_medida")]
        public async Task<IActionResult> CrearUnidadMedida([FromBody] UnidadMedidaUpsertDto dto)
        {
            try
            {
                var nombreNormalizado = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la unidad de medida es obligatorio" });

                var existe = await _context.UnidadesMedida
                    .AnyAsync(p => p.Nombre.ToLower() == nombreNormalizado.ToLower());
                if (existe)
                {
                    return BadRequest(new { mensaje = "Ya existe una unidad de medida con ese nombre, por favor revisa que no esté inactivo" });
                }

                var unidadMedida = new UnidadMedida
                {
                    Nombre = nombreNormalizado,
                    Clave = dto.Clave,
                    PermiteDecimales = dto.PermiteDecimales,
                    Activo = true
                };

                _context.UnidadesMedida.Add(unidadMedida);
                await _context.SaveChangesAsync();

                var response = new UnidadMedidaResponseDto
                {
                    Id = unidadMedida.Id,
                    Nombre = unidadMedida.Nombre,
                    Clave = unidadMedida.Clave,
                    PermiteDecimales = unidadMedida.PermiteDecimales
                };

                return CreatedAtAction(nameof(GetUnidadMedidaPorId), new { id = unidadMedida.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la unidad de medida");
                return StatusCode(500, new { mensaje = "Error interno al crear la unidad de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.Actualizar)]
        [HttpPut("actualizar_unidad_medida/{idUnidadMedida:int}")]
        public async Task<ActionResult<UnidadMedidaResponseDto>> ActualizarUnidadMedida(int idUnidadMedida, [FromBody] UnidadMedidaUpsertDto dto)
        {
            try
            {
                var unidadMedida = await _context.UnidadesMedida
                    .FirstOrDefaultAsync(u => u.Id == idUnidadMedida && u.Activo);

                if (unidadMedida == null)
                    return NotFound(new { mensaje = "Unidad de Medida no encontrada" });

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la unidad de medida es obligatorio" });

                var existe = await _context.UnidadesMedida
                    .AnyAsync(m => m.Id != idUnidadMedida && m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe otra unidad de medida con ese nombre" });

                unidadMedida.Nombre = nombreNormalizado;
                unidadMedida.Clave = dto.Clave.Trim();
                unidadMedida.PermiteDecimales = dto.PermiteDecimales;

                await _context.SaveChangesAsync();

                var response = new UnidadMedidaResponseDto
                {
                    Id = unidadMedida.Id,
                    Nombre = unidadMedida.Nombre,
                    Clave = unidadMedida.Clave,
                    PermiteDecimales = unidadMedida.PermiteDecimales
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la unidad de medida");
                return StatusCode(500, new { mensaje = "Error interno al actualizar la unidad de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.Activar)]
        [HttpPut("activar_unidad_medida/{idUnidadMedida:int}")]
        public async Task<IActionResult> ActivarUnidadMedida(int idUnidadMedida)
        {
            try
            {
                var unidadMedida = await _context.UnidadesMedida
                    .FirstOrDefaultAsync(m => m.Id == idUnidadMedida && !m.Activo);

                if (unidadMedida == null)
                    return NotFound(new { mensaje = "Unidad de Medida no encontrada" });

                unidadMedida.Activo = true;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar la unidad de medida");
                return StatusCode(500, new { mensaje = "Error interno al activar la unidad de medida." });
            }
        }

        [Authorize(Policy = Permissions.UnidadesMedida.Desactivar)]
        [HttpDelete("desactivar_unidad_medida/{idUnidadMedida:int}")]
        public async Task<IActionResult> DesactivarUnidadMedida(int idUnidadMedida)
        {
            try
            {
                var unidadMedida = await _context.UnidadesMedida
                    .FirstOrDefaultAsync(m => m.Id == idUnidadMedida && m.Activo);

                if (unidadMedida == null)
                    return NotFound(new { mensaje = "Unidad de Medida no encontrada" });

                var tieneProductos = await _context.Productos
                    .AnyAsync(p => p.IdUnidadMedida == idUnidadMedida && p.Activo);

                if (tieneProductos)
                    return BadRequest(new { mensaje = "No se puede eliminar la Unidad de Medida porque tiene productos asociados" });

                unidadMedida.Activo = false;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar la unidad de medida");
                return StatusCode(500, new { mensaje = "Error interno al desactivar la unidad de medida." });
            }
        }
    }
}
