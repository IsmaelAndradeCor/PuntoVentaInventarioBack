select * from Productos
SELECT * FROM Proveedores
select * from ProductoProveedores
SELECT * FROM MetodosPago
select * from Ventas
SELECT * FROM DetalleVentas

SELECT * FROM AperturasCaja

SELECT * FROM Marcas
SELECT * FROM Categorias
SELECT * FROM UnidadesMedida


--select * from AspNetUsers

--DECLARE @UserId NVARCHAR(450) = '60da141d-6bd0-426c-8229-6623be971dbf';

--INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
--SELECT @UserId, 'permission', P.ClaimValue
--FROM (VALUES
--    ('home.ver'),

--    ('ventas.realizar'),
--    ('ventas.historial.ver'),

--    ('mermas.ver'),
--    ('mermas.registrar'),
--    ('mermas.historial.ver'),

--    ('productos.ver'),
--    ('productos.activos.ver'),
--    ('productos.inactivos.ver'),
--    ('productos.crear'),
--    ('productos.actualizar'),
--    ('productos.activar'),
--    ('productos.desactivar'),

--    ('marcas.ver'),
--    ('marcas.activos.ver'),
--    ('marcas.inactivos.ver'),
--    ('marcas.crear'),
--    ('marcas.actualizar'),
--    ('marcas.activar'),
--    ('marcas.desactivar'),

--    ('categorias.ver'),
--    ('categorias.activos.ver'),
--    ('categorias.inactivos.ver'),
--    ('categorias.crear'),
--    ('categorias.actualizar'),
--    ('categorias.activar'),
--    ('categorias.desactivar'),

--    ('unidadesmedida.ver'),
--    ('unidadesmedida.activos.ver'),
--    ('unidadesmedida.inactivos.ver'),
--    ('unidadesmedida.crear'),
--    ('unidadesmedida.actualizar'),
--    ('unidadesmedida.activar'),
--    ('unidadesmedida.desactivar'),

--    ('proveedores.ver'),
--    ('proveedores.activos.ver'),
--    ('proveedores.inactivos.ver'),
--    ('proveedores.crear'),
--    ('proveedores.actualizar'),
--    ('proveedores.activar'),
--    ('proveedores.desactivar')
--) AS P(ClaimValue)
--WHERE NOT EXISTS (
--    SELECT 1
--    FROM AspNetUserClaims UC
--    WHERE UC.UserId = @UserId
--      AND UC.ClaimType = 'permission'
--      AND UC.ClaimValue = P.ClaimValue
--);


--select * from AspNetUserClaims

--select * from AspNetRoles