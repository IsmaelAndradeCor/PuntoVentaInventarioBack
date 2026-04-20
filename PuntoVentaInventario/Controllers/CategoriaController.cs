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
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("listar_categorias")]
        public async Task<IActionResult> GetCategorias()
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
                return StatusCode(500, $"Error al obtener categorias: {ex.Message}");
            }
        }

        [HttpGet("obtener_categoria/{id:int}")]
        public async Task<IActionResult> GetCategorisPorId(int id)
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
                return StatusCode(500, $"Error al obtener la marca: {ex.Message}");
            }
        }

        [HttpPost("crear_categoria")]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de la categoria es obligatorio" });

                var existe = await _context.Categorias
                    .AnyAsync(m => m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe una categoria con ese nombre" });

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

                return CreatedAtAction(nameof(GetCategorisPorId), new { id = categoria.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la categoria: {ex.Message}");
            }
        }

        [HttpPut("actualizar_categoria/{id:int}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] CategoriaUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la categoria: {ex.Message}");
            }
        }

        [HttpDelete("eliminar_categoria/{id:int}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(m => m.Id == id && m.Activo);

                if (categoria == null)
                    return NotFound(new { mensaje = "Categoria no encontrada" });

                var tieneProductos = await _context.Productos
                    .AnyAsync(p => p.IdCategoria == id && p.Activo);

                if (tieneProductos)
                    return BadRequest(new { mensaje = "No se puede eliminar la categoria porque tiene productos asociados" });

                categoria.Activo = false;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la categoria: {ex.Message}");
            }
        }
    }
}