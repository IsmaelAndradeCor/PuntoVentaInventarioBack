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
        //[HttpGet("db")]
        //public IActionResult TestDb()
        //{
        //    try
        //    {
        //        var connString = _config.GetConnectionString("DefaultConnection");
        //        using var connection = new SqlConnection(connString);
        //        connection.Open();
        //        return Ok($"¡Conexión exitosa! Servidor: {connection.ServerVersion}");
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpGet("productos")]  // Lista todos
        //public IActionResult GetProductos()
        //{
        //    try
        //    {
        //        var connString = _config.GetConnectionString("DefaultConnection");
        //        using var conn = new SqlConnection(connString);
        //        using var cmd = new SqlCommand("SP_ProductosListar", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        conn.Open();
        //        using var reader = cmd.ExecuteReader();
        //        var productos = new List<object>();
        //        while (reader.Read())
        //        {
        //            var producto = new
        //            {
        //                Id = reader["Id"],
        //                Codigo = reader["Codigo"],
        //                Nombre = reader["Nombre"],
        //                Descripcion = reader["Descripcion"],
        //                Precio = reader["Precio"],
        //                Stock = reader["Stock"],
        //                Categoria = reader["Categoria"]
        //            };
        //            productos.Add(producto);
        //        }
        //        return Ok(productos);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpGet("productos")]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new ProductoDto  // ← Mapeo directo EF → DTO
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    Categoria = p.Categoria,
                    Proveedor = p.Proveedor
                })
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        {
            var producto = await _context.Productos
                .Where(p => p.Codigo == codigo && p.Activo)
                .Select(p => new ProductoDto  // ← Mapeo directo
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    Categoria = p.Categoria,
                    Proveedor = p.Proveedor
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return NotFound("Producto no encontrado");

            return Ok(producto);
        }

        [HttpPost("producto")]
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


        [HttpPut("producto/{codigo}")]
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


        //[HttpGet("producto/{id}")]  // Uno por Id
        //public IActionResult GetProducto(int id)
        //{
        //    try
        //    {
        //        var connString = _config.GetConnectionString("DefaultConnection");
        //        using var conn = new SqlConnection(connString);
        //        using var cmd = new SqlCommand("SP_ProductosObtener", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        conn.Open();
        //        using var reader = cmd.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            var producto = new
        //            {
        //                Id = reader["Id"],
        //                Codigo = reader["Codigo"],
        //                Nombre = reader["Nombre"],
        //                Descripcion = reader["Descripcion"] ?? "",
        //                Precio = reader["Precio"],
        //                Stock = reader["Stock"],
        //                Categoria = reader["Categoria"] ?? ""
        //            };
        //            return Ok(producto);
        //        }
        //        return NotFound("Producto no encontrado");
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpPost("producto")]  // Insertar
        //public IActionResult InsertarProducto([FromBody] ProductoDto producto)
        //{
        //    try
        //    {
        //        var connString = _config.GetConnectionString("DefaultConnection");
        //        using var conn = new SqlConnection(connString);
        //        using var cmd = new SqlCommand("SP_ProductosInsertar", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        cmd.Parameters.AddWithValue("@Codigo", producto.Codigo);
        //        cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
        //        cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@Precio", producto.Precio);
        //        cmd.Parameters.AddWithValue("@Stock", producto.Stock);
        //        cmd.Parameters.AddWithValue("@Categoria", (object)producto.Categoria ?? DBNull.Value);

        //        conn.Open();
        //        var nuevoId = cmd.ExecuteScalar();
        //        return Ok(new { Id = nuevoId, Mensaje = "Producto insertado" });
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
    }
}
