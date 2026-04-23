using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models.Dtos.Responses;
using PuntoVentaInventario.Models.Entities;
using System.Data;

namespace PuntoVentaInventario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase  // Cambia a ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;  // ← AGREGAR: DbContext
        // ← MODIFICAR constructor:
        public ProductoController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;  // ← Inyecta
        }

        //// Obtiene todos los productos activos
        //[HttpGet("listar_productos")]
        //public async Task<IActionResult> GetProductos()
        //{
        //    try
        //    {
        //        var productos = await _context.ProductosDto
        //            .FromSqlRaw("EXEC SP_ProductosListar")
        //            .ToListAsync();

        //        return Ok(productos);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error al obtener los productos: {ex.Message}");
        //    }
        //    //var productos = await _context.Productos
        //    //    .Where(p => p.Activo)
        //    //    .Select(p => new ProductoDto  // ← Mapeo directo EF → DTO
        //    //    {
        //    //        Id = p.Id,
        //    //        Codigo = p.Codigo,
        //    //        Nombre = p.Nombre,
        //    //        Descripcion = p.Descripcion,
        //    //        PrecioCompra = p.PrecioCompra,
        //    //        PrecioVenta = p.PrecioVenta,
        //    //        Stock = p.Stock,
        //    //        StockMinimo = p.StockMinimo,
        //    //        Categoria = p.Categoria,
        //    //        Proveedor = p.Proveedor
        //    //    })
        //    //    .ToListAsync();
        //    //return Ok(productos);
        //}
        [HttpGet("listar_productos")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(m => m.Activo)
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
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }

        /// <summary>
        /// Funcion para obtener un producto por su código utilizando el procedimiento almacenado SP_ProductosObtener
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns>productoDto</returns>
        //[HttpGet("producto_codigo/{codigo}")]
        //public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        //{
        //    try
        //    {
        //        var producto = (await _context.ProductosDto
        //            .FromSqlInterpolated($"EXEC sp_ProductosObtener @Codigo={codigo}")
        //            .AsNoTracking()
        //            .ToListAsync())
        //            .FirstOrDefault();

        //        if (producto == null)
        //            return NotFound("Producto no encontrado");

        //        return Ok(producto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error al obtener producto: {ex.Message}");
        //    }
        //}
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
                    //IdCategoria = p.IdCategoria,
                    //Categoria = p.Categoria != null ? p.Categoria.Nombre : null,
                    //IdMarca = p.IdMarca,
                    //Marca = p.Marca != null ? p.Marca.Nombre : null,
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
                    //IdUnidadMedida = p.IdUnidadMedida,
                    //UnidadMedida = p.UnidadMedida != null ? p.UnidadMedida.Nombre : null,

                    //IdsProveedores = p.ProductoProveedores.Select(pp => pp.IdProveedor).ToList(),
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

        [HttpGet("stock_minimo")]
        public async Task<IActionResult> GetProductosStockMinimo()
        {
            try
            {
                var productos = await _context.ProductosDto
                    .FromSqlRaw("sp_ProductosObtenerStockMinimo")
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
            }
        }

        //// Inserta nuevo producto
        //[HttpPost("crear_producto")]
        //public async Task<IActionResult> InsertarProducto([FromBody] ProductoDto productoDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Verificar código único
        //    if (await _context.Productos.AnyAsync(p => p.Codigo == productoDto.Codigo))
        //        return BadRequest(new { mensaje = "Código ya existe" });

        //    var producto = new Producto
        //    {
        //        Codigo = productoDto.Codigo,
        //        Nombre = productoDto.Nombre,
        //        Descripcion = productoDto.Descripcion,
        //        PrecioCompra = productoDto.PrecioCompra,
        //        PrecioVenta = productoDto.PrecioVenta,
        //        Stock = productoDto.Stock,
        //        StockMinimo = productoDto.StockMinimo,
        //        Categoria = productoDto.Categoria,
        //        Proveedor = productoDto.Proveedor,
        //        FechaCreacion = DateTime.UtcNow,
        //        IdUsuarioCreacion = 1,
        //        Activo = true
        //    };

        //    _context.Productos.Add(producto);
        //    await _context.SaveChangesAsync();

        //    // ← CAMBIO: Retorna DTO (no Entity)
        //    var dto = new ProductoDto
        //    {
        //        Id = producto.Id,
        //        Codigo = producto.Codigo,
        //        Nombre = producto.Nombre,
        //        Descripcion = producto.Descripcion,
        //        PrecioCompra = producto.PrecioCompra,
        //        PrecioVenta = producto.PrecioVenta,
        //        Stock = producto.Stock,
        //        StockMinimo = producto.StockMinimo,
        //        Categoria = producto.Categoria,
        //        Proveedor = producto.Proveedor
        //    };
        //    return CreatedAtAction(nameof(GetProductoPorCodigo), new { codigo = producto.Codigo }, dto);
        //}


        [HttpPost("crear_producto")]
        public async Task<IActionResult> InsertarProducto([FromBody] ProductoUpsertDto dto)
        {

            if (await _context.Productos.AnyAsync(p => p.Codigo == dto.Codigo))
            {
                ModelState.AddModelError(nameof(dto.Codigo), "El código ya existe.");
            }

            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    mensaje = "Errores de validación",
                    errors = errores
                });
            }

            //if (await _context.Productos.AnyAsync(p => p.Codigo == dto.Codigo))
            //    return BadRequest(new
            //    {
            //        mensaje = "Errores de validación",
            //        errors = new Dictionary<string, string[]>
            //        {
            //            { nameof(dto.Codigo), new[] { "El código ya existe." } }
            //        }
            //    });

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
                FechaCreacion = DateTime.UtcNow,
                IdUsuarioCreacion = 1,
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

        //// Actualiza producto existente por código
        //[HttpPut("actualizar_producto/{codigo}")]
        //public async Task<IActionResult> ActualizarProducto(string codigo, [FromBody] ProductoDto productoDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var producto = await _context.Productos
        //        .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo);

        //    if (producto == null)
        //        return NotFound( new { mensaje = "Producto no encontrado" });

        //    // Actualiza propiedades
        //    producto.Nombre = productoDto.Nombre;
        //    producto.Descripcion = productoDto.Descripcion;
        //    producto.PrecioCompra = productoDto.PrecioCompra;
        //    producto.PrecioVenta = productoDto.PrecioVenta;
        //    producto.Stock = productoDto.Stock;
        //    producto.StockMinimo = productoDto.StockMinimo;
        //    producto.Categoria = productoDto.Categoria;
        //    producto.Proveedor = productoDto.Proveedor;
        //    producto.FechaModificacion = DateTime.UtcNow;
        //    producto.IdUsuarioModificacion = 1;  // Usuario logueado

        //    await _context.SaveChangesAsync();
        //    return NoContent();  // 204 Success
        //}
        [HttpPut("actualizar_producto/{codigo}")]
        public async Task<IActionResult> ActualizarProducto(string codigo, [FromBody] ProductoUpsertDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var producto = await _context.Productos
                .Include(p => p.ProductoProveedores)
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Costo = dto.Costo;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.StockMinimo = dto.StockMinimo;
            producto.IdCategoria = dto.IdCategoria;
            producto.IdMarca = dto.IdMarca;
            producto.IdUnidadMedida = dto.IdUnidadMedida;
            producto.FechaModificacion = DateTime.UtcNow;
            producto.IdUsuarioModificacion = 1;

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
            return NoContent();
        }

        // Soft delete - marca como inactivo
        [HttpDelete("eliminar_producto/{codigo}")]
        public async Task<IActionResult> EliminarProducto(string codigo)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo);

            if (producto == null)
                return NotFound("Producto no encontrado");

            // Soft delete: solo marca como inactivo
            producto.Activo = false;
            producto.FechaEliminacion = DateTime.UtcNow;
            producto.IdUsuarioEliminacion = 1;  // Usuario logueado

            await _context.SaveChangesAsync();
            return NoContent();  // 204 Success
        }


    }
}
