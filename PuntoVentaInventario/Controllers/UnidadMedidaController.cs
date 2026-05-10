using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UnidadMedidaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("listar_unidades_medida")]
        public async Task<IActionResult> GetUnidadesMedida()
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
                return StatusCode(500, $"Error al obtener las unidades de medida: {ex.Message}");
            }
        }

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
                return StatusCode(500, $"Error al obtener la unidad de medida: {ex.Message}");
            }
        }

        [HttpPost("crear_unidad_medida")]
        public async Task<IActionResult> CrearUnidadMedida([FromBody] UnidadMedidaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nombreNormalizado = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la unidad de medida es obligatorio" });

                var existe = await _context.UnidadesMedida
                    .AnyAsync(p => p.Activo && p.Nombre.ToLower() == nombreNormalizado.ToLower());
                if (existe)
                {
                    return BadRequest(new { mensaje = "Ya existe una unidad de medida con ese nombre" });
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
                return StatusCode(500, $"Error al crear la unidad de medida: {ex.Message}");
            }
        }

        [HttpPut("actualizar_unidad_medida/{idUnidadMedida:int}")]
        public async Task<ActionResult<UnidadMedidaResponseDto>> ActualizarUnidadMedida(int idUnidadMedida, [FromBody] UnidadMedidaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
                return StatusCode(500, $"Error al actualizar la unidad de medida: {ex.Message}");
            }
        }

        [HttpDelete("eliminar_unidad_medida/{idUnidadMedida:int}")]
        public async Task<IActionResult> EliminarUnidadMedida(int idUnidadMedida)
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
                return StatusCode(500, $"Error al eliminar la Unidad de Medida: {ex.Message}");
            }
        }
    }
}
