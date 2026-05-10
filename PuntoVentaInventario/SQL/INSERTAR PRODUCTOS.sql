INSERT INTO Productos
(
    Codigo, Nombre, Descripcion, Costo, Precio, Stock, StockMinimo,
    FechaCreacion, FechaModificacion, FechaEliminacion,
    IdUsuarioCreacion, IdUsuarioModificacion, IdUsuarioEliminacion,
    Activo, IdCategoria, IdMarca, IdUnidadMedida
)
VALUES
('P0001','Coca-Cola 600 ml','Refresco Coca-Cola 600 ml',12.50,18.00,48,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,1,5),
('P0002','Coca-Cola 2.5 L','Refresco Coca-Cola 2.5 litros',28.00,39.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,1,5),
('P0003','Coca-Cola lata 355 ml','Refresco Coca-Cola en lata 355 ml',9.00,14.00,60,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,1,6),
('P0004','Pepsi 400 ml','Refresco Pepsi 400 ml',10.00,15.00,36,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,5,5),
('P0005','Pepsi 2 L','Refresco Pepsi 2 litros',22.00,34.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,5,5),
('P0006','7UP 400 ml','Refresco 7UP 400 ml',10.00,15.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,32,5),
('P0007','7UP 2 L','Refresco 7UP 2 litros',22.00,33.00,18,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,32,5),
('P0008','Manzanita Sol 400 ml','Refresco Manzanita Sol 400 ml',10.00,15.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,33,5),
('P0009','Manzanita Sol 2 L','Refresco Manzanita Sol 2 litros',23.00,34.00,18,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,33,5),
('P0010','Jumex Mango 473 ml','Néctar Jumex sabor mango',9.50,14.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,13,6),

('P0011','Jumex Durazno 473 ml','Néctar Jumex sabor durazno',9.50,14.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,13,6),
('P0012','Del Valle Manzana 1 L','Jugo Del Valle manzana 1 litro',18.00,27.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,14,11),
('P0013','Del Valle Naranja 1 L','Jugo Del Valle naranja 1 litro',18.00,27.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,14,11),
('P0014','Gatorade Naranja 1 L','Bebida deportiva Gatorade naranja',20.00,30.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,31,11),
('P0015','Gatorade Mora Azul 1 L','Bebida deportiva Gatorade mora azul',20.00,30.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,31,11),

('P0016','Emperador Chocolate','Galletas Emperador sabor chocolate',12.00,18.00,50,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,38,3),
('P0017','Emperador Vainilla','Galletas Emperador sabor vainilla',12.00,18.00,50,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,38,3),
('P0018','Chokis original','Galletas Chokis original',13.00,20.00,48,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,37,3),
('P0019','Oreo original','Galletas Oreo original',13.50,20.00,48,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,26,3),
('P0020','Ritz original','Galletas Ritz original',13.00,19.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,27,3),

('P0021','Saladitas original','Galletas Saladitas original',11.00,17.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,39,3),
('P0022','Gamesa Marias','Galletas Marías Gamesa',10.00,16.00,60,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,2,3),
('P0023','Gamesa Crackers','Galletas saladas Gamesa crackers',11.00,17.00,45,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,2,3),
('P0024','Marinela Gansito','Pastelito Gansito Marinela',13.50,19.00,36,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,7,1),
('P0025','Marinela Submarinos','Pastelito Submarinos Marinela',12.50,18.00,36,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,7,1),

('P0026','Bimbo Pan Blanco Grande','Pan blanco grande Bimbo',30.00,42.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,6,1),
('P0027','Bimbo Pan Integral','Pan integral Bimbo',31.00,44.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,6,1),
('P0028','Bimbo Roles Canela','Roles de canela Bimbo',25.00,35.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,6,1),
('P0029','Ricolino Bubulubu','Chocolate Bubulubu',8.00,12.00,80,20,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,3,1),
('P0030','Ricolino Paleta Payaso','Paleta Payaso Ricolino',9.00,14.00,70,18,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,3,1),

