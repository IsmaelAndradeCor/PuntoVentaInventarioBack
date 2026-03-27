using Microsoft.EntityFrameworkCore;
using PuntoVentaInventario.Models;  // Tu clase Producto

namespace PuntoVentaInventario.Data  // ← Namespace de Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }  // ← Constructor para DI

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .HasQueryFilter(p => p.Activo && p.FechaEliminacion == null);

            // Config DB
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();
        }
    }
}
