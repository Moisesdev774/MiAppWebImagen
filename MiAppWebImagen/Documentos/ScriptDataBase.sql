CREATE DATABASE MiAppImagenDB;
GO

USE MiAppImagenDB;
GO

CREATE TABLE Imagen (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Campo Id con autoincremento
    Nombre NVARCHAR(MAX) NOT NULL,    -- Campo Nombre (nombre de la imagen)
    RutaImagen NVARCHAR(MAX) NOT NULL -- Campo RutaImagen (ruta de la imagen en el sistema de archivos)
);
GO