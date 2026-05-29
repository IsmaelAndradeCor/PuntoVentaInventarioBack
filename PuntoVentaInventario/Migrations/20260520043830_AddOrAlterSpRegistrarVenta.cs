using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace PuntoVentaInventario.Migrations
{

    public partial class AddOrAlterSpRegistrarVenta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarVenta]
    @IdUsuario NVARCHAR(450),
    @Detalle NVARCHAR(MAX),
    @MetodoPago NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF ISJSON(@Detalle) <> 1
            THROW 50001, 'El detalle enviado no es un JSON válido.', 1;

        DECLARE @DetalleTmp TABLE
        (
            IdProducto INT NOT NULL,
            Cantidad DECIMAL(18,2) NOT NULL
        );

        INSERT INTO @DetalleTmp (IdProducto, Cantidad)
        SELECT
            j.IdProducto,
            SUM(j.Cantidad) AS Cantidad
        FROM OPENJSON(@Detalle)
        WITH
        (
            IdProducto INT '$.idProducto',
            Cantidad DECIMAL(18,2) '$.cantidad'
        ) j
        GROUP BY j.IdProducto;

        IF NOT EXISTS (SELECT 1 FROM @DetalleTmp)
            THROW 50002, 'El detalle de la venta está vacío.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp
            WHERE IdProducto IS NULL OR Cantidad IS NULL OR Cantidad <= 0
        )
            THROW 50003, 'Todos los productos y cantidades deben ser válidos.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Folios WITH (UPDLOCK, HOLDLOCK)
            WHERE Tipo = 'VENTA'
        )
            THROW 50004, 'No existe configuración de folios para VENTA.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            LEFT JOIN Productos p ON p.Id = d.IdProducto
            WHERE p.Id IS NULL
        )
            THROW 50005, 'Uno o más productos no existen.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.Activo = 0
        )
            THROW 50006, 'Uno o más productos están inactivos.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.FechaEliminacion IS NOT NULL
        )
            THROW 50007, 'Uno o más productos ya no están disponibles.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.Stock < d.Cantidad
        )
            THROW 50008, 'Stock insuficiente para uno o más productos.', 1;

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
        WHERE Tipo = 'VENTA';

        SELECT TOP 1 @SiguienteNumero = Numero
        FROM @NumeroGenerado;

        IF @SiguienteNumero IS NULL
            THROW 50009, 'No se pudo generar el consecutivo del folio.', 1;

        SET @Folio =
            'VTA-' +
            CONVERT(CHAR(8), @Hoy, 112) +
            '-' +
            RIGHT('000000' + CAST(@SiguienteNumero AS VARCHAR(6)), 6);

        DECLARE @DetalleVenta TABLE
        (
            IdProducto INT NOT NULL,
            CodigoProducto NVARCHAR(50) NOT NULL,
            NombreProducto NVARCHAR(100) NOT NULL,
            Cantidad DECIMAL(18,2) NOT NULL,
            CostoUnitario DECIMAL(18,2) NOT NULL,
            CostoTotal DECIMAL(18,2) NOT NULL,
            PrecioUnitario DECIMAL(18,2) NOT NULL,
            PrecioTotal DECIMAL(18,2) NOT NULL
        );

        INSERT INTO @DetalleVenta
        (
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal,
            PrecioUnitario,
            PrecioTotal
        )
        SELECT
            p.Id,
            p.Codigo,
            p.Nombre,
            d.Cantidad,
            p.Costo,
            d.Cantidad * p.Costo,
            p.Precio,
            d.Cantidad * p.Precio
        FROM @DetalleTmp d
        INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto;

        DECLARE @Total DECIMAL(10,2);

        SELECT @Total = SUM(PrecioTotal)
        FROM @DetalleVenta;

        IF @Total IS NULL OR @Total <= 0
            THROW 50010, 'No se pudo calcular el total de la venta.', 1;

        INSERT INTO Ventas
        (
            Folio,
            FechaVenta,
            Subtotal,
            Descuento,
            Total,
            IdUsuario,
            FormaPago
        )
        VALUES
        (
            @Folio,
            GETDATE(),
            @Total,
            0,
            @Total,
            @IdUsuario,
            @MetodoPago
        );

        DECLARE @IdVenta INT = SCOPE_IDENTITY();

        INSERT INTO DetalleVentas
        (
            IdVenta,
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal,
            PrecioUnitario,
            PrecioTotal
        )
        SELECT
            @IdVenta,
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal,
            PrecioUnitario,
            PrecioTotal
        FROM @DetalleVenta;

        UPDATE p
        SET p.Stock = p.Stock - d.Cantidad
        FROM Productos p
        INNER JOIN @DetalleTmp d ON p.Id = d.IdProducto;

        COMMIT TRANSACTION;

        SELECT
            @IdVenta AS IdVenta,
            @Folio AS Folio,
            @Total AS Total;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
IF OBJECT_ID('dbo.sp_RegistrarVenta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarVenta
""");
        }
    }
}