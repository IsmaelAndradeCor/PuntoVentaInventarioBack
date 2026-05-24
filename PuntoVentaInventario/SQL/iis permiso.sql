--CREATE LOGIN [IIS APPPOOL\pruebapvi] FROM WINDOWS;

--CREATE USER [IIS APPPOOL\pruebapvi] FOR LOGIN [IIS APPPOOL\pruebapvi];

--ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\pruebapvi];
--ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\pruebapvi];

USE [PuntoVentaInventario];
GO

ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\pruebapvi];
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\pruebapvi];
GO

GRANT EXECUTE TO [IIS APPPOOL\pruebapvi];
GO

ng serve --proxy-config src/proxy.conf.json

dotnet publish -c Release -o C:\Deploy\PVI
ng build --configuration=production
