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

    }
}
