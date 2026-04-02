INSERT INTO Productos (Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, Categoria, Proveedor, FechaCreacion, IdUsuarioCreacion, Activo)
VALUES
-- ELECTRÓNICOS (15)
('LAP001', 'Laptop Dell Inspiron 15', 'i5 8GB 512GB SSD', 12000.00, 15000.00, 8, 3, 'Electrónicos', 'Dell México', GETDATE(), 1, 1),
('MOUSE01', 'Mouse Logitech Wireless', 'Óptico 1600 DPI', 200.00, 250.00, 45, 10, 'Electrónicos', 'Logitech', GETDATE(), 1, 1),
('TECL001', 'Teclado Mecánico RGB', 'Switch Red USB', 640.00, 800.00, 12, 5, 'Electrónicos', 'HyperX', GETDATE(), 1, 1),
('MONI001', 'Monitor Samsung 24" IPS', 'Full HD 75Hz', 3600.00, 4500.00, 6, 2, 'Electrónicos', 'Samsung', GETDATE(), 1, 1),
('IMP001', 'Impresora HP LaserJet', 'B/N 1200 páginas', 2400.00, 3000.00, 4, 1, 'Electrónicos', 'HP', GETDATE(), 1, 1),
('CEL001', 'iPhone 15 128GB', 'Azul nuevo sellado', 14400.00, 18000.00, 3, 1, 'Electrónicos', 'Apple', GETDATE(), 1, 1),
('AUR001', 'Audífonos Sony WH-1000XM5', 'Noise Cancelling', 6400.00, 8000.00, 7, 2, 'Electrónicos', 'Sony', GETDATE(), 1, 1),
('ROU001', 'Router TP-Link AX3000', 'WiFi 6 Dual Band', 1200.00, 1500.00, 15, 5, 'Electrónicos', 'TP-Link', GETDATE(), 1, 1),
('SSD001', 'SSD Samsung 1TB NVMe', 'Gen4 7000MB/s', 1200.00, 1500.00, 20, 5, 'Electrónicos', 'Samsung', GETDATE(), 1, 1),
('RAM001', 'RAM 16GB DDR4 3200MHz', 'Kingston Fury Beast', 800.00, 1000.00, 25, 10, 'Electrónicos', 'Kingston', GETDATE(), 1, 1),

-- PAPELERÍA (10)
('CUAD001', 'Cuaderno Norica 100hjs', 'Rayado A4', 20.00, 25.00, 150, 30, 'Papelería', 'Norica', GETDATE(), 1, 1),
('BOLI001', 'Bolígrafo Pilot G2', 'Pack 12 negro', 40.00, 50.00, 80, 20, 'Papelería', 'Pilot', GETDATE(), 1, 1),
('CARP001', 'Carpetas Plásticas A4', 'Pack 5 colores', 64.00, 80.00, 60, 15, 'Papelería', 'Office Depot', GETDATE(), 1, 1),
('MARC001', 'Marcadores Stabilo', 'Pack 4 colores', 120.00, 150.00, 40, 10, 'Papelería', 'Stabilo', GETDATE(), 1, 1),
('GLUE001', 'Pegamento Blanco', '250ml escolar', 32.00, 40.00, 35, 10, 'Papelería', '3M', GETDATE(), 1, 1),
('TIJ001', 'Tijeras Office', 'Acero inoxidable', 48.00, 60.00, 25, 8, 'Papelería', 'Office Depot', GETDATE(), 1, 1),
('RAP001', 'Cinta Adhesiva Transparente', '18mm x 20m', 16.00, 20.00, 100, 25, 'Papelería', 'Scotch', GETDATE(), 1, 1),
('ESC001', 'Escuadra Plástica', 'Set 45° 30°', 24.00, 30.00, 70, 20, 'Papelería', 'Staedtler', GETDATE(), 1, 1),
('GOMA001', 'Sacapuntas Metálico', 'Doble agujero', 16.00, 20.00, 90, 25, 'Papelería', 'Faber-Castell', GETDATE(), 1, 1),

-- ALIMENTOS/BEBIDAS (10)
('ARRO001', 'Arroz La Corona 5kg', 'Blanco grano largo', 80.00, 100.00, 30, 10, 'Alimentos', 'La Corona', GETDATE(), 1, 1),
('ACEI001', 'Aceite Canola 900ml', 'Vegetal puro', 28.00, 35.00, 40, 15, 'Alimentos', 'Cargill', GETDATE(), 1, 1),
('COCA001', 'Coca Cola 3L', 'Retornable vidrio', 22.00, 28.00, 50, 20, 'Bebidas', 'Coca Cola', GETDATE(), 1, 1),
('LECH001', 'Leche Lala Entera 1L', 'Ultralarga vida', 18.00, 22.00, 60, 15, 'Bebidas', 'Lala', GETDATE(), 1, 1),
('SALSA01', 'Valentina Hot 370ml', 'Salsa picante', 28.00, 35.00, 75, 20, 'Alimentos', 'Grupo Herdez', GETDATE(), 1, 1),
('PAST001', 'Pasta La Moderna 500g', 'Espagueti', 16.00, 20.00, 85, 25, 'Alimentos', 'La Moderna', GETDATE(), 1, 1),
('ATUN001', 'Atún en agua 170g', 'Pack 3 latas', 60.00, 75.00, 35, 10, 'Alimentos', 'Gamesa', GETDATE(), 1, 1),

-- ROPA/CALZADO (10)
('CAMI001', 'Camisa Polo Cotton', 'M azul marino', 320.00, 400.00, 18, 5, 'Ropa', 'H&M', GETDATE(), 1, 1),
('JEAN001', 'Jeans Levi''s 501', 'Straight 32x32', 960.00, 1200.00, 12, 4, 'Ropa', 'Levi''s', GETDATE(), 1, 1),
('ZAPA001', 'Tenis Nike Air', 'Talle 25 negro', 1200.00, 1500.00, 22, 8, 'Calzado', 'Nike', GETDATE(), 1, 1),
('SUIT001', 'Traje Formal Slim', 'Negro 40R', 4000.00, 5000.00, 5, 2, 'Ropa', 'Tom Ford', GETDATE(), 1, 1),

-- HIGIENE/LIMPIEZA (5)
('JABO001', 'Jabón Dove Crema', 'Pack 4 barras', 36.00, 45.00, 55, 15, 'Higiene', 'Unilever', GETDATE(), 1, 1)