using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Requests;
using System.Data;
using System.Security.Claims;

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
        public async Task<ActionResult<int>> RegistrarVenta([FromBody] RegistrarVentaUpsertDto request)
        {
            if (request == null)
                return BadRequest("Datos no válidos.");

            if (string.IsNullOrWhiteSpace(request.Folio))
                return BadRequest("El folio es obligatorio.");

            if (request.Total <= 0)
                return BadRequest("El total debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(request.Detalle))
                return BadRequest("El detalle XML es obligatorio.");

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

                command.Parameters.Add(new SqlParameter("@Folio", SqlDbType.NVarChar, 20)
                {
                    Value = request.Folio
                });

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.NVarChar, 450)
                {
                    Value = userIdClaim
                });

                command.Parameters.Add(new SqlParameter("@Total", SqlDbType.Decimal)
                {
                    Precision = 10,
                    Scale = 2,
                    Value = request.Total
                });

                command.Parameters.Add(new SqlParameter("@Detalle", SqlDbType.Xml)
                {
                    Value = request.Detalle
                });

                var result = await command.ExecuteScalarAsync();
                int idVenta = Convert.ToInt32(result);

                return Ok(idVenta);
            }
            catch (SqlException ex) when (ex.Number == 50001 || ex.Number == 50002)
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