using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;
using System.Data;
using System.Security.Claims;

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

        [Authorize(Policy = Permissions.Proveedores.ActivosVer)]
        [HttpGet("listar_proveedores_activos")]
        public async Task<IActionResult> GetProveedoresActivos() 
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

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

        [Authorize(Policy = Permissions.Proveedores.InactivosVer)]
        [HttpGet("listar_proveedores_inactivos")]
        public async Task<IActionResult> GetProveedoresInactivos()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var proveedores = await _context.Proveedores
                    .Where(p => !p.Activo)
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

        [Authorize(Policy = Permissions.Proveedores.Ver)]
        [HttpGet("obtener_proveedor/{id:int}")]
        public async Task<IActionResult> GetProveedorPorId(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var proveedor = await _context.Proveedores
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.Activo)
                    .Select(p => new ProveedorResponseDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Telefono = p.Telefono,
                        Contacto = p.Contacto,
                        Correo = p.Correo
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

        [Authorize(Policy = Permissions.Proveedores.Crear)]
        [HttpPost("crear_proveedor")]
        public async Task<IActionResult> CrearProveedor([FromBody] ProveedorUpsertDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var nombreNormalizado = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                    return BadRequest(new { mensaje = "El nombre de del proveedor es obligatorio" });

                var existe = await _context.Proveedores
                    .AnyAsync(p => p.Nombre.ToLower() == nombreNormalizado.ToLower());
                if (existe)
                {
                    return BadRequest(new { mensaje = "Ya existe un proveedor con ese nombre, por favor revisa que no esté inactivo" });
                }

                var proveedor = new Proveedor
                {
                    Nombre = nombreNormalizado,
                    Contacto = dto.Contacto,
                    Telefono = dto.Telefono,
                    Correo = dto.Correo,
                    Activo = true
                };

                proveedor.Nombre = dto.Nombre.Trim();
                proveedor.Contacto = string.IsNullOrWhiteSpace(dto.Contacto) ? null : dto.Contacto.Trim();
                proveedor.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
                proveedor.Correo = string.IsNullOrWhiteSpace(dto.Correo) ? null : dto.Correo.Trim();

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

        [Authorize(Policy = Permissions.Proveedores.Actualizar)]
        [HttpPut("actualizar_proveedor/{idProveedor:int}")]
        public async Task<ActionResult<ProveedorResponseDto>> ActualizarProveedor(int idProveedor, [FromBody] ProveedorUpsertDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");
            try
            {
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

        [Authorize(Policy = Permissions.Proveedores.Activar)]
        [HttpPut("activar_proveedor/{idProveedor:int}")]
        public async Task<IActionResult> ActivarProveedor(int idProveedor)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var proveedor = await _context.Proveedores
                    .FirstOrDefaultAsync(m => m.Id == idProveedor && !m.Activo);

                if (proveedor == null)
                    return NotFound(new { mensaje = "Proveedor no encontrado" });

                proveedor.Activo = true;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar al Proveedor: {ex.Message}");
            }
        }

        [Authorize(Policy = Permissions.Proveedores.Desactivar)]
        [HttpDelete("desactivar_proveedor/{idProveedor:int}")]
        public async Task<IActionResult> DesactivarProveedor(int idProveedor)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

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

        [Authorize(Policy = Permissions.Proveedores.Pagar)]
        [HttpPost("registrar_pago_proveedor")]
        public async Task<ActionResult<RegistrarPagoProveedorResponseDto>> RegistrarPagoProveedor([FromBody] RegistrarPagoProveedorUpsertDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "sp_RegistrarPagoProveedor";
                command.CommandType = CommandType.StoredProcedure;

                var metodoPago = await _context.MetodosPago
                    .Where(m => m.Id == request.IdMetodoPago && m.Activo)
                    .Select(m => new MetodosPagoResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Activo = m.Activo,
                        AfectaCaja = m.AfectaCaja
                    })
                    .FirstOrDefaultAsync();

                if (metodoPago == null) return NotFound(new {mensaje = "Metodo de pago inactivo."});

                command.Parameters.Add(new SqlParameter("@IdProveedor", SqlDbType.Int)
                {
                    Value = request.IdProveedor
                });

                command.Parameters.Add(new SqlParameter("@Monto", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = request.Monto
                });

                command.Parameters.Add(new SqlParameter("@MetodoPago", SqlDbType.NVarChar, 50)
                {
                    Value = metodoPago.Nombre
                });

                command.Parameters.Add(new SqlParameter("@Referencia", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(request.Referencia)
                        ? DBNull.Value
                        : request.Referencia.Trim()
                });

                command.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrWhiteSpace(request.Observaciones)
                        ? DBNull.Value
                        : request.Observaciones.Trim()
                });

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return StatusCode(500, "No se recibió respuesta al registrar el pago al proveedor.");

                var response = new RegistrarPagoProveedorResponseDto
                {
                    IdPagoProveedor = reader.GetInt32(reader.GetOrdinal("IdPagoProveedor")),
                    Folio = reader.GetString(reader.GetOrdinal("Folio")),
                    Monto = reader.GetDecimal(reader.GetOrdinal("Monto"))
                };

                return Ok(response);
            }
            catch (SqlException ex) when (
                ex.Number == 50101 ||
                ex.Number == 50102 ||
                ex.Number == 50103 ||
                ex.Number == 50104 ||
                ex.Number == 50105 ||
                ex.Number == 50106 ||
                ex.Number == 50107)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el pago al proveedor: {ex.Message}");
            }
        }

        [Authorize(Policy = Permissions.Proveedores.PagosVer)]
        [HttpGet("listar_pagos_proveedores")]
        public async Task<IActionResult> GetPagosProveedores()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var pagoProveedores = await _context.PagosProveedores
                    .Select(p => new PagosProveedoresResponseDto
                    {
                        Id = p.Id,
                        Folio = p.Folio,
                        NombreProveedor = p.Proveedor.Nombre,
                        Monto = p.Monto,
                        MetodoPago = p.MetodoPago,
                        Referencia = p.Referencia ?? "",
                        Observaciones = p.Observaciones ?? "",
                        FechaPago = p.FechaPago
                    })
                    .ToListAsync();
                return Ok(pagoProveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los pagos a proveedores: {ex.Message}");
            }
        }
    }
}
