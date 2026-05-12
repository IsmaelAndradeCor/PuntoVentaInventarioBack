using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class Marca
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}