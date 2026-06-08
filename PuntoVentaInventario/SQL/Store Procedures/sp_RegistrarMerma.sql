CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarMerma]
    @IdUsuario NVARCHAR(450),
    @Detalle NVARCHAR(MAX),
    @Observaciones NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF ISJSON(@Detalle) <> 1
            THROW 50301, 'El detalle enviado no es un JSON válido.', 1;

        DECLARE @DetalleTmp TABLE
        (
            IdProducto INT NOT NULL,
            Cantidad DECIMAL(18,3) NOT NULL
        );

        INSERT INTO @DetalleTmp (IdProducto, Cantidad)
        SELECT
            j.IdProducto,
            SUM(j.Cantidad) AS Cantidad
        FROM OPENJSON(@Detalle)
        WITH
        (
            IdProducto INT '$.idProducto',
            Cantidad DECIMAL(18,3) '$.cantidad'
        ) j
        GROUP BY j.IdProducto;

        IF NOT EXISTS (SELECT 1 FROM @DetalleTmp)
            THROW 50302, 'El detalle de la merma está vacío.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp
            WHERE IdProducto IS NULL OR Cantidad IS NULL OR Cantidad <= 0
        )
            THROW 50303, 'Todos los productos y cantidades deben ser válidos.', 1;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Folios WITH (UPDLOCK, HOLDLOCK)
            WHERE Tipo = 'MERMA'
        )
            THROW 50304, 'No existe configuración de folios para MERMA.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            LEFT JOIN Productos p ON p.Id = d.IdProducto
            WHERE p.Id IS NULL
        )
            THROW 50305, 'Uno o más productos no existen.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.Activo = 0
        )
            THROW 50306, 'Uno o más productos están inactivos.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.FechaEliminacion IS NOT NULL
        )
            THROW 50307, 'Uno o más productos ya no están disponibles.', 1;

        IF EXISTS (
            SELECT 1
            FROM @DetalleTmp d
            INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto
            WHERE p.Stock < d.Cantidad
        )
            THROW 50308, 'Stock insuficiente para uno o más productos.', 1;

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
        WHERE Tipo = 'MERMA';

        SELECT TOP 1 @SiguienteNumero = Numero
        FROM @NumeroGenerado;

        IF @SiguienteNumero IS NULL
            THROW 50309, 'No se pudo generar el consecutivo del folio.', 1;

        SET @Folio =
            'MER-' +
            CONVERT(CHAR(8), @Hoy, 112) +
            '-' +
            RIGHT('000000' + CAST(@SiguienteNumero AS VARCHAR(6)), 6);

        DECLARE @DetalleMerma TABLE
        (
            IdProducto INT NOT NULL,
            CodigoProducto NVARCHAR(50) NOT NULL,
            NombreProducto NVARCHAR(100) NOT NULL,
            Cantidad DECIMAL(18,3) NOT NULL,
            CostoUnitario DECIMAL(18,2) NOT NULL,
            CostoTotal DECIMAL(18,2) NOT NULL
        );

        INSERT INTO @DetalleMerma
        (
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal
        )
        SELECT
            p.Id,
            p.Codigo,
            p.Nombre,
            d.Cantidad,
            p.Costo,
            d.Cantidad * p.Costo
        FROM @DetalleTmp d
        INNER JOIN Productos p WITH (UPDLOCK, HOLDLOCK) ON p.Id = d.IdProducto;

        DECLARE @CostoTotal DECIMAL(18,2);

        SELECT @CostoTotal = SUM(CostoTotal)
        FROM @DetalleMerma;

        IF @CostoTotal IS NULL OR @CostoTotal <= 0
            THROW 50310, 'No se pudo calcular el costo total de la merma.', 1;

        INSERT INTO Mermas
        (
            Folio,
            FechaMerma,
            CostoTotal,
            Observaciones,
            IdUsuario
        )
        VALUES
        (
            @Folio,
            GETDATE(),
            @CostoTotal,
            NULLIF(LTRIM(RTRIM(@Observaciones)), ''),
            @IdUsuario
        );

        DECLARE @IdMerma INT = SCOPE_IDENTITY();

        INSERT INTO DetalleMermas
        (
            IdMerma,
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal
        )
        SELECT
            @IdMerma,
            IdProducto,
            CodigoProducto,
            NombreProducto,
            Cantidad,
            CostoUnitario,
            CostoTotal
        FROM @DetalleMerma;

        UPDATE p
        SET p.Stock = p.Stock - d.Cantidad
        FROM Productos p
        INNER JOIN @DetalleTmp d ON p.Id = d.IdProducto;

        COMMIT TRANSACTION;

        SELECT
            @IdMerma AS IdMerma,
            @Folio AS Folio,
            @CostoTotal AS CostoTotal;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