('P0031','Carlos V barra','Chocolate Carlos V barra',10.00,15.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,24,1),
('P0032','KitKat 4 fingers','Chocolate KitKat 4 fingers',13.00,19.00,55,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,25,1),
('P0033','Halls Menta','Caramelo Halls sabor menta',7.00,11.00,80,20,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,29,1),
('P0034','Halls Cereza','Caramelo Halls sabor cereza',7.00,11.00,80,20,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,29,1),
('P0035','Trident Sandía','Goma de mascar Trident sandía',8.00,12.00,90,20,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,28,1),

('P0036','Trident Hierbabuena','Goma de mascar Trident hierbabuena',8.00,12.00,90,20,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,28,1),
('P0037','Sabritas Original 45 g','Papas Sabritas original 45 g',11.00,17.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,4,4),
('P0038','Sabritas Adobadas 45 g','Papas Sabritas adobadas 45 g',11.00,17.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,4,4),
('P0039','Doritos Nacho 62 g','Botana Doritos Nacho',13.00,20.00,55,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,34,4),
('P0040','Doritos Incógnita 62 g','Botana Doritos Incógnita',13.00,20.00,55,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,34,4),

('P0041','Cheetos Torciditos','Botana Cheetos Torciditos',12.00,18.00,55,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,35,4),
('P0042','Cheetos Flamin Hot','Botana Cheetos Flamin Hot',12.00,18.00,55,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,35,4),
('P0043','Tostitos Salsa Verde','Botana Tostitos salsa verde',14.00,21.00,50,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,36,4),
('P0044','Tostitos Original','Botana Tostitos original',14.00,21.00,50,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,36,4),
('P0045','Barcel Chips Fuego','Botana chips fuego Barcel',12.00,18.00,50,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,8,4),

('P0046','Barcel Takis Fuego','Botana Takis Fuego Barcel',13.00,20.00,65,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,8,4),
('P0047','Maruchan vaso pollo','Sopa instantánea Maruchan sabor pollo',14.00,20.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,40,1),
('P0048','Maruchan vaso camarón','Sopa instantánea Maruchan sabor camarón',14.00,20.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,40,1),
('P0049','Maggi sopa pollo','Sopa instantánea Maggi sabor pollo',11.00,17.00,45,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,23,1),
('P0050','Maggi cubos pollo','Cubos sazonadores Maggi pollo',9.00,14.00,50,12,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,23,3),

('P0051','La Costeña frijoles negros','Frijoles negros refritos La Costeña',16.00,23.00,36,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,9,6),
('P0052','La Costeña chiles jalapeños','Chiles jalapeños en escabeche',14.00,21.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,9,6),
('P0053','La Costeña rajas','Rajas jalapeño en lata',13.00,20.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,9,6),
('P0054','Herdez salsa verde','Salsa verde Herdez',15.00,22.00,36,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,10,6),
('P0055','Herdez salsa roja','Salsa roja Herdez',15.00,22.00,36,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,10,6),

('P0056','Herdez mayonesa','Mayonesa Herdez',22.00,31.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,10,5),
('P0057','Verde Valle arroz 900 g','Arroz Verde Valle 900 g',24.00,34.00,28,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,11,4),
('P0058','Verde Valle frijol pinto 900 g','Frijol pinto Verde Valle 900 g',25.00,36.00,28,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,11,4),
('P0059','Verde Valle lenteja 500 g','Lenteja Verde Valle 500 g',17.00,25.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,11,4),
('P0060','La Moderna espagueti 200 g','Espagueti La Moderna 200 g',7.00,12.00,70,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,12,3),

('P0061','La Moderna codito 200 g','Pasta codito La Moderna 200 g',7.00,12.00,70,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,12,3),
('P0062','La Moderna sopa letras','Pasta sopa letras La Moderna',7.00,12.00,70,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,12,3),
('P0063','Quaker avena 475 g','Avena Quaker 475 g',22.00,32.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,30,3),
('P0064','Quaker avena 1 kg','Avena Quaker 1 kg',39.00,54.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,30,4),
('P0065','Nescafé Clásico 120 g','Café soluble Nescafé clásico',58.00,79.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,20,1),

