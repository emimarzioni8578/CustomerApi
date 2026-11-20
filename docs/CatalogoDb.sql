/*
    CatalogoDb: base "heredada" para practicar DATABASE FIRST.

    Simula una base que ya existe en la empresa y que mantiene otro equipo (el DBA).
    La API NO la crea ni la modifica con migraciones: la lee y escribe con un DbContext
    generado a partir de ella (dotnet ef dbcontext scaffold).

    Cómo ejecutarlo (cualquiera de las tres):
      - SQL Server Management Studio: abrir este archivo y presionar F5.
      - Extensión SQL Server (mssql) de VS Code: abrir el archivo y "Execute Query".
      - Terminal:  sqlcmd -S .\SQLEXPRESS01 -E -i database\CatalogoDb.sql

    Se puede ejecutar varias veces: borra las tablas y las vuelve a crear con los datos iniciales.
*/

IF DB_ID(N'CatalogoDb') IS NULL
    CREATE DATABASE CatalogoDb;
GO

USE CatalogoDb;
GO

IF OBJECT_ID(N'dbo.Productos', N'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID(N'dbo.Categorias', N'U') IS NOT NULL DROP TABLE dbo.Categorias;
GO

-- Las reglas viven EN LA BASE: claves, únicos, CHECK y valores por defecto.
CREATE TABLE dbo.Categorias
(
    CategoriaId  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categorias PRIMARY KEY,
    Nombre       NVARCHAR(80)      NOT NULL CONSTRAINT UQ_Categorias_Nombre UNIQUE,
    Descripcion  NVARCHAR(300)     NULL
);
GO

CREATE TABLE dbo.Productos
(
    ProductoId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Productos PRIMARY KEY,
    CategoriaId  INT               NOT NULL CONSTRAINT FK_Productos_Categorias
                                            REFERENCES dbo.Categorias (CategoriaId),
    Codigo       VARCHAR(20)       NOT NULL CONSTRAINT UQ_Productos_Codigo UNIQUE,   -- formato AAA-000, ej.: TEC-001
    Nombre       NVARCHAR(120)     NOT NULL,
    Precio       DECIMAL(10,2)     NOT NULL CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
    Stock        INT               NOT NULL CONSTRAINT DF_Productos_Stock DEFAULT (0)
                                            CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0),
    Activo       BIT               NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1),
    FechaAlta    DATETIME2(0)      NOT NULL CONSTRAINT DF_Productos_FechaAlta DEFAULT (SYSUTCDATETIME())
);
GO

INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
    (N'Tecnología', N'Computadoras, periféricos y accesorios'),
    (N'Hogar',      N'Artículos para la casa'),
    (N'Librería',   N'Útiles escolares y papelería');

INSERT INTO dbo.Productos (CategoriaId, Codigo, Nombre, Precio, Stock) VALUES
    (1, 'TEC-001', N'Teclado mecánico',       59.90, 15),
    (1, 'TEC-002', N'Mouse inalámbrico',      24.50, 40),
    (1, 'TEC-003', N'Monitor 24 pulgadas',   189.00,  0),
    (2, 'HOG-001', N'Lámpara de escritorio',  32.00,  8),
    (2, 'HOG-002', N'Set de tazas x4',        18.75, 25),
    (3, 'LIB-001', N'Cuaderno A4 rayado',      4.20, 120),
    (3, 'LIB-002', N'Caja de lápices x12',     6.80, 60);
GO

SELECT p.ProductoId, p.Codigo, p.Nombre, c.Nombre AS Categoria, p.Precio, p.Stock
FROM dbo.Productos p
JOIN dbo.Categorias c ON c.CategoriaId = p.CategoriaId
ORDER BY p.ProductoId;
