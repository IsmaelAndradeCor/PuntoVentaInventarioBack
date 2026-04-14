using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Contacto { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<ProductoProveedor> ProductoProveedores { get; set; } = new List<ProductoProveedor>();
    }
}