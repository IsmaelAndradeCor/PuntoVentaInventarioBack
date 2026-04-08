using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models;
using System.Data;

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

        [HttpPost("realizar_venta")]
        public async Task<ActionResult<int>> RegistrarVenta([FromBody] RegistrarVentaDto request)
        {
            if (request == null)
                return BadRequest("Datos no válidos.");

            if (string.IsNullOrWhiteSpace(request.Folio))
                return BadRequest("El folio es obligatorio.");

            if (request.Total <= 0)
                return BadRequest("El total debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(request.Detalle))
                return BadRequest("El detalle XML es obligatorio.");

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

                command.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int)
                {
                    Value = request.IdUsuario
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
            }
        }
    }
}