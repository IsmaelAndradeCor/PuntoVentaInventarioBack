namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class RemovePermissionUpsertDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
    }
}
