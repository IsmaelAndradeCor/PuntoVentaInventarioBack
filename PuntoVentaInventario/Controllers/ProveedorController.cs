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
    public class ProveedorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedorController(AppDbContext context) { 
         _context = context;
        }

        [HttpGet("listar_proveedores")]
        public async Task<IActionResult> GetProveedores() {
            try
            {
                var proveedores = await _context.Proveedores
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new ProveedorResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Contacto = p.Contacto,
                        Telefono = p.Telefono,
                        Correo = p.Correo
                    })
                    .ToListAsync();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener proveedores: {ex.Message}");
            }
        }

        [HttpGet("obtener_proveedor/{id:int}")]
        public async Task<IActionResult> GetProveedorPorId(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.Activo)
                    .Select(p => new ProveedorResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre
                    })
                    .FirstOrDefaultAsync();

                if (proveedor == null)
                    return NotFound(new { mensaje = "Proveedor no encontrado" });

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el proveedor: {ex.Message}");
            }
        }

        [HttpPost("crear_proveedor")]
        public async Task<IActionResult> CrearProveedor([FromBody] ProveedorUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nombreNormalizado = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de del proveedor es obligatorio" });

                var existe = await _context.Proveedores
                    .AnyAsync(p => p.Activo && p.Nombre.ToLower() == nombreNormalizado.ToLower());
                if (existe)
                {
                    return BadRequest(new { mensaje = "Ya existe un proveedor con ese nombre" });
                }

                var proveedor = new Proveedor
                {
                    Nombre = nombreNormalizado,
                    Contacto = dto.Contacto,
                    Telefono = dto.Telefono,
                    Correo = dto.Correo,
                    Activo = true
                };

                _context.Proveedores.Add( proveedor );
                await _context.SaveChangesAsync();

                var response = new ProveedorResponseDto
                {
                    Id = proveedor.Id,
                    Nombre = proveedor.Nombre,
                    Contacto = proveedor.Contacto,
                    Telefono = proveedor.Telefono,
                    Correo = proveedor.Correo
                };

                return CreatedAtAction(nameof(GetProveedorPorId), new { id = proveedor.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el proveedor: {ex.Message}");
            }
        }

        [HttpPut("actualizar_proveedor/{idProveedor:int}")]
        public async Task<ActionResult<ProveedorResponseDto>> ActualizarProveedor(int idProveedor, [FromBody] ProveedorUpsertDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var proveedor = await _context.Proveedores
                    .FirstOrDefaultAsync(u => u.Id == idProveedor && u.Activo);

                if (proveedor == null)
                    return NotFound(new { mensaje = "Proveedor no encontrado" });

                var nombreNormalizado = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre del Proveedor es obligatorio" });

                var existe = await _context.Proveedores
                    .AnyAsync(m => m.Id != idProveedor && m.Activo && m.Nombre.ToLower() == nombreNormalizado.ToLower());

                if (existe)
                    return BadRequest(new { mensaje = "Ya existe otro Proveedor con ese nombre" });

                proveedor.Nombre = nombreNormalizado;
                proveedor.Contacto = dto.Contacto?.Trim();
                proveedor.Telefono = dto.Telefono?.Trim();
                proveedor.Correo = dto.Correo?.Trim();

                await _context.SaveChangesAsync();

                var response = new ProveedorResponseDto
                {
                    Id = proveedor.Id,
                    Nombre = proveedor.Nombre,
                    Contacto = proveedor.Contacto,
                    Telefono = proveedor.Telefono,
                    Correo = proveedor.Correo
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el Proveedor: {ex.Message}");
            }
        }

        [HttpDelete("eliminar_proveedor/{idProveedor:int}")]
        public async Task<IActionResult> EliminarProveedor(int idProveedor)
        {
            try
            {
                var proveedor = await _context.Proveedores
                    .FirstOrDefaultAsync(m => m.Id == idProveedor && m.Activo);

                if (proveedor == null)
                    return NotFound(new { mensaje = "Proveedor no encontrado" });

                var tieneProductos = await _context.ProductoProveedores
                    .AnyAsync(p => p.IdProveedor == idProveedor);

                if (tieneProductos)
                    return BadRequest(new { mensaje = "No se puede eliminar al Proveedor porque tiene Productos asociados" });

                proveedor.Activo = false;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar al Proveedor: {ex.Message}");
            }
        }
    }
}
