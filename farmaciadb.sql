-- -----------------------------------------------------
-- Esquema de la Base de Datos
-- -----------------------------------------------------

Create database farmaciadb;
use farmaciadb;

-- -----------------------------------------------------
-- Tabla `RolUsuario`
-- Almacena los roles que puede tener un usuario (ej: Administrador, Vendedor).
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `RolUsuario` (
  `idRol` INT NOT NULL AUTO_INCREMENT,
  `tipoRol` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`idRol`),
  UNIQUE INDEX `tipoRol_UNIQUE` (`tipoRol` ASC)
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `CategoriasProducto`
-- Almacena las categorías de los productos.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CategoriasProducto` (
  `idCategoria` INT NOT NULL AUTO_INCREMENT,
  `nombreCategoria` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`idCategoria`),
  UNIQUE INDEX `nombreCategoria_UNIQUE` (`nombreCategoria` ASC)
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `Productos`
-- Almacena la información detallada de cada producto.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Productos` (
  `idProducto` INT NOT NULL AUTO_INCREMENT,
  `nombreProducto` VARCHAR(255) NOT NULL,
  `descripcionProducto` TEXT NULL,
  `fechaCaducidad` DATE NULL,
  `PrecioUnitario` DECIMAL(10, 2) NOT NULL,
  `StockProducto` INT NOT NULL DEFAULT 0,
  `idCategoria` INT NOT NULL, -- FK a CategoriasProducto
  PRIMARY KEY (`idProducto`),
  INDEX `fk_Productos_CategoriasProducto_idx` (`idCategoria` ASC),
  CONSTRAINT `fk_Productos_CategoriasProducto`
    FOREIGN KEY (`idCategoria`)
    REFERENCES `CategoriasProducto` (`idCategoria`)
    ON DELETE RESTRICT -- No permite eliminar la categoría si tiene productos asociados
    ON UPDATE CASCADE
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `Venta`
-- Almacena la información general de cada venta.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Venta` (
  `idVenta` INT NOT NULL AUTO_INCREMENT,
  `TotalVenta` DECIMAL(10, 2) NOT NULL,
  `FechaVenta` DATE NOT NULL,
  `HoraVenta` TIME NOT NULL,
  PRIMARY KEY (`idVenta`)
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `DetalleVenta`
-- Almacena los productos que se vendieron en una venta específica.
-- Esta tabla reemplaza a 'ProductosVenta' en tu diseño original para normalizar la DB.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `DetalleVenta` (
  `idDetalle` INT NOT NULL AUTO_INCREMENT,
  `idVenta` INT NOT NULL, -- FK a Venta
  `idProducto` INT NOT NULL, -- FK a Productos
  `cantidadVendida` INT NOT NULL,
  `precioUnitarioVenta` DECIMAL(10, 2) NOT NULL, -- Precio al momento de la venta
  `subtotal` DECIMAL(10, 2) NOT NULL,
  PRIMARY KEY (`idDetalle`),
  INDEX `fk_DetalleVenta_Venta_idx` (`idVenta` ASC),
  INDEX `fk_DetalleVenta_Productos_idx` (`idProducto` ASC),
  CONSTRAINT `fk_DetalleVenta_Venta`
    FOREIGN KEY (`idVenta`)
    REFERENCES `Venta` (`idVenta`)
    ON DELETE CASCADE -- Si se elimina la venta, se eliminan sus detalles
    ON UPDATE CASCADE,
  CONSTRAINT `fk_DetalleVenta_Productos`
    FOREIGN KEY (`idProducto`)
    REFERENCES `Productos` (`idProducto`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `Configuracion`
-- Almacena parámetros de configuración del sistema.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Configuracion` (
  `idConfiguracion` INT NOT NULL AUTO_INCREMENT,
  `stockMinUnidades` INT NOT NULL DEFAULT 5, -- Stock mínimo para generar alerta
  `horaEntrada` TIME NULL,                  -- Hora de inicio de atención
  `horaSalida` TIME NULL,                   -- Hora de fin de atención
  `notificacionesStockB` BOOLEAN NOT NULL DEFAULT TRUE, -- TRUE para activar notificaciones de stock bajo
  PRIMARY KEY (`idConfiguracion`)
) ENGINE = InnoDB;
---

-- -----------------------------------------------------
-- Tabla `Notificacion`
-- Almacena las notificaciones del sistema.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Notificacion` (
  `idNotificacion` INT NOT NULL AUTO_INCREMENT,
  `DescripcionNoti` TEXT NOT NULL,
  `leida` BOOLEAN NOT NULL DEFAULT FALSE,
  `fechaHora` DATETIME NOT NULL,
  PRIMARY KEY (`idNotificacion`)
) ENGINE = InnoDB;

---

-- -----------------------------------------------------
-- Tabla `ReporteVentas`
-- Almacena el reporte agregado de ventas mensuales por categoría.
-- NOTA: Esta tabla está optimizada para reportes pre-agregados, no necesita 'idVenta'.
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ReporteVentas` (
  `idReporte` INT NOT NULL AUTO_INCREMENT,
  `MesVenta` INT NOT NULL, -- 1 a 12
  `AñoVenta` INT NOT NULL, -- Ej: 2025
  `idCategoria` INT NOT NULL, -- FK a CategoriasProducto
  `unidadesVendidas` INT NOT NULL DEFAULT 0, -- Unidades totales vendidas de esa categoría en el mes
  `TotalVendido` DECIMAL(10, 2) NOT NULL DEFAULT 0.00, -- Monto total vendido de esa categoría en el mes
  PRIMARY KEY (`idReporte`),
  UNIQUE INDEX `uq_ReporteMesCategoria` (`MesVenta` ASC, `AñoVenta` ASC, `idCategoria` ASC), -- Asegura un único reporte por mes/año/categoría
  INDEX `fk_ReporteVentas_CategoriasProducto_idx` (`idCategoria` ASC),
  CONSTRAINT `fk_ReporteVentas_CategoriasProducto`
    FOREIGN KEY (`idCategoria`)
    REFERENCES `CategoriasProducto` (`idCategoria`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE = InnoDB;

-- Inserción de Roles
INSERT INTO `RolUsuario` (`tipoRol`) VALUES
('Administrador'),
('Vendedor');

-- Inserción de Categorías de Productos
INSERT INTO `CategoriasProducto` (`nombreCategoria`) VALUES
('Medicamentos - Venta Libre (OTC)'),
('Medicamentos - Con Receta'),
('Medicamentos - Controlados'),
('Dermocosmética'),
('Higiene y Cuidado Personal'),
('Puericultura'),
('Botiquín y Curación');




