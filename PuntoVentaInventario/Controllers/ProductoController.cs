using Microsoft.AspNetCore.Authorization;
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
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductoController> _logger;
        public ProductoController(AppDbContext context, ILogger<ProductoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtiene todos los productos activos
        [Authorize(Policy = Permissions.Productos.ActivosVer)]
        [HttpGet("listar_productos_activos")]
        public async Task<IActionResult> GetProductosActivos([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var query = _context.Productos
                    .AsNoTracking()
                    .Where(m => m.Activo);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var termino = search.Trim();
                    query = query.Where(m => m.Codigo.Contains(termino) || m.Nombre.Contains(termino));
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(m => m.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new ProductoResponseDto
                    {
                        Id = m.Id,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        Descripcion = m.Descripcion,
                        Costo = m.Costo,
                        Precio = m.Precio,
                        Stock = m.Stock,
                        StockMinimo = m.StockMinimo,
                        Categoria = m.Categoria == null ? null : new CategoriaResponseDto
                        {
                            Id = m.Categoria.Id,
                            Nombre = m.Categoria.Nombre
                        },
                        Marca = m.Marca == null ? null : new MarcaResponseDto
                        {
                            Id = m.Marca.Id,
                            Nombre = m.Marca.Nombre
                        },
                        UnidadMedida = m.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                        {
                            Id = m.UnidadMedida.Id,
                            Nombre = m.UnidadMedida.Nombre,
                            Clave = m.UnidadMedida.Clave,
                            PermiteDecimales = m.UnidadMedida.PermiteDecimales
                        },
                        Proveedores = m.ProductoProveedores
                        .Select(pp => new ProveedorResponseDto
                        {
                            Id = pp.Proveedor.Id,
                            Nombre = pp.Proveedor.Nombre,
                            Contacto = pp.Proveedor.Contacto,
                            Telefono = pp.Proveedor.Telefono,
                            Correo = pp.Proveedor.Correo
                        })
                        .ToList()
                    })
                    .ToListAsync();

                var resultado = new PagedResultDto<ProductoResponseDto>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos activos");
                return StatusCode(500, new { mensaje = "Error interno al obtener productos." });
            }
        }

        // Obtiene una lista sencilla de los productos con stock para realizar la venta, el permiso es con ventas.realizar
        [Authorize(Policy = Permissions.Ventas.Realizar)]
        [HttpGet("listar_productos_venta")]
        public async Task<IActionResult> GetProductosVenta()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(m => m.Activo && m.Stock > 0)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new ProductoSimpleResponseDto
                    {
                        Id = m.Id,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        //Descripcion = m.Descripcion,
                        Costo = m.Costo,
                        Precio = m.Precio,
                        Stock = m.Stock,
                        //StockMinimo = m.StockMinimo,
                        //Categoria = m.Categoria == null ? null : new CategoriaResponseDto
                        //{
                        //    Id = m.Categoria.Id,
                        //    Nombre = m.Categoria.Nombre
                        //},
                        //Marca = m.Marca == null ? null : new MarcaResponseDto
                        //{
                        //    Id = m.Marca.Id,
                        //    Nombre = m.Marca.Nombre
                        //},
                        UnidadMedida = m.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                        {
                            Id = m.UnidadMedida.Id,
                            Nombre = m.UnidadMedida.Nombre,
                            Clave = m.UnidadMedida.Clave,
                            PermiteDecimales = m.UnidadMedida.PermiteDecimales
                        }
                        //Proveedores = m.ProductoProveedores
                        //.Select(pp => new ProveedorResponseDto
                        //{
                        //    Id = pp.Proveedor.Id,
                        //    Nombre = pp.Proveedor.Nombre,
                        //    Contacto = pp.Proveedor.Contacto,
                        //    Telefono = pp.Proveedor.Telefono,
                        //    Correo = pp.Proveedor.Correo
                        //})
                        //.ToList()
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos para venta");
                return StatusCode(500, new { mensaje = "Error interno al obtener productos." });
            }
        }

        // Obtiene todos los productos inactivos
        [Authorize(Policy = Permissions.Productos.InactivosVer)]
        [HttpGet("listar_productos_inactivos")]
        public async Task<IActionResult> GetProductosInactivos()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(m => !m.Activo)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new ProductoResponseDto
                    {
                        Id = m.Id,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        Descripcion = m.Descripcion,
                        Costo = m.Costo,
                        Precio = m.Precio,
                        Stock = m.Stock,
                        StockMinimo = m.StockMinimo,
                        Categoria = m.Categoria == null ? null : new CategoriaResponseDto
                        {
                            Id = m.Categoria.Id,
                            Nombre = m.Categoria.Nombre
                        },
                        Marca = m.Marca == null ? null : new MarcaResponseDto
                        {
                            Id = m.Marca.Id,
                            Nombre = m.Marca.Nombre
                        },
                        UnidadMedida = m.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                        {
                            Id = m.UnidadMedida.Id,
                            Nombre = m.UnidadMedida.Nombre,
                            Clave = m.UnidadMedida.Clave,
                            PermiteDecimales = m.UnidadMedida.PermiteDecimales
                        },
                        Proveedores = m.ProductoProveedores
                        .Select(pp => new ProveedorResponseDto
                        {
                            Id = pp.Proveedor.Id,
                            Nombre = pp.Proveedor.Nombre,
                            Contacto = pp.Proveedor.Contacto,
                            Telefono = pp.Proveedor.Telefono,
                            Correo = pp.Proveedor.Correo
                        })
                        .ToList()
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos inactivos");
                return StatusCode(500, new { mensaje = "Error interno al obtener productos." });
            }
        }

        [Authorize(Policy = Permissions.Productos.Ver)]
        [HttpGet("producto_codigo/{codigo}")]
        public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.UnidadMedida)
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Proveedor)
                .Where(p => p.Codigo == codigo && p.Activo)
                .Select(p => new ProductoResponseDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Costo = p.Costo,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    Categoria = p.Categoria == null ? null : new CategoriaResponseDto
                    {
                        Id = p.Categoria.Id,
                        Nombre = p.Categoria.Nombre
                    },
                    Marca = p.Marca == null ? null : new MarcaResponseDto
                    {
                        Id = p.Marca.Id,
                        Nombre = p.Marca.Nombre
                    },
                    UnidadMedida = p.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                    {
                        Id = p.UnidadMedida.Id,
                        Nombre = p.UnidadMedida.Nombre,
                        Clave = p.UnidadMedida.Clave,
                        PermiteDecimales = p.UnidadMedida.PermiteDecimales
                    },
                    Proveedores = p.ProductoProveedores
                        .Select(pp => new ProveedorResponseDto
                        {
                            Id = pp.Proveedor.Id,
                            Nombre = pp.Proveedor.Nombre,
                            Contacto = pp.Proveedor.Contacto,
                            Telefono = pp.Proveedor.Telefono,
                            Correo = pp.Proveedor.Correo
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return NotFound("Producto no encontrado");

            return Ok(producto);
        }

        [Authorize(Policy = Permissions.Home.StockMinimoVer)]
        [HttpGet("stock_minimo")]
        public async Task<IActionResult> GetProductosStockMinimo()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(m => m.Activo && m.Stock <= m.StockMinimo)
                    .OrderBy(m => m.Stock)
                    .Select(m => new ProductoResponseDto
                    {
                        Id = m.Id,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        Descripcion = m.Descripcion,
                        Costo = m.Costo,
                        Precio = m.Precio,
                        Stock = m.Stock,
                        StockMinimo = m.StockMinimo,
                        //IdCategoria = m.IdCategoria,
                        //Categoria = m.Categoria != null ? m.Categoria.Nombre : null,
                        //IdMarca = m.IdMarca,
                        //Marca = m.Marca != null ? m.Marca.Nombre : null,
                        Categoria = m.Categoria == null ? null : new CategoriaResponseDto
                        {
                            Id = m.Categoria.Id,
                            Nombre = m.Categoria.Nombre
                        },
                        Marca = m.Marca == null ? null : new MarcaResponseDto
                        {
                            Id = m.Marca.Id,
                            Nombre = m.Marca.Nombre
                        },
                        UnidadMedida = m.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                        {
                            Id = m.UnidadMedida.Id,
                            Nombre = m.UnidadMedida.Nombre,
                            Clave = m.UnidadMedida.Clave,
                            PermiteDecimales = m.UnidadMedida.PermiteDecimales
                        },
                        //IdUnidadMedida = m.IdUnidadMedida,
                        //UnidadMedida = m.UnidadMedida != null ? m.UnidadMedida.Nombre : null,
                        //IdsProveedores = m.ProductoProveedores.Select(pp => pp.IdProveedor).ToList(),
                        Proveedores = m.ProductoProveedores
                        .Select(pp => new ProveedorResponseDto
                        {
                            Id = pp.Proveedor.Id,
                            Nombre = pp.Proveedor.Nombre,
                            Contacto = pp.Proveedor.Contacto,
                            Telefono = pp.Proveedor.Telefono,
                            Correo = pp.Proveedor.Correo
                        })
                        .ToList()
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con stock mínimo");
                return StatusCode(500, new { mensaje = "Error interno al obtener productos." });
            }
            //try
            //{
            //    var productos = await _context.Productos
            //        .FromSqlRaw("sp_ProductosObtenerStockMinimo")
            //        .ToListAsync();

            //    return Ok(productos);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Error al obtener los productos con stock minimo: {ex.Message}");
            //}
        }

        [Authorize(Policy = Permissions.Productos.Crear)]
        [HttpPost("crear_producto")]
        public async Task<IActionResult> InsertarProducto([FromBody] ProductoUpsertDto dto)
        {
            dto.Codigo = dto.Codigo.Trim();

            if (await _context.Productos.AnyAsync(p => p.Codigo.Trim() == dto.Codigo))
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe un producto con ese código."
                });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Costo = dto.Costo,
                Precio = dto.Precio,
                Stock = dto.Stock,
                StockMinimo = dto.StockMinimo,
                IdCategoria = dto.IdCategoria,
                IdMarca = dto.IdMarca,
                IdUnidadMedida = dto.IdUnidadMedida,
                FechaCreacion = DateTime.Now,
                IdUsuarioCreacion = userIdClaim,
                Activo = true
            };

            if (dto.IdsProveedores?.Any() == true)
            {
                producto.ProductoProveedores = dto.IdsProveedores
                    .Distinct()
                    .Select(idProveedor => new ProductoProveedor
                    {
                        IdProveedor = idProveedor,
                        Producto = producto
                    })
                    .ToList();
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductoPorCodigo), new { codigo = producto.Codigo }, new
            {
                producto.Id,
                producto.Codigo,
                producto.Nombre
            });
        }

        [Authorize(Policy = Permissions.Productos.Actualizar)]
        [HttpPut("actualizar_producto")]
        public async Task<ActionResult<ProductoResponseDto>> ActualizarProducto([FromBody] ProductoUpsertDto dto)
        {
            dto.Codigo = dto.Codigo.Trim();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.UnidadMedida)
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Proveedor)
                .FirstOrDefaultAsync(p => p.Codigo.Trim() == dto.Codigo && p.Activo);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Costo = dto.Costo;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.StockMinimo = dto.StockMinimo;
            producto.IdCategoria = dto.IdCategoria;
            producto.IdMarca = dto.IdMarca;
            producto.IdUnidadMedida = dto.IdUnidadMedida;
            producto.FechaModificacion = DateTime.Now;
            producto.IdUsuarioModificacion = userIdClaim;

            var proveedoresNuevos = dto.IdsProveedores?.Distinct().ToHashSet() ?? new HashSet<int>();

            var eliminar = producto.ProductoProveedores
                .Where(pp => !proveedoresNuevos.Contains(pp.IdProveedor))
                .ToList();

            foreach (var item in eliminar)
            {
                producto.ProductoProveedores.Remove(item);
            }

            var proveedoresActuales = producto.ProductoProveedores
                .Select(pp => pp.IdProveedor)
                .ToHashSet();

            foreach (var idProveedor in proveedoresNuevos.Except(proveedoresActuales))
            {
                producto.ProductoProveedores.Add(new ProductoProveedor
                {
                    IdProducto = producto.Id,
                    IdProveedor = idProveedor
                });
            }

            await _context.SaveChangesAsync();

            producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.UnidadMedida)
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Proveedor)
                .FirstAsync(p => p.Id == producto.Id);

            var response = new ProductoResponseDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Costo = producto.Costo,
                Precio = producto.Precio,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,

                Categoria = producto.Categoria == null ? null : new CategoriaResponseDto
                {
                    Id = producto.Categoria.Id,
                    Nombre = producto.Categoria.Nombre
                },

                Marca = producto.Marca == null ? null : new MarcaResponseDto
                {
                    Id = producto.Marca.Id,
                    Nombre = producto.Marca.Nombre
                },

                UnidadMedida = producto.UnidadMedida == null ? null : new UnidadMedidaResponseDto
                {
                    Id = producto.UnidadMedida.Id,
                    Nombre = producto.UnidadMedida.Nombre,
                    Clave = producto.UnidadMedida.Clave,
                    PermiteDecimales = producto.UnidadMedida.Activo
                },

                Proveedores = producto.ProductoProveedores
                    .Where(pp => pp.Proveedor != null)
                    .Select(pp => new ProveedorResponseDto
                    {
                        Id = pp.Proveedor.Id,
                        Nombre = pp.Proveedor.Nombre
                    })
                    .ToList()
            };

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Productos.Activar)]
        [HttpPut("activar_producto/{idProducto}")]
        public async Task<IActionResult> ActivarProducto(int idProducto)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == idProducto && !p.Activo && p.FechaEliminacion != null);

            if (producto == null)
                return NotFound("Producto no encontrado");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            producto.Activo = true;
            producto.FechaEliminacion = null;
            producto.IdUsuarioEliminacion = null;
            producto.FechaModificacion = DateTime.Now;
            producto.IdUsuarioModificacion = userIdClaim;

            await _context.SaveChangesAsync();
            return NoContent();  // 204 Success
        }

        [Authorize(Policy = Permissions.Productos.Desactivar)]
        [HttpDelete("desactivar_producto/{idProducto}")]
        public async Task<IActionResult> DesactivarProducto(int idProducto)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == idProducto && p.Activo);

            if (producto == null)
                return NotFound("Producto no encontrado");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            // Soft delete: solo marca como inactivo
            producto.Activo = false;
            producto.FechaEliminacion = DateTime.Now;
            producto.IdUsuarioEliminacion = userIdClaim;  // Usuario logueado

            await _context.SaveChangesAsync();
            return NoContent();  // 204 Success
        }
    }
}
