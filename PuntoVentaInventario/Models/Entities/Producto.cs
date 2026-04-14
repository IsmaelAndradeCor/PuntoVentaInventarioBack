using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace PuntoVentaInventario.Models.Entities
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Stock { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal StockMinimo { get; set; }

        public int? IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        public int? IdMarca { get; set; }
        public Marca? Marca { get; set; }

        public int? IdUnidadMedida { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public int? IdUsuarioModificacion { get; set; }
        public int? IdUsuarioEliminacion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<ProductoProveedor> ProductoProveedores { get; set; } = new List<ProductoProveedor>();
    }
}