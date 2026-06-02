using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Authorization;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models;
using PuntoVentaInventario.Models.Dtos.Requests;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CajaController> _logger;

        public CajaController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CajaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("apertura_hoy")]
        public async Task<ActionResult<AperturaCajaResponseDto>> ObtenerAperturaHoy()
        {
            try
            {
                var apertura = await _context.AperturasCaja
                    .AsNoTracking()
                    .Where(a => a.Activo)
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
                    return NotFound(new { mensaje = "No hay ningun turno de caja activo." });

                return Ok(apertura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la apertura de caja");
                return StatusCode(500, new { mensaje = "Error interno al obtener la apertura de caja." });
            }
        }

        [Authorize]
        [HttpPost("registrar_apertura")]
        public async Task<ActionResult<AperturaCajaResponseDto>> RegistrarAperturaCaja(
            [FromBody] RegistrarAperturaCajaUpsertDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized(new { mensaje = "No se pudo obtener el usuario autenticado." });

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
                    return StatusCode(500, new { mensaje = "No se recibió respuesta al registrar la apertura de caja." });

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
                _logger.LogWarning(ex, "Conflicto al registrar apertura de caja (código {ErrorNumber})", ex.Number);
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la apertura de caja");
                return StatusCode(500, new { mensaje = "Error interno al registrar la apertura de caja." });
            }
        }

        [Authorize(Policy = Permissions.CorteCaja.Ver)]
        [HttpGet("obtener_corte_hoy")]
        public async Task<ActionResult<CorteCajaHoyResponseDto>> ObtenerCorteCajaHoy()
        {
            try
            {
                var hoy = DateTime.UtcNow.Date;
                var manana = hoy.AddDays(1);

                var aperturaActiva = await _context.AperturasCaja
                    .Where(a => a.Activo)
                    .FirstOrDefaultAsync();

                var response = new CorteCajaHoyResponseDto
                {
                    FechaOperacion = hoy,
                    CortePendiente = aperturaActiva != null,
                    IdAperturaActiva = aperturaActiva?.Id
                };

                if (aperturaActiva != null)
                {
                    var desde = aperturaActiva.FechaRegistro;

                    var montoVentas = await _context.Ventas
                        .Where(v => v.FechaVenta >= desde && v.FechaVenta < manana && v.IdMetodoPago == MetodoPagoConstants.Efectivo)
                        .SumAsync(v => (decimal?)v.Total) ?? 0;

                    var montoPagos = await _context.PagosProveedores
                        .Where(p => p.FechaPago >= desde && p.FechaPago < manana && p.IdMetodoPago == MetodoPagoConstants.Caja && p.Activo)
                        .SumAsync(p => (decimal?)p.Monto) ?? 0;

                    response.MontoInicialCaja = aperturaActiva.MontoInicial;
                    response.MontoVentas = montoVentas;
                    response.MontoPagoProveedores = montoPagos;
                    response.CorteCaja = aperturaActiva.MontoInicial + montoVentas - montoPagos;

                    response.IdUsuarioActivo = aperturaActiva.IdUsuario;
                    var usuarioActivo = await _userManager.FindByIdAsync(aperturaActiva.IdUsuario);
                    response.NombreUsuarioActivo = usuarioActivo?.NombreCompleto;
                }

                var cortes = await _context.CortesCaja
                    .AsNoTracking()
                    .Where(c => c.FechaCorte >= hoy && c.FechaCorte < manana)
                    .OrderByDescending(c => c.FechaCorte)
                    .ToListAsync();

                var cortesDto = new List<CorteRealizadoDto>();

                foreach (var c in cortes)
                {
                    var corteAnterior = await _context.CortesCaja
                        .Where(cc => cc.FechaCorte < c.FechaCorte && cc.FechaCorte >= hoy)
                        .OrderByDescending(cc => cc.FechaCorte)
                        .FirstOrDefaultAsync();

                    var inicioTurno = corteAnterior?.FechaCorte ?? hoy;

                    var ventas = await _context.Ventas
                        .Where(v => v.FechaVenta >= inicioTurno && v.FechaVenta <= c.FechaCorte && v.IdMetodoPago == MetodoPagoConstants.Efectivo)
                        .Select(v => new CorteDetalleVentaDto
                        {
                            IdVenta = v.Id,
                            Folio = v.Folio,
                            FechaVenta = v.FechaVenta,
                            Total = v.Total
                        })
                        .ToListAsync();

                    var pagos = await _context.PagosProveedores
                        .Where(p => p.FechaPago >= inicioTurno && p.FechaPago <= c.FechaCorte && p.IdMetodoPago == MetodoPagoConstants.Caja && p.Activo)
                        .Select(p => new CorteDetallePagoDto
                        {
                            IdPago = p.Id,
                            Folio = p.Folio,
                            FechaPago = p.FechaPago,
                            Monto = p.Monto,
                            Proveedor = p.Proveedor.Nombre
                        })
                        .ToListAsync();

                    cortesDto.Add(new CorteRealizadoDto
                    {
                        Id = c.Id,
                        FechaCorte = c.FechaCorte,
                        MontoInicial = c.MontoInicial,
                        MontoVentasEfectivo = c.MontoVentasEfectivo,
                        MontoPagoProveedores = c.MontoPagoProveedores,
                        MontoEsperado = c.MontoEsperado,
                        Retiro = c.Retiro,
                        MontoFinal = c.MontoFinal,
                        IdUsuarioPrevio = c.IdUsuarioPrevio,
                        NombreUsuarioPrevio = c.NombreUsuarioPrevio,
                        IdUsuarioCorte = c.IdUsuarioCorte,
                        NombreUsuarioCorte = c.NombreUsuarioCorte,
                        IdUsuarioRecepcion = c.IdUsuarioRecepcion,
                        NombreUsuarioRecepcion = c.NombreUsuarioRecepcion,
                        CorteFinal = c.MontoFinal == 0,
                        Observaciones = c.Observaciones,
                        Ventas = ventas,
                        PagosProveedores = pagos
                    });
                }

                response.CortesRealizados = cortesDto;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el corte de caja");
                return StatusCode(500, new { mensaje = "Error interno al obtener el corte de caja." });
            }
        }

        [Authorize(Roles = "Administrador")]
        [Authorize(Policy = Permissions.CorteCaja.Realizar)]
        [HttpPost("realizar_corte")]
        public async Task<ActionResult<CorteCajaResponseDto>> RealizarCorteCaja(
            [FromBody] RealizarCorteUpsertDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized(new { mensaje = "No se pudo obtener el usuario autenticado." });

            try
            {
                var apertura = await _context.AperturasCaja
                    .Where(a => a.Activo)
                    .FirstOrDefaultAsync();

                if (apertura == null)
                    return NotFound(new { mensaje = "No hay ningun turno de caja activo para realizar el corte." });

                var usuarioCorte = await _userManager.FindByIdAsync(userIdClaim);
                if (usuarioCorte == null)
                    return Unauthorized(new { mensaje = "No se pudo obtener la informacion del usuario que realiza el corte." });

                var usuarioPrevio = await _userManager.FindByIdAsync(apertura.IdUsuario);
                var nombrePrevio = usuarioPrevio?.NombreCompleto ?? "Desconocido";

                var desde = apertura.FechaRegistro;
                var hoy = DateTime.UtcNow.Date;
                var manana = hoy.AddDays(1);

                var montoVentas = await _context.Ventas
                    .Where(v => v.FechaVenta >= desde && v.FechaVenta < manana && v.IdMetodoPago == MetodoPagoConstants.Efectivo)
                    .SumAsync(v => (decimal?)v.Total) ?? 0;

                var montoPagos = await _context.PagosProveedores
                    .Where(p => p.FechaPago >= desde && p.FechaPago < manana && p.IdMetodoPago == MetodoPagoConstants.Caja && p.Activo)
                    .SumAsync(p => (decimal?)p.Monto) ?? 0;

                var montoEsperado = apertura.MontoInicial + montoVentas - montoPagos;

                if (request.Retiro > montoEsperado)
                    return BadRequest(new { mensaje = "El retiro no puede ser mayor al monto esperado en caja." });

                var esCorteFinal = request.Retiro >= montoEsperado;

                if (!esCorteFinal && string.IsNullOrWhiteSpace(request.IdUsuarioRecepcion))
                    return BadRequest(new { mensaje = "Para un corte parcial debes seleccionar a quién se entrega la caja." });

                if (!esCorteFinal)
                {
                    var usuarioRecepcion = await _userManager.FindByIdAsync(request.IdUsuarioRecepcion!);
                    if (usuarioRecepcion == null)
                        return BadRequest(new { mensaje = "El usuario receptor seleccionado no existe." });

                    if (!usuarioRecepcion.Activo)
                        return BadRequest(new { mensaje = "El usuario receptor seleccionado esta inactivo." });

                    var claimsRecepcion = await _userManager.GetClaimsAsync(usuarioRecepcion);
                    var permisosRecepcion = claimsRecepcion
                        .Where(c => c.Type == "permission")
                        .Select(c => c.Value)
                        .ToHashSet();

                    var rolesRecepcion = await _userManager.GetRolesAsync(usuarioRecepcion);
                    var esAdmin = rolesRecepcion.Contains("Administrador");

                    if (!esAdmin &&
                        !permisosRecepcion.Contains(Permissions.Ventas.Ver) &&
                        !permisosRecepcion.Contains(Permissions.Ventas.Realizar))
                    {
                        return BadRequest(new
                        {
                            mensaje = $"El usuario '{usuarioRecepcion.NombreCompleto}' no tiene permisos de ventas ({Permissions.Ventas.Ver} o {Permissions.Ventas.Realizar}). No puedes asignarle la caja."
                        });
                    }
                }

                var montoFinal = montoEsperado - request.Retiro;
                var aperturaId = apertura.Id;
                var nombreCorte = usuarioCorte.NombreCompleto;

                var strategy = _context.Database.CreateExecutionStrategy();
                CorteCajaResponseDto response = null!;

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                    try
                    {
                        var corte = new CorteCaja
                        {
                            IdAperturaCaja = aperturaId,
                            FechaCorte = DateTime.UtcNow,
                            MontoInicial = apertura.MontoInicial,
                            MontoVentasEfectivo = montoVentas,
                            MontoPagoProveedores = montoPagos,
                            MontoEsperado = montoEsperado,
                            Retiro = request.Retiro,
                            MontoFinal = montoFinal,
                            IdUsuarioPrevio = apertura.IdUsuario,
                            NombreUsuarioPrevio = nombrePrevio,
                            IdUsuarioCorte = userIdClaim,
                            NombreUsuarioCorte = nombreCorte,
                            IdUsuarioRecepcion = esCorteFinal ? null : request.IdUsuarioRecepcion,
                            Observaciones = request.Observaciones
                        };

                        _context.CortesCaja.Add(corte);

                        var aperturaDb = await _context.AperturasCaja
                            .FirstAsync(a => a.Id == aperturaId);
                        aperturaDb.Activo = false;

                        int? nuevoIdApertura = null;
                        decimal? nuevoMontoInicial = null;

                        if (!esCorteFinal)
                        {
                            var usuarioRecepcion = await _userManager.FindByIdAsync(request.IdUsuarioRecepcion!);
                            var nombreRecepcion = usuarioRecepcion?.NombreCompleto;
                            corte.NombreUsuarioRecepcion = nombreRecepcion;

                            var nuevaApertura = new AperturaCaja
                            {
                                FechaOperacion = hoy,
                                MontoInicial = montoFinal,
                                FechaRegistro = DateTime.UtcNow,
                                IdUsuario = request.IdUsuarioRecepcion!,
                                Activo = true
                            };

                            _context.AperturasCaja.Add(nuevaApertura);
                            await _context.SaveChangesAsync();

                            nuevoIdApertura = nuevaApertura.Id;
                            nuevoMontoInicial = nuevaApertura.MontoInicial;
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        response = new CorteCajaResponseDto
                        {
                            Id = corte.Id,
                            IdAperturaCaja = corte.IdAperturaCaja,
                            FechaCorte = corte.FechaCorte,
                            MontoInicial = corte.MontoInicial,
                            MontoVentasEfectivo = corte.MontoVentasEfectivo,
                            MontoPagoProveedores = corte.MontoPagoProveedores,
                            MontoEsperado = corte.MontoEsperado,
                            Retiro = corte.Retiro,
                            MontoFinal = corte.MontoFinal,
                            IdUsuarioPrevio = corte.IdUsuarioPrevio,
                            NombreUsuarioPrevio = corte.NombreUsuarioPrevio,
                            IdUsuarioCorte = corte.IdUsuarioCorte,
                            NombreUsuarioCorte = corte.NombreUsuarioCorte,
                            IdUsuarioRecepcion = corte.IdUsuarioRecepcion,
                            NombreUsuarioRecepcion = corte.NombreUsuarioRecepcion,
                            Observaciones = corte.Observaciones,
                            NuevoIdApertura = nuevoIdApertura,
                            NuevoMontoInicial = nuevoMontoInicial,
                            CorteFinal = esCorteFinal
                        };
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el corte de caja");
                return StatusCode(500, new { mensaje = "Error interno al realizar el corte de caja." });
            }
        }
    }
}
