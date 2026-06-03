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

        /// <summary>
        /// GET api/Caja/apertura_hoy
        /// Obtiene la apertura de caja activa del d&iacute;a.
        /// 1. Busca en AperturasCaja el registro con Activo = true
        /// 2. Si no hay apertura activa regresa 404
        /// 3. Regresa el Id, fecha, monto inicial, fecha de registro y usuario
        /// </summary>
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

        /// <summary>
        /// POST api/Caja/registrar_apertura
        /// Registra una nueva apertura de caja (inicio de turno).
        /// 1. Obtiene el Id del usuario autenticado desde el JWT
        /// 2. Ejecuta el SP sp_RegistrarAperturaCaja con el monto inicial y el usuario
        /// 3. El SP valida que no haya otra apertura activa (si la hay lanza error 502xx)
        /// 4. Si el SP se ejecuta bien, regresa los datos de la apertura creada
        /// 5. Si el SP lanza SqlException con c&oacute;digo 502xx, regresa 409 Conflict
        /// </summary>
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

        /// <summary>
        /// GET api/Caja/obtener_corte_hoy
        /// Obtiene el estado actual de caja del d&iacute;a y todos los cortes realizados.
        ///
        /// PARTE 1 — Resumen del turno activo (si existe):
        ///   1. Calcula hoy = DateTime.Now.Date, manana = hoy + 1 d&iacute;a (rango del d&iacute;a local)
        ///   2. Busca si hay una apertura de caja activa (Activo = true)
        ///   3. Si hay turno activo:
        ///      a. Toma desde = FechaRegistro de la apertura (inicio del turno)
        ///      b. Suma las ventas en efectivo (IdMetodoPago = 1) desde el inicio del turno hasta manana
        ///      c. Suma los pagos a proveedores de caja (IdMetodoPago = 4) desde el inicio del turno hasta manana
        ///      d. Calcula el esperado en caja = montoInicial + ventas - pagos
        ///      e. Obtiene el nombre del usuario que tiene la caja asignada
        ///
        /// PARTE 2 — Cortes realizados hoy:
        ///   1. Trae todos los cortes de caja del d&iacute;a (FechaCorte entre hoy y manana)
        ///   2. Por cada corte:
        ///      a. Busca el corte anterior m&aacute;s cercano para saber d&oacute;nde inici&oacute; este turno
        ///      b. Si no hay corte anterior, el turno inici&oacute; a la medianoche (hoy)
        ///      c. Obtiene las ventas en efectivo entre el inicio del turno y la fecha del corte
        ///      d. Obtiene los pagos a proveedores de caja entre el inicio del turno y la fecha del corte
        ///      e. Agrega el corte con su detalle a la lista de respuesta
        ///   3. Asigna la lista de cortes al response
        /// </summary>
        [Authorize(Policy = Permissions.CorteCaja.Ver)]
        [HttpGet("obtener_corte_hoy")]
        public async Task<ActionResult<CorteCajaHoyResponseDto>> ObtenerCorteCajaHoy()
        {
            try
            {
                var hoy = DateTime.Now.Date;
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
                        .ToListAsync();

                    var ventasIds = ventas.Select(v => v.IdVenta).ToList();

                    if (ventasIds.Count > 0)
                    {
                        var detallesAgrupados = await _context.DetalleVentas
                            .Where(d => ventasIds.Contains(d.IdVenta))
                            .GroupBy(d => d.IdVenta)
                            .Select(g => new
                            {
                                IdVenta = g.Key,
                                Detalles = g.Select(d => new GenerarVentaDetalleResponseDto
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
                            .ToListAsync();

                        foreach (var grupo in detallesAgrupados)
                        {
                            var venta = ventas.FirstOrDefault(v => v.IdVenta == grupo.IdVenta);
                            if (venta != null)
                            {
                                venta.Detalles = grupo.Detalles;
                            }
                        }
                    }

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

        /// <summary>
        /// POST api/Caja/realizar_corte
        /// Realiza el corte de caja del turno activo.
        ///
        /// PASO 1 — Validaciones previas:
        ///   1. Obtiene el usuario autenticado del JWT
        ///   2. Busca la apertura de caja activa (si no hay, regresa 404)
        ///   3. Obtiene los datos del usuario que hace el corte y del usuario previo (due&ntilde;o de la caja)
        ///
        /// PASO 2 — C&aacute;lculo de montos:
        ///   1. Calcula desde = FechaRegistro de la apertura (inicio del turno)
        ///   2. Suma las ventas en efectivo desde el inicio del turno hasta manana
        ///   3. Suma los pagos a proveedores de caja desde el inicio del turno hasta manana
        ///   4. Calcula montoEsperado = montoInicial + ventas - pagos
        ///   5. Si el retiro solicitado es mayor al esperado, regresa 400
        ///   6. Determina si es corte final (retiro &gt;= montoEsperado)
        ///
        /// PASO 3 — Validaci&oacute;n del usuario receptor (solo cortes parciales):
        ///   1. Si no es corte final, requiere seleccionar un receptor
        ///   2. Verifica que el receptor exista, est&eacute; activo
        ///   3. Verifica que tenga permisos de ventas (Ver o Realizar)
        ///
        /// PASO 4 — Transacci&oacute;n (ExecutionStrategy + Serializable):
        ///   1. Crea el registro CorteCaja con todos los montos calculados
        ///   2. Desactiva la apertura actual (Activo = false)
        ///   3. Si NO es corte final:
        ///      a. Guarda el nombre del usuario receptor en el corte
        ///      b. Crea una NUEVA apertura de caja con el montoFinal como inicial
        ///      c. Asigna la nueva apertura al usuario receptor
        ///   4. Guarda los cambios y hace commit
        ///   5. Si algo falla, hace rollback de todo
        ///
        /// PASO 5 — Respuesta:
        ///   Regresa los datos del corte realizado, incluyendo si se cre&oacute; una nueva apertura
        /// </summary>
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
                var hoy = DateTime.Now.Date;
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
                            FechaCorte = DateTime.Now,
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
                                FechaRegistro = DateTime.Now,
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
