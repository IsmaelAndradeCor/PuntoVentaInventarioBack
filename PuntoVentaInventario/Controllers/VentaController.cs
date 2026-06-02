using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace PuntoVentaInventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VentaController> _logger;

        public VentaController(AppDbContext context, ILogger<VentaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = Permissions.Ventas.HistorialVer)]
        [HttpGet("generar_ventas")]
        public async Task<IActionResult> GetGenerarVentas([FromQuery] GenerarVentasRequestDto request)
        {
            try
            {
                var query = _context.Ventas
                    .AsNoTracking()
                    .Where(v =>
                        (!request.FechaInicio.HasValue || v.FechaVenta.Date >= request.FechaInicio.Value.Date) &&
                        (!request.FechaFin.HasValue || v.FechaVenta.Date <= request.FechaFin.Value.Date));

                if (request.IncluirDetalle)
                {
                    var ventasConDetalle = await query
                        .Select(v => new GenerarVentaResponseDto
                        {
                            IdVenta = v.Id,
                            Folio = v.Folio,
                            FechaVenta = v.FechaVenta,
                            Subtotal = v.Subtotal,
                            Descuento = v.Descuento,
                            CostoTotal = v.Detalles.Sum(d => (decimal?)d.CostoTotal) ?? 0,
                            Total = v.Total,
                            Ganancias = v.Total - (v.Detalles.Sum(d => (decimal?)d.CostoTotal) ?? 0),
                            IdMetodoPago = v.IdMetodoPago,
                            MetodoPago = v.MetodoPago.Nombre,
                            Detalles = v.Detalles.Select(d => new GenerarVentaDetalleResponseDto
                            {
                                IdDetalleVenta = d.Id,
                                IdProducto = d.IdProducto,
                                CodigoProducto = d.CodigoProducto,
                                NombreProducto = d.NombreProducto,
                                Cantidad = d.Cantidad,
                                CostoUnitario = d.CostoUnitario,
                                CostoTotal = d.CostoTotal,
                                PrecioUnitario = d.PrecioUnitario,
                                PrecioTotal = d.PrecioTotal
                            }).ToList()
                        })
                        .OrderByDescending(v => v.FechaVenta)
                        .ToListAsync();

                    return Ok(ventasConDetalle);
                }

                var ventasSinDetalle = await query
                    .Select(v => new GenerarVentaResponseDto
                    {
                        IdVenta = v.Id,
                        Folio = v.Folio,
                        FechaVenta = v.FechaVenta,
                        Subtotal = v.Subtotal,
                        Descuento = v.Descuento,
                        CostoTotal = v.Detalles.Sum(d => (decimal?)d.CostoTotal) ?? 0,
                        Total = v.Total,
                        Ganancias = v.Total - (v.Detalles.Sum(d => (decimal?)d.CostoTotal) ?? 0),
                        IdMetodoPago = v.IdMetodoPago,
                        MetodoPago = v.MetodoPago.Nombre,
                        Detalles = new List<GenerarVentaDetalleResponseDto>()
                    })
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                return Ok(ventasSinDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar ventas");
                return StatusCode(500, new { mensaje = "Error interno al generar ventas." });
            }
        }

        [Authorize(Policy = Permissions.Ventas.Realizar)]
        [HttpPost("realizar_venta")]
        public async Task<ActionResult<RegistrarVentaResponseDto>> RegistrarVenta([FromBody] RegistrarVentaUpsertDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var aperturaActiva = await _context.AperturasCaja
                    .Where(a => a.Activo)
                    .FirstOrDefaultAsync();

                if (aperturaActiva == null)
                    return BadRequest(new
                    {
                        mensaje = "No hay un turno de caja activo. Debes abrir caja antes de realizar una venta.",
                        codigo = "SIN_CAJA_ACTIVA"
                    });

                if (aperturaActiva.IdUsuario != userIdClaim)
                    return BadRequest(new
                    {
                        mensaje = "La caja está asignada a otro usuario. Solo el portador de la caja puede realizar ventas.",
                        codigo = "CAJA_ASIGNADA_OTRO_USUARIO",
                        idUsuarioActivo = aperturaActiva.IdUsuario
                    });

                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "sp_RegistrarVenta";
                command.CommandType = CommandType.StoredProcedure;

                var detalleJson = JsonSerializer.Serialize(
                    request.Detalles.Select(d => new
                    {
                        idProducto = d.IdProducto,
                        cantidad = d.Cantidad
                    }));

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

                if (metodoPago == null) return NotFound(new { mensaje = "Metodo de pago inactivo." });

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                command.Parameters.Add(new SqlParameter("@Detalle", SqlDbType.NVarChar)
                {
                    Value = detalleJson
                });

                command.Parameters.Add(new SqlParameter("@IdMetodoPago", SqlDbType.Int)
                {
                    Value = metodoPago.Id
                });

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return StatusCode(500, "No se recibió respuesta al registrar la venta.");

                var response = new RegistrarVentaResponseDto
                {
                    IdVenta = reader.GetInt32(reader.GetOrdinal("IdVenta")),
                    Folio = reader.GetString(reader.GetOrdinal("Folio")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                };

                return Ok(response);
            }
            catch (SqlException ex) when (
                ex.Number == 50001 ||
                ex.Number == 50002 ||
                ex.Number == 50003 ||
                ex.Number == 50004 ||
                ex.Number == 50005 ||
                ex.Number == 50006 ||
                ex.Number == 50007 ||
                ex.Number == 50008 ||
                ex.Number == 50009 ||
                ex.Number == 50010 ||
                ex.Number == 50011)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la venta");
                return StatusCode(500, new { mensaje = "Error interno al registrar la venta." });
            }
        }
        
        [Authorize(Policy = Permissions.Ventas.Realizar)]
        [HttpGet("obtener-metodos-pago")]
        public async Task<IActionResult> ObtenerMetodosPago()
        {
            try
            {
                var metodosPago = await _context.MetodosPago
                    .Select(m => new MetodosPagoResponseDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Activo = m.Activo,
                        AfectaCaja = m.AfectaCaja
                    })
                    .ToListAsync();

                return Ok(metodosPago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los métodos de pago");
                return StatusCode(500, new { mensaje = "Error interno al obtener los métodos de pago." });
            }
        }
    }
}