using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Models;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoDto> ProductosDto { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<GenerarVentasDto> GenerarVentasDto { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProductoProveedor> ProductoProveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .HasQueryFilter(p => p.Activo && p.FechaEliminacion == null);

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.Folio)
                .IsUnique();

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.Property(e => e.Id).HasColumnOrder(0);
                entity.Property(e => e.IdVenta).HasColumnOrder(1);
                entity.Property(e => e.IdProducto).HasColumnOrder(2);
                entity.Property(e => e.CodigoProducto).HasColumnOrder(3);
                entity.Property(e => e.NombreProducto).HasColumnOrder(4);
                entity.Property(e => e.Cantidad).HasColumnOrder(5);
                entity.Property(e => e.CostoUnitario).HasColumnOrder(6);
                entity.Property(e => e.CostoTotal).HasColumnOrder(7);
                entity.Property(e => e.PrecioUnitario).HasColumnOrder(8);
                entity.Property(e => e.PrecioTotal).HasColumnOrder(9);
            });

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Marca>()
                .HasIndex(m => m.Nombre)
                .IsUnique();

            modelBuilder.Entity<UnidadMedida>()
                .HasIndex(u => u.Clave)
                .IsUnique();

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.Nombre);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Marca)
                .WithMany(m => m.Productos)
                .HasForeignKey(p => p.IdMarca)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.UnidadMedida)
                .WithMany(u => u.Productos)
                .HasForeignKey(p => p.IdUnidadMedida)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductoProveedor>()
                .HasKey(pp => new { pp.IdProducto, pp.IdProveedor });

            modelBuilder.Entity<ProductoProveedor>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProductoProveedores)
                .HasForeignKey(pp => pp.IdProducto);

            modelBuilder.Entity<ProductoProveedor>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(p => p.ProductoProveedores)
                .HasForeignKey(pp => pp.IdProveedor);

            modelBuilder.Entity<ProductoDto>().HasNoKey();
            modelBuilder.Entity<GenerarVentasDto>().HasNoKey();
        }
    }
}