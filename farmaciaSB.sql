-- -----------------------------------------------------
-- Creación de la Base de Datos
-- -----------------------------------------------------
CREATE DATABASE IF NOT EXISTS farmaciaSB;
USE farmaciaSB;

-- -----------------------------------------------------
-- Tabla Categoria
-- farmacia se relaciona con categoria (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Categoria (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL
);

-- -----------------------------------------------------
-- Tabla Farmacia
-- Es la entidad principal, muchas tablas referencian a esta.
-- -----------------------------------------------------
CREATE TABLE Farmacia (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(255) NOT NULL,
    direccion VARCHAR(255),
    telefono VARCHAR(20)
);

-- -----------------------------------------------------
-- Tabla Usuario
-- farmacia se relaciona con usuario (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Usuario (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    rol VARCHAR(50),
    telefono VARCHAR(20),
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id)
);

-- -----------------------------------------------------
-- Tabla Producto
-- farmacia se relaciona con producto (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Producto (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    id_categoria INT,
    nombre VARCHAR(255) NOT NULL,
    descripcion TEXT,
    precio DECIMAL(10, 2) NOT NULL,
    stock INT NOT NULL DEFAULT 0,
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id),
    FOREIGN KEY (id_categoria) REFERENCES Categoria(id)
);

-- -----------------------------------------------------
-- Tabla Inventario (Tabla maestra para los movimientos de stock)
-- Se mantiene la tabla Inventario, pero la lista de productos se maneja
-- con una tabla de relación (InventarioProducto).
-- -----------------------------------------------------
CREATE TABLE Inventario (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    -- Campos adicionales si son necesarios (ej. fecha_actualizacion)
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id)
);

-- -----------------------------------------------------
-- Tabla InventarioProducto (Tabla de relación M:M entre Inventario y Producto)
-- producto se relaciona con inventario (1 a 0..*) -> Se implementa como M:M
-- -----------------------------------------------------
CREATE TABLE InventarioProducto (
    id_inventario INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad_actual INT NOT NULL,
    PRIMARY KEY (id_inventario, id_producto),
    FOREIGN KEY (id_inventario) REFERENCES Inventario(id),
    FOREIGN KEY (id_producto) REFERENCES Producto(id)
);

-- -----------------------------------------------------
-- Tabla Venta
-- farmacia se relaciona con venta (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Venta (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    id_usuario INT NOT NULL,
    fecha DATETIME NOT NULL,
    total DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id)
);

-- -----------------------------------------------------
-- Tabla DetalleVenta
-- venta se relaciona con detalleVenta (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE DetalleVenta (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_venta INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id),
    FOREIGN KEY (id_producto) REFERENCES Producto(id)
);

-- -----------------------------------------------------
-- Tabla Factura
-- venta se relaciona con factura (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Factura (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_venta INT UNIQUE NOT NULL, -- UNIQUE para asegurar 1 factura por venta
    monto_total DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id)
);

-- -----------------------------------------------------
-- Tabla Proveedor
-- farmacia se relaciona con Proveedor (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Proveedor (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    telefono VARCHAR(20),
    direccion VARCHAR(255),
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id)
);

-- -----------------------------------------------------
-- Tabla Pedido
-- Proveedor se relaciona con Pedido (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Pedido (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_proveedor INT NOT NULL,
    fecha_pedido DATE NOT NULL,
    -- Campos adicionales como estado, total, etc.
    FOREIGN KEY (id_proveedor) REFERENCES Proveedor(id)
);

-- -----------------------------------------------------
-- Tabla DetallePedido (Para manejar los productos en un Pedido)
-- Se crea para manejar la lista de productos de la clase Pedido.
-- -----------------------------------------------------
CREATE TABLE DetallePedido (
    id_pedido INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad INT NOT NULL,
    precio_compra DECIMAL(10, 2) NOT NULL,
    PRIMARY KEY (id_pedido, id_producto),
    FOREIGN KEY (id_pedido) REFERENCES Pedido(id),
    FOREIGN KEY (id_producto) REFERENCES Producto(id)
);

-- -----------------------------------------------------
-- Tabla ReporteVenta
-- farmacia se relaciona con ReporteVenta (1 a 0..*)
-- Nota: En la práctica, los reportes se generan de la tabla Venta, pero se incluye
-- como tabla si requiere almacenar resúmenes precalculados.
-- -----------------------------------------------------
CREATE TABLE ReporteVenta (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    fecha DATE NOT NULL UNIQUE, -- Único por día si es reporte diario
    total_ventas DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id)
);

-- -----------------------------------------------------
-- Tabla HistorialVentas
-- farmacia se relaciona con HistorialVentas (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE HistorialVentas (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    id_venta INT NOT NULL,
    fecha DATETIME NOT NULL,
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id),
    FOREIGN KEY (id_venta) REFERENCES Venta(id)
);

-- -----------------------------------------------------
-- Tabla Auditoria
-- farmacia se relaciona con Auditoria (1 a 0..*)
-- -----------------------------------------------------
CREATE TABLE Auditoria (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    id_usuario INT, -- Quién realizó la acción
    accion VARCHAR(255) NOT NULL,
    fecha DATETIME NOT NULL,
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id)
);

-- -----------------------------------------------------
-- Tabla Configuracion
-- farmacia se relaciona con Configuracion (1 a 0..*)
-- Se usa la relación 1:M y se modela el 'Map' de parámetros como pares clave-valor
-- -----------------------------------------------------
CREATE TABLE Configuracion (
    id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_farmacia INT NOT NULL,
    parametro_clave VARCHAR(100) NOT NULL,
    parametro_valor VARCHAR(255),
    FOREIGN KEY (id_farmacia) REFERENCES Farmacia(id),
    UNIQUE KEY (id_farmacia, parametro_clave)
);