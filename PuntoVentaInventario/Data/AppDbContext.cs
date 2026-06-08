using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Models;
using PuntoVentaInventario.Models.Entities;

namespace PuntoVentaInventario.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<PagoProveedor> PagosProveedores { get; set; }
        public DbSet<ProductoProveedor> ProductoProveedores { get; set; }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<AperturaCaja> AperturasCaja { get; set; }
        public DbSet<CorteCaja> CortesCaja { get; set; }
        public DbSet<Merma> Mermas { get; set; }
        public DbSet<DetalleMerma> DetalleMermas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Producto>()
            //    .HasQueryFilter(p => p.Activo && p.FechaEliminacion == null);

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.Folio)
                .IsUnique();

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.MetodoPago)
                .WithMany()
                .HasForeignKey(v => v.IdMetodoPago)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Folio>(entity =>
            {
                entity.HasKey(f => f.Tipo);

                entity.Property(f => f.Tipo)
                    .HasMaxLength(20);

                entity.Property(f => f.Fecha)
                    .HasColumnType("date");

                entity.HasData(
                    new Folio
                    {
                        Tipo = "VENTA",
                        Fecha = new DateTime(2026, 1, 1),
                        UltimoNumero = 0
                    },
                    new Folio
                    {
                        Tipo = "PAGO",
                        Fecha = new DateTime(2026, 1, 1),
                        UltimoNumero = 0
                    },
                    new Folio
                    {
                        Tipo = "MERMA",
                        Fecha = new DateTime(2026, 1, 1),
                        UltimoNumero = 0
                    }
                );
            });

            modelBuilder.Entity<PagoProveedor>(entity =>
            {
                entity.ToTable("PagosProveedores");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Folio)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.MetodoPago)
                    .WithMany()
                    .HasForeignKey(e => e.IdMetodoPago)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Referencia)
                    .HasMaxLength(100);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.HasOne(e => e.Proveedor)
                    .WithMany(p => p.PagosProveedores)
                    .HasForeignKey(e => e.IdProveedor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MetodoPago>(entity =>
            {
                entity.ToTable("MetodosPago");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(e => e.Nombre)
                    .IsUnique();

                entity.HasData(
                    new MetodoPago { Id = 1, Nombre = "Efectivo", Activo = true, AfectaCaja = true },
                    new MetodoPago { Id = 2, Nombre = "Transferencia", Activo = true, AfectaCaja = false },
                    new MetodoPago { Id = 3, Nombre = "Terminal", Activo = true, AfectaCaja = false },
                    new MetodoPago { Id = 4, Nombre = "Caja", Activo = true, AfectaCaja = true }
                );
            });

            modelBuilder.Entity<AperturaCaja>(entity =>
            {
                entity.ToTable("AperturasCaja");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaOperacion)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.MontoInicial)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime2")
                    .IsRequired();

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.HasIndex(e => new { e.FechaOperacion, e.Activo });

                entity.HasMany<CorteCaja>()
                    .WithOne(c => c.AperturaCaja)
                    .HasForeignKey(c => c.IdAperturaCaja)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CorteCaja>(entity =>
            {
                entity.ToTable("CortesCaja");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaCorte)
                    .HasColumnType("datetime2")
                    .IsRequired();

                entity.Property(e => e.MontoInicial)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.MontoVentasEfectivo)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.MontoPagoProveedores)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.MontoEsperado)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Retiro)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.MontoFinal)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.IdUsuarioPrevio)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.Property(e => e.NombreUsuarioPrevio)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.IdUsuarioCorte)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.Property(e => e.NombreUsuarioCorte)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.IdUsuarioRecepcion)
                    .HasMaxLength(450);

                entity.Property(e => e.NombreUsuarioRecepcion)
                    .HasMaxLength(200);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Merma>(entity =>
            {
                entity.ToTable("Mermas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Folio)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.FechaMerma)
                    .HasColumnType("datetime2")
                    .IsRequired();

                entity.Property(e => e.CostoTotal)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(500);

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.HasIndex(e => e.Folio)
                    .IsUnique();
            });

            modelBuilder.Entity<DetalleMerma>(entity =>
            {
                entity.ToTable("DetalleMermas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.CodigoProducto)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.NombreProducto)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Cantidad)
                    .HasColumnType("decimal(18,3)")
                    .IsRequired();

                entity.Property(e => e.CostoUnitario)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.CostoTotal)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasOne(e => e.Merma)
                    .WithMany(m => m.Detalles)
                    .HasForeignKey(e => e.IdMerma)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}