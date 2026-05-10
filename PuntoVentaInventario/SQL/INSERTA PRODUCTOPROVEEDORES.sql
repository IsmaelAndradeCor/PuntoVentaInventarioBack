INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (1,4,11)
WHERE p.Codigo IN ('P0001','P0002','P0003','P0096');

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (2,4,13)
WHERE p.Codigo IN ('P0004','P0005','P0097');

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (4,13)
WHERE p.Codigo IN ('P0006','P0007','P0008','P0009');

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (5,13,14)
WHERE p.Codigo IN ('P0010','P0011','P0012','P0013','P0098','P0099');

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (11,14,17)
WHERE p.Codigo IN ('P0014','P0015','P0100');

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (3,6,7)
WHERE p.Codigo BETWEEN 'P0016' AND 'P0025';

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (2,6,8)
WHERE p.Codigo BETWEEN 'P0026' AND 'P0036';

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (4,9,10)
WHERE p.Codigo BETWEEN 'P0037' AND 'P0046';

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (5,11,13)
WHERE p.Codigo BETWEEN 'P0047' AND 'P0066';

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (1,2,15)
WHERE p.Codigo BETWEEN 'P0067' AND 'P0080';

INSERT INTO ProductoProveedores (IdProducto, IdProveedor)
SELECT p.Id, v.Id
FROM Productos p
JOIN Proveedores v ON v.Id IN (6,7,14)
WHERE p.Codigo BETWEEN 'P0081' AND 'P0095';