using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class CorteCajaTurnos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "CortesCaja",
                newName: "IdUsuarioRecepcion");

            migrationBuilder.AddColumn<string>(
                name: "IdUsuarioCorte",
                table: "CortesCaja",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreUsuarioCorte",
                table: "CortesCaja",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreUsuarioRecepcion",
                table: "CortesCaja",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsuarioCorte",
                table: "CortesCaja");

            migrationBuilder.DropColumn(
                name: "NombreUsuarioCorte",
                table: "CortesCaja");

            migrationBuilder.DropColumn(
                name: "NombreUsuarioRecepcion",
                table: "CortesCaja");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioRecepcion",
                table: "CortesCaja",
                newName: "IdUsuario");
        }
    }
}
