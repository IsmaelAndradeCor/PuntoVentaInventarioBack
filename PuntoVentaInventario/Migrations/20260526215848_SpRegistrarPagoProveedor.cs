using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVentaInventario.Migrations
{
    /// <inheritdoc />
    public partial class SpRegistrarPagoProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenerarVentasDto");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Productos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "PagosProveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosProveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosProveedores_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_IdProveedor",
                table: "PagosProveedores",
                column: "IdProveedor");

            migrationBuilder.Sql(
                @"
create or ALTER PROCEDURE [dbo].[sp_RegistrarPagoProveedor]
    @IdProveedor INT,
    @Monto DECIMAL(18,2),
    @MetodoPago NVARCHAR(50),
    @Referencia NVARCHAR(100) = NULL,
    @Observaciones NVARCHAR(500) = NULL,
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @IdProveedor IS NULL OR @IdProveedor <= 0
            THROW 50101, 'El proveedor enviado no es válido.', 1;

        IF @Monto IS NULL OR @Monto <= 0
            THROW 50102, 'El monto debe ser mayor a 0.', 1;

        IF @MetodoPago IS NULL OR LTRIM(RTRIM(@MetodoPago)) = ''
            THROW 50103, 'El método de pago es obligatorio.', 1;

        IF @IdUsuario IS NULL OR LTRIM(RTRIM(@IdUsuario)) = ''
            THROW 50104, 'El usuario es obligatorio.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Folios WITH (UPDLOCK, HOLDLOCK)
            WHERE Tipo = 'PAGO'
        )
            THROW 50105, 'No existe configuración de folios para PAGO.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Proveedores WITH (UPDLOCK, HOLDLOCK)
            WHERE Id = @IdProveedor AND Activo = 1
        )
            THROW 50106, 'El proveedor no existe o está inactivo.', 1;

        DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
        DECLARE @NumeroGenerado TABLE (Numero INT);
        DECLARE @SiguienteNumero INT;
        DECLARE @Folio NVARCHAR(30);

        UPDATE dbo.Folios
        SET
            UltimoNumero =
                CASE
                    WHEN Fecha = @Hoy THEN UltimoNumero + 1
                    ELSE 1
                END,
            Fecha = @Hoy
        OUTPUT INSERTED.UltimoNumero INTO @NumeroGenerado(Numero)
        WHERE Tipo = 'PAGO';

        SELECT TOP 1 @SiguienteNumero = Numero
        FROM @NumeroGenerado;

        IF @SiguienteNumero IS NULL
            THROW 50107, 'No se pudo generar el consecutivo del folio.', 1;

        SET @Folio =
            'PAG-' +
            CONVERT(CHAR(8), @Hoy, 112) +
            '-' +
            RIGHT('000000' + CAST(@SiguienteNumero AS VARCHAR(6)), 6);

        INSERT INTO dbo.PagosProveedores
        (
            Folio,
            IdProveedor,
            Monto,
            MetodoPago,
            Referencia,
            Observaciones,
            FechaPago,
            IdUsuario,
            Activo
        )
        VALUES
        (
            @Folio,
            @IdProveedor,
            @Monto,
            LTRIM(RTRIM(@MetodoPago)),
            NULLIF(LTRIM(RTRIM(@Referencia)), ''),
            NULLIF(LTRIM(RTRIM(@Observaciones)), ''),
            GETDATE(),
            @IdUsuario,
            1
        );

        DECLARE @IdPagoProveedor INT = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT
            @IdPagoProveedor AS IdPagoProveedor,
            @Folio AS Folio,
            @Monto AS Monto;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
            DROP PROCEDURE IF EXISTS sp_RegistrarPagoProveedor;
            ");

            migrationBuilder.DropTable(
                name: "PagosProveedores");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Productos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "GenerarVentasDto",
                columns: table => new
                {
                    CostoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ganancias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
