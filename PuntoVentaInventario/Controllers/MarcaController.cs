using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MarcaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("listar_marcas")]
        public async Task<IActionResult> GetMarcas()
        {
            try
            {
                var marcas = await _context.Marcas
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new MarcaResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre
                    })
                    .ToListAsync();

                return Ok(marcas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener marcas: {ex.Message}");
            }
        }

        [HttpGet("obtener_marca/{id:int}")]
        public async Task<IActionResult> GetMarcaPorId(int id)
        {
            try
            {
                var marca = await _context.Marcas
                    .AsNoTracking()
                    .Where(m => m.Id == id && m.Activo)
                    .Select(m => new MarcaResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre
                    })
                    .FirstOrDefaultAsync();

                if (marca == null)
                    return NotFound(new { mensaje = "Marca no encontrada" });

                return Ok(marca);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la marca: {ex.Message}");
            }
        }

        [HttpPost("crear_marca")]
        public async Task<IActionResult> CrearMarca([FromBody] MarcaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la marca es obligatorio" });

                var existe = await _context.Marcas
                    .AnyAsync(m => m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe una marca con ese nombre" });

                var marca = new Marca
                {
                    Nombre = nombreNormalizado,
                    Activo = true
                };

                _context.Marcas.Add(marca);
                await _context.SaveChangesAsync();

                var response = new MarcaResponseDto
                {
                    Id = marca.Id,
                    Nombre = marca.Nombre
                };

                return CreatedAtAction(nameof(GetMarcaPorId), new { id = marca.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la marca: {ex.Message}");
            }
        }

        [HttpPut("actualizar_marca/{id:int}")]
        public async Task<IActionResult> ActualizarMarca(int id, [FromBody] MarcaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var marcaDb = await _context.Marcas
                    .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

                if (marcaDb == null)
                    return NotFound(new { mensaje = "Marca no encontrada" });

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la marca es obligatorio" });

                var existe = await _context.Marcas
                    .AnyAsync(m => m.Id != id && m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe otra marca con ese nombre" });

                marcaDb.Nombre = nombreNormalizado;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la marca: {ex.Message}");
            }
        }

        [HttpDelete("eliminar_marca/{id:int}")]
        public async Task<IActionResult> EliminarMarca(int id)
        {
            try
            {
                var marca = await _context.Marcas
                    .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

                if (marca == null)
                    return NotFound(new { mensaje = "Marca no encontrada" });

                var tieneProductos = await _context.Productos
                    .AnyAsync(p => p.IdMarca == id && p.Activo);

                if (tieneProductos)
                    return BadRequest(new { mensaje = "No se puede eliminar la marca porque tiene productos asociados" });

                marca.Activo = false;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la marca: {ex.Message}");
            }
        }
    }
}