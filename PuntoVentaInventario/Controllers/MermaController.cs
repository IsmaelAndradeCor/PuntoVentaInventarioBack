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
    public class MermaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MermaController> _logger;

        public MermaController(AppDbContext context, ILogger<MermaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = Permissions.Mermas.Registrar)]
        [HttpPost("registrar_merma")]
        public async Task<ActionResult<RegistrarMermaResponseDto>> RegistrarMerma([FromBody] RegistrarMermaUpsertDto request)
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
                command.CommandText = "sp_RegistrarMerma";
                command.CommandType = CommandType.StoredProcedure;

                var detalleJson = JsonSerializer.Serialize(
                    request.Detalles.Select(d => new
                    {
                        idProducto = d.IdProducto,
                        cantidad = d.Cantidad
                    }));

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                command.Parameters.Add(new SqlParameter("@Detalle", SqlDbType.NVarChar)
                {
                    Value = detalleJson
                });

                command.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrWhiteSpace(request.Observaciones)
                        ? DBNull.Value
                        : request.Observaciones.Trim()
                });

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return StatusCode(500, "No se recibió respuesta al registrar la merma.");

                var response = new RegistrarMermaResponseDto
                {
                    IdMerma = reader.GetInt32(reader.GetOrdinal("IdMerma")),
                    Folio = reader.GetString(reader.GetOrdinal("Folio")),
                    CostoTotal = reader.GetDecimal(reader.GetOrdinal("CostoTotal"))
                };

                return Ok(response);
            }
            catch (SqlException ex) when (
                ex.Number == 50301 ||
                ex.Number == 50302 ||
                ex.Number == 50303 ||
                ex.Number == 50304 ||
                ex.Number == 50305 ||
                ex.Number == 50306 ||
                ex.Number == 50307 ||
                ex.Number == 50308 ||
                ex.Number == 50309 ||
                ex.Number == 50310)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la merma");
                return StatusCode(500, new { mensaje = "Error interno al registrar la merma." });
            }
        }

        [Authorize(Policy = Permissions.Mermas.HistorialVer)]
        [HttpGet("listar_mermas")]
        public async Task<IActionResult> GetMermas([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var query = _context.Mermas
                    .AsNoTracking();

                if (fechaInicio.HasValue)
                    query = query.Where(m => m.FechaMerma.Date >= fechaInicio.Value.Date);

                if (fechaFin.HasValue)
                    query = query.Where(m => m.FechaMerma.Date <= fechaFin.Value.Date);

                var mermas = await query
                    .OrderByDescending(m => m.FechaMerma)
                    .Select(m => new MermaResponseDto
                    {
                        Id = m.Id,
                        Folio = m.Folio,
                        FechaMerma = m.FechaMerma,
                        CostoTotal = m.CostoTotal,
                        Observaciones = m.Observaciones,
                        IdUsuario = m.IdUsuario,
                        Detalles = m.Detalles.Select(d => new MermaDetalleResponseDto
                        {
                            Id = d.Id,
                            IdProducto = d.IdProducto,
                            CodigoProducto = d.CodigoProducto,
                            NombreProducto = d.NombreProducto,
                            Cantidad = d.Cantidad,
                            CostoUnitario = d.CostoUnitario,
                            CostoTotal = d.CostoTotal
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(mermas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar mermas");
                return StatusCode(500, new { mensaje = "Error interno al listar mermas." });
            }
        }
    }
}
