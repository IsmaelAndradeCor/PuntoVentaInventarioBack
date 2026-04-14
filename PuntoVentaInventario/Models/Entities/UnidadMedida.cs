using System.ComponentModel.DataAnnotations;

namespace PuntoVentaInventario.Models.Entities
{
    public class UnidadMedida
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Clave { get; set; } = string.Empty;

        public bool PermiteDecimales { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}