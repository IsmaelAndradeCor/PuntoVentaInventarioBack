using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class CorteCajaTurnos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AperturasCaja_FechaOperacion",
                table: "AperturasCaja");

            migrationBuilder.CreateTable(
                name: "CortesCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAperturaCaja = table.Column<int>(type: "int", nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoVentasEfectivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPagoProveedores = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoEsperado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Retiro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CortesCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CortesCaja_AperturasCaja_IdAperturaCaja",
                        column: x => x.IdAperturaCaja,
                        principalTable: "AperturasCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AperturasCaja_FechaOperacion_Activo",
                table: "AperturasCaja",
                columns: new[] { "FechaOperacion", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_CortesCaja_IdAperturaCaja",
                table: "CortesCaja",
                column: "IdAperturaCaja");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CortesCaja");

            migrationBuilder.DropIndex(
                name: "IX_AperturasCaja_FechaOperacion_Activo",
                table: "AperturasCaja");

            migrationBuilder.CreateIndex(
                name: "IX_AperturasCaja_FechaOperacion",
                table: "AperturasCaja",
                column: "FechaOperacion",
                unique: true);
        }
    }
}
