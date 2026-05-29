CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarPagoProveedor]
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