CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerCorteCajaHoy]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @Manana DATE = DATEADD(DAY, 1, @Hoy);

    DECLARE @MontoCaja DECIMAL(18,2) = 0;
    DECLARE @MontoVentas DECIMAL(18,2) = 0;
    DECLARE @MontoPagoProveedores DECIMAL(18,2) = 0;

    SELECT
        @MontoCaja = ISNULL(SUM(MontoInicial), 0)
    FROM dbo.AperturasCaja
    WHERE FechaOperacion = @Hoy
      AND Activo = 1;

    SELECT
        @MontoVentas = ISNULL(SUM(Total), 0)
    FROM dbo.Ventas
    WHERE FechaVenta >= @Hoy
      AND FechaVenta < @Manana
      AND FormaPago = 'Efectivo';

    SELECT
        @MontoPagoProveedores = ISNULL(SUM(Monto), 0)
    FROM dbo.PagosProveedores
    WHERE FechaPago >= @Hoy
      AND FechaPago < @Manana
      AND MetodoPago = 'Caja'
      AND Activo = 1;

    SELECT
        @Hoy AS FechaOperacion,
        @MontoCaja AS MontoInicialCaja,
        @MontoVentas AS MontoVentas,
        @MontoPagoProveedores AS MontoPagoProveedores,
        (@MontoCaja + @MontoVentas - @MontoPagoProveedores) AS CorteCaja;
END