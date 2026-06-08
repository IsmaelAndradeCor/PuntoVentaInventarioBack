using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVentaInventario.Models.Entities
{
    public class DetalleMerma
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Merma))]
        public int IdMerma { get; set; }

        [ForeignKey(nameof(Producto))]
        public int IdProducto { get; set; }

        [Required, MaxLength(20)]
        public string CodigoProducto { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NombreProducto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,3)")]
        public decimal Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoTotal { get; set; }

        public Merma Merma { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