('P0066','Nescafé Clásico 200 g','Café soluble Nescafé clásico 200 g',89.00,119.00,12,3,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,20,1),
('P0067','Nestlé Corn Flakes','Cereal hojuelas de maíz',46.00,62.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,19,3),
('P0068','Nestlé Chocapic','Cereal Chocapic Nestlé',49.00,66.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,19,3),
('P0069','Nido Kinder 360 g','Leche en polvo Nido Kinder',78.00,102.00,16,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,21,1),
('P0070','La Lechera lata','Leche condensada La Lechera',24.00,34.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,22,6),

('P0071','Lala leche entera 1 L','Leche entera Lala 1 litro',20.00,29.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,15,11),
('P0072','Lala deslactosada 1 L','Leche deslactosada Lala 1 litro',22.00,31.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,15,11),
('P0073','Alpura leche entera 1 L','Leche entera Alpura 1 litro',20.00,29.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,16,11),
('P0074','Alpura deslactosada 1 L','Leche deslactosada Alpura 1 litro',22.00,31.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,16,11),
('P0075','Nutrileche 1 L','Leche Nutrileche 1 litro',17.00,25.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,17,11),

('P0076','Santa Clara chocolate 1 L','Leche sabor chocolate Santa Clara',24.00,35.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,18,11),
('P0077','Santa Clara vainilla 1 L','Leche sabor vainilla Santa Clara',24.00,35.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,18,11),
('P0078','Lala yogur bebible fresa','Yogur bebible Lala fresa',11.00,17.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,15,5),
('P0079','Alpura yogur natural','Yogur natural Alpura',14.00,20.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,16,5),
('P0080','Nestlé media crema','Media crema Nestlé',17.00,24.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,3,19,6),

('P0081','Bimbo tortillas harina','Tortillas de harina Bimbo',26.00,36.00,18,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,6,3),
('P0082','Gamesa Canelitas','Galletas Gamesa Canelitas',12.00,18.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,2,3),
('P0083','Gamesa Emperador combinado','Galletas Gamesa Emperador combinado',12.50,18.50,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,2,3),
('P0084','Marinela Pingüinos','Pastelito Pingüinos Marinela',13.50,19.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,6,7,1),
('P0085','Barcel Runners','Botana Barcel Runners',11.00,17.00,45,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,8,4),

('P0086','Sabritas Receta Crujiente','Papas Sabritas receta crujiente',12.00,18.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,4,4),
('P0087','Doritos Flamin Hot','Botana Doritos Flamin Hot',13.00,20.00,45,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,34,4),
('P0088','Cheetos Bolita','Botana Cheetos Bolita',12.00,18.00,45,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,35,4),
('P0089','Tostitos Habanero','Botana Tostitos habanero',14.00,21.00,40,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,36,4),
('P0090','Quaker barras avena','Barras de avena Quaker',18.00,26.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,30,3),

('P0091','La Costeña puré tomate','Puré de tomate La Costeña',13.00,19.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,9,6),
('P0092','Herdez mole doña María','Mole Doña María Herdez',36.00,49.00,16,4,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,10,6),
('P0093','Verde Valle garbanzo','Garbanzo Verde Valle 500 g',18.00,26.00,24,6,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,11,4),
('P0094','La Moderna fideo','Fideo La Moderna 200 g',7.00,12.00,60,15,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,12,3),
('P0095','Nescafé Decaf 120 g','Café soluble descafeinado',60.00,82.00,12,3,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,1,20,1),

('P0096','Coca-Cola sin azúcar 600 ml','Refresco Coca-Cola sin azúcar 600 ml',12.50,18.00,36,10,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,1,5),
('P0097','Pepsi Black 600 ml','Refresco Pepsi Black 600 ml',10.50,15.50,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,5,5),
('P0098','Jumex Guayaba 473 ml','Néctar Jumex sabor guayaba',9.50,14.00,30,8,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,13,6),
('P0099','Del Valle Mango 1 L','Jugo Del Valle mango 1 litro',18.00,27.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,14,11),
('P0100','Gatorade Lima Limón 1 L','Bebida deportiva Gatorade lima limón',20.00,30.00,20,5,SYSDATETIME(),NULL,NULL,1,NULL,NULL,1,2,31,11);

