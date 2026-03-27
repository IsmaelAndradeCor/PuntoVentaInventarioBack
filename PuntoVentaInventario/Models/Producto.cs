namespace PuntoVentaInventario.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Producto
    {
        [Key]  // ← Agrega PK explícita
        public int Id { get; set; }  // ← Falta ID primary key!

        [Required, MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }  // ← Nullable como interface Angular

        [Column(TypeName = "decimal(18,2)")]  // ← Precisión DB
        public decimal PrecioCompra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        public int Stock { get; set; }
        public int StockMinimo { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }

        [MaxLength(100)]
        public string? Proveedor { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }  // ← Nullable
        public DateTime? FechaEliminacion { get; set; }   // ← Nullable
        public int IdUsuarioCreacion { get; set; }
        public int? IdUsuarioModificacion { get; set; }  // ← Nullable
        public int? IdUsuarioEliminacion { get; set; }  // ← Nullable
        public bool Activo { get; set; } = true;
    }
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public string? Categoria { get; set; }
        public string? Proveedor { get; set; }
    }

}
