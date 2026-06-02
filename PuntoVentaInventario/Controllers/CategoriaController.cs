using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(AppDbContext context, ILogger<CategoriaController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Policy = Permissions.Categorias.ActivosVer)]
        [HttpGet("listar_categorias_activas")]
        public async Task<IActionResult> GetCategoriasActivas()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new CategoriaResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre
                    })
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías activas");
                return StatusCode(500, new { mensaje = "Error interno al obtener categorías." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.InactivosVer)]
        [HttpGet("listar_categorias_inactivas")]
        public async Task<IActionResult> GetCategoriasInactivas()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(m => !m.Activo)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new CategoriaResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre
                    })
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías inactivas");
                return StatusCode(500, new { mensaje = "Error interno al obtener categorías." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.Ver)]
        [HttpGet("obtener_categoria/{id:int}")]
        public async Task<IActionResult> GetCategoriasPorId(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .AsNoTracking()
                    .Where(m => m.Id == id && m.Activo)
                    .Select(m => new CategoriaResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre
                    })
                    .FirstOrDefaultAsync();

                if (categoria == null)
                    return NotFound(new { mensaje = "Categoria no encontrada" });

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría por ID");
                return StatusCode(500, new { mensaje = "Error interno al obtener la categoría." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.Crear)]
        [HttpPost("crear_categoria")]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaUpsertDto dto)
        {
            try
            {
                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la categoria es obligatorio" });

                var existe = await _context.Categorias
                    .AnyAsync(m => m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe una categoria con ese nombre, por favor revisa que no este inactiva" });

                var categoria = new Categoria
                {
                    Nombre = nombreNormalizado,
                    Activo = true
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                var response = new CategoriaResponseDto
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre
                };

                return CreatedAtAction(nameof(GetCategoriasPorId), new { id = categoria.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría");
                return StatusCode(500, new { mensaje = "Error interno al crear la categoría." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.Actualizar)]
        [HttpPut("actualizar_categoria/{id:int}")]
        public async Task<ActionResult<CategoriaResponseDto>> ActualizarCategoria(int id, [FromBody] CategoriaUpsertDto dto)
        {
            try
            {
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

                if (categoria == null)
                    return NotFound(new { mensaje = "Categoria no encontrada" });

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la categoria es obligatorio" });

                var existe = await _context.Categorias
                    .AnyAsync(m => m.Id != id && m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe otra categoria con ese nombre" });

                categoria.Nombre = nombreNormalizado;

                await _context.SaveChangesAsync();

                var response = new CategoriaResponseDto
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría");
                return StatusCode(500, new { mensaje = "Error interno al actualizar la categoría." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.Activar)]
        [HttpPut("activar_categoria/{id:int}")]
        public async Task<IActionResult> ActivarCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (categoria == null)
                    return NotFound(new { mensaje = "Categoria no encontrada" });

                if (categoria.Activo == true)
                    return BadRequest(new { mensaje = "Categoria ya activa" });

                categoria.Activo = true;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar la categoría");
                return StatusCode(500, new { mensaje = "Error interno al activar la categoría." });
            }
        }

        [Authorize(Policy = Permissions.Categorias.Desactivar)]
        [HttpDelete("desactivar_categoria/{id:int}")]
        public async Task<IActionResult> DesactivarCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (categoria == null)
                    return NotFound(new { mensaje = "Categoria no encontrada" });

                if (categoria.Activo == false)
                    return BadRequest(new { mensaje = "Categoria ya inactiva" });

                var tieneProductos = await _context.Productos
                    .AnyAsync(p => p.IdCategoria == id && p.Activo);

                if (tieneProductos)
                    return BadRequest(new { mensaje = "No se puede desactivar la categoria porque tiene productos asociados" });

                categoria.Activo = false;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar la categoría");
                return StatusCode(500, new { mensaje = "Error interno al desactivar la categoría." });
            }
        }
    }
}