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

        public VentaController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = Permissions.Ventas.HistorialVer)]
        [HttpGet("generar_ventas")]
        public async Task<IActionResult> GetGenerarVentas()
        {
            try
            {
                var ventas = await _context.GenerarVentasDto
                    .FromSqlRaw("EXEC sp_GenerarVentas")
                    .ToListAsync();

                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
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

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                command.Parameters.Add(new SqlParameter("@Detalle", SqlDbType.NVarChar)
                {
                    Value = detalleJson
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
                ex.Number == 50010)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
            }
        }
    }
}