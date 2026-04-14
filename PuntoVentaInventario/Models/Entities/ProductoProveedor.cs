namespace PuntoVentaInventario.Models.Entities
{
    public class ProductoProveedor
    {
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } = null!;

        public int IdProveedor { get; set; }
        public Proveedor Proveedor { get; set; } = null!;
    }
}