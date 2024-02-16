-- Crear la base de datos
CREATE DATABASE AxoftExam;

-- Seleccionar la base de datos
USE AxoftExam;

-- Tabla Articulo
CREATE TABLE Articulo (
    Id INT PRIMARY KEY IDENTITY,
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Descripcion VARCHAR(MAX) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL
);

-- Tabla Cliente
CREATE TABLE Cliente (
    Id INT PRIMARY KEY IDENTITY,
    Cuil VARCHAR(11) NOT NULL UNIQUE,
    Nombre VARCHAR(MAX) NOT NULL,
    Direccion VARCHAR(MAX) NOT NULL
);

-- Tabla Factura
CREATE TABLE Factura (
    Id INT PRIMARY KEY IDENTITY,
    Numero INT NOT NULL,
    Fecha DATETIME NOT NULL,
    ClienteId INT NOT NULL,
    TotalSinImpuestos DECIMAL(18,2) NOT NULL,
    Iva DECIMAL(18,2) NOT NULL,
    ImporteIva DECIMAL(18,2) NOT NULL,
    TotalConImpuestos DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Cliente(Id)
);

-- Tabla RenglonFactura
CREATE TABLE RenglonFactura (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT NOT NULL,
    ArticuloId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (FacturaId) REFERENCES Factura(Id),
    FOREIGN KEY (ArticuloId) REFERENCES Articulo(Id)
);

-- Índices
CREATE INDEX IX_Articulo_Codigo ON Articulo (Codigo);
CREATE INDEX IX_Cliente_Cuil ON Cliente (Cuil);
