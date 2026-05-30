using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CajaController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("apertura_hoy")]
        public async Task<ActionResult<AperturaCajaResponseDto>> ObtenerAperturaHoy()
        {
            try
            {
                var hoy = DateTime.Today;

                var apertura = await _context.AperturasCaja
                    .AsNoTracking()
                    .Where(a => a.Activo && a.FechaOperacion == hoy)
                    .Select(a => new AperturaCajaResponseDto
                    {
                        Id = a.Id,
                        FechaOperacion = a.FechaOperacion,
                        MontoInicial = a.MontoInicial,
                        FechaRegistro = a.FechaRegistro,
                        IdUsuario = a.IdUsuario
                    })
                    .FirstOrDefaultAsync();

                if (apertura == null)
                    return NotFound(new { mensaje = "No existe apertura de caja para hoy." });

                return Ok(apertura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la apertura de caja: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("registrar_apertura")]
        public async Task<ActionResult<AperturaCajaResponseDto>> RegistrarAperturaCaja(
            [FromBody] RegistrarAperturaCajaUpsertDto request)
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
                command.CommandText = "sp_RegistrarAperturaCaja";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@MontoInicial", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = request.MontoInicial
                });

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return StatusCode(500, "No se recibió respuesta al registrar la apertura de caja.");

                var response = new AperturaCajaResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FechaOperacion = reader.GetDateTime(reader.GetOrdinal("FechaOperacion")),
                    MontoInicial = reader.GetDecimal(reader.GetOrdinal("MontoInicial")),
                    FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                    IdUsuario = reader.GetString(reader.GetOrdinal("IdUsuario"))
                };

                return Ok(response);
            }
            catch (SqlException ex) when (
                ex.Number == 50201 ||
                ex.Number == 50202 ||
                ex.Number == 50203)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la apertura de caja: {ex.Message}");
            }
        }

        [Authorize(Policy = Permissions.CorteCaja.Ver)]
        [HttpGet("obtener_corte_hoy")]
        public async Task<ActionResult<CorteCajaHoyResponseDto>> ObtenerCorteCajaHoy()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "sp_ObtenerCorteCajaHoy";
                command.CommandType = CommandType.StoredProcedure;

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return StatusCode(500, "No se recibió respuesta al obtener el corte de caja.");

                var response = new CorteCajaHoyResponseDto
                {
                    FechaOperacion = reader.GetDateTime(reader.GetOrdinal("FechaOperacion")),
                    MontoInicialCaja = reader.GetDecimal(reader.GetOrdinal("MontoInicialCaja")),
                    MontoVentas = reader.GetDecimal(reader.GetOrdinal("MontoVentas")),
                    MontoPagoProveedores = reader.GetDecimal(reader.GetOrdinal("MontoPagoProveedores")),
                    CorteCaja = reader.GetDecimal(reader.GetOrdinal("CorteCaja"))
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el corte de caja: {ex.Message}");
            }
        }
    }
}