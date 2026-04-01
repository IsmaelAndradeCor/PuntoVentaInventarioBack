using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PuntoVentaInventario.Data;
using PuntoVentaInventario.Models;
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

        // Obtiene todos los productos activos
        [HttpGet("listar_productos")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await _context.ProductosDto
                    .FromSqlRaw("EXEC SP_ProductosListar")
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
            }
            //var productos = await _context.Productos
            //    .Where(p => p.Activo)
            //    .Select(p => new ProductoDto  // ← Mapeo directo EF → DTO
            //    {
            //        Id = p.Id,
            //        Codigo = p.Codigo,
            //        Nombre = p.Nombre,
            //        Descripcion = p.Descripcion,
            //        PrecioCompra = p.PrecioCompra,
            //        PrecioVenta = p.PrecioVenta,
            //        Stock = p.Stock,
            //        StockMinimo = p.StockMinimo,
            //        Categoria = p.Categoria,
            //        Proveedor = p.Proveedor
            //    })
            //    .ToListAsync();
            //return Ok(productos);
        }

        /// <summary>
        /// Funcion para obtener un producto por su código utilizando el procedimiento almacenado SP_ProductosObtener
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns>productoDto</returns>
        [HttpGet("producto_codigo/{codigo}")]
        public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        {
            try
            {
                var producto = (await _context.ProductosDto
                    .FromSqlInterpolated($"EXEC sp_ProductosObtener @Codigo={codigo}")
                    .AsNoTracking()
                    .ToListAsync())
                    .FirstOrDefault();

                if (producto == null)
                    return NotFound("Producto no encontrado");

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener producto: {ex.Message}");
            }
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

        // Inserta nuevo producto
        [HttpPost("crear_producto")]
        public async Task<IActionResult> InsertarProducto([FromBody] ProductoDto productoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar código único
            if (await _context.Productos.AnyAsync(p => p.Codigo == productoDto.Codigo))
                return BadRequest("Código ya existe");

            var producto = new Producto
            {
                Codigo = productoDto.Codigo,
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                PrecioCompra = productoDto.PrecioCompra,
                PrecioVenta = productoDto.PrecioVenta,
                Stock = productoDto.Stock,
                StockMinimo = productoDto.StockMinimo,
                Categoria = productoDto.Categoria,
                Proveedor = productoDto.Proveedor,
                FechaCreacion = DateTime.UtcNow,
                IdUsuarioCreacion = 1,
                Activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // ← CAMBIO: Retorna DTO (no Entity)
            var dto = new ProductoDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                // ... resto propiedades
            };
            return CreatedAtAction(nameof(GetProductoPorCodigo), new { codigo = producto.Codigo }, dto);
        }

        // Actualiza producto existente por código
        [HttpPut("actualizar_producto/{codigo}")]
        public async Task<IActionResult> ActualizarProducto(string codigo, [FromBody] ProductoDto productoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo);

            if (producto == null)
                return NotFound("Producto no encontrado");

            // Actualiza propiedades
            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.PrecioCompra = productoDto.PrecioCompra;
            producto.PrecioVenta = productoDto.PrecioVenta;
            producto.Stock = productoDto.Stock;
            producto.StockMinimo = productoDto.StockMinimo;
            producto.Categoria = productoDto.Categoria;
            producto.Proveedor = productoDto.Proveedor;
            producto.FechaModificacion = DateTime.UtcNow;
            producto.IdUsuarioModificacion = 1;  // Usuario logueado

            await _context.SaveChangesAsync();
            return NoContent();  // 204 Success
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
