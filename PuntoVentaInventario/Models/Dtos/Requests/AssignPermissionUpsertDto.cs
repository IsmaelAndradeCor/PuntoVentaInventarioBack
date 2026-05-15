namespace PuntoVentaInventario.Models.Dtos.Requests
{
    public class AssignPermissionUpsertDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
    }
}
