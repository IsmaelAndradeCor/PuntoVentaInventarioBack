using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Models;

namespace PuntoVentaInventario.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core para la aplicación.
    /// 
    /// Esta clase representa la sesión con la base de datos y permite:
    /// - Consultar información
    /// - Insertar registros
    /// - Actualizar registros
    /// - Eliminar registros
    /// - Configurar relaciones, índices y reglas del modelo
    /// 
    /// Aquí se definen las tablas principales del sistema:
    /// Productos, Ventas y DetalleVentas.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe la configuración del contexto desde
        /// la inyección de dependencias (DI).
        /// 
        /// EF Core usa estas opciones para saber:
        /// - Qué motor de base de datos se utilizará
        /// - Qué cadena de conexión usar
        /// - Qué configuraciones adicionales aplicar
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Representa la tabla Productos en la base de datos.
        /// 
        /// Permite consultar y manipular los productos registrados
        /// en el sistema.
        /// </summary>
        public DbSet<Producto> Productos { get; set; }

        /// <summary>
        /// Trae el listado de los Productos Activos y los mapea posteriormente
        /// </summary>
        public DbSet<ProductoDto> ProductosDto { get; set; }

        /// <summary>
        /// Representa la tabla Ventas en la base de datos.
        /// 
        /// Contiene la cabecera o información general de cada venta,
        /// como folio, fecha, usuario, totales y forma de pago.
        /// </summary>
        public DbSet<Venta> Ventas { get; set; }

        /// <summary>
        /// Representa la tabla DetalleVentas en la base de datos.
        /// 
        /// Aquí se almacenan los productos que pertenecen a cada venta,
        /// incluyendo cantidad, precio unitario y subtotal por renglón.
        /// </summary>
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        /// <summary>
        /// Método donde se configura el modelo de datos con Fluent API.
        /// 
        /// Este método permite definir:
        /// - Filtros globales
        /// - Índices
        /// - Restricciones
        /// - Relaciones entre tablas
        /// - Reglas de eliminación
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de EF Core</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Filtro global para Producto.
            /// 
            /// Hace que EF Core solo devuelva productos activos
            /// y que no estén marcados como eliminados lógicamente.
            /// 
            /// Esto significa que cuando hagas consultas como:
            /// context.Productos.ToList()
            /// automáticamente se excluirán los productos inactivos
            /// o con FechaEliminacion distinta de null.
            /// </summary>
            modelBuilder.Entity<Producto>()
                .HasQueryFilter(p => p.Activo && p.FechaEliminacion == null);

            /// <summary>
            /// Índice único para el campo Codigo en Producto.
            /// 
            /// Evita que existan dos productos con el mismo código.
            /// Esto ayuda a mantener integridad de datos y mejora
            /// búsquedas por ese campo.
            /// </summary>
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            /// <summary>
            /// Índice único para el campo Folio en Venta.
            /// 
            /// Evita folios duplicados en las ventas.
            /// Cada venta debe tener un identificador único.
            /// </summary>
            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.Folio)
                .IsUnique();

            /// <summary>
            /// Configura la relación entre DetalleVenta y Venta.
            /// 
            /// - Un detalle pertenece a una sola venta
            /// - Una venta puede tener muchos detalles
            /// - La llave foránea es IdVenta
            /// - Si una venta se elimina, sus detalles también se eliminan
            ///   automáticamente (DeleteBehavior.Cascade)
            /// </summary>
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Configura la relación entre DetalleVenta y Producto.
            /// 
            /// - Un detalle usa un solo producto
            /// - Un producto puede aparecer en muchos detalles de venta
            /// - La llave foránea es IdProducto
            /// - No se permite eliminar un producto si está siendo usado
            ///   en detalles de venta (DeleteBehavior.Restrict)
            /// 
            /// Esto protege el historial de ventas.
            /// </summary>
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración del DTO como tipo sin llave
            modelBuilder.Entity<ProductoDto>().HasNoKey();
        }
    }
}