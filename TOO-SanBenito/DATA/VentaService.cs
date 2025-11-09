using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using TOO_SanBenito.Models;
using TOO_SanBenito.UI;

namespace TOO_SanBenito.Data
{
    /// <summary>
    /// Servicio para manejar las operaciones de Venta y recuperación de Productos/Categorías.
    /// </summary>
    public class VentaService
    {
        // ... [CÓDIGO GetProductosDisponibles y GetCategorias ANTERIOR] ...
        // (Asumimos que el código anterior para cargar productos y categorías está aquí)

        // Nota: Por restricciones de espacio, omito GetProductosDisponibles y GetCategorias,
        // asumiendo que ya existen y funcionan en tu archivo.

        public List<Producto> GetProductosDisponibles()
        {
            List<Producto> productos = new List<Producto>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return productos;

                // Solo productos con stock > 0
                string query = @"
                    SELECT 
                        p.idProducto, 
                        p.nombreProducto, 
                        p.descripcionProducto, 
                        p.PrecioUnitario, 
                        p.StockProducto, 
                        p.idCategoria,
                        c.nombreCategoria
                    FROM Productos p
                    INNER JOIN CategoriasProducto c ON p.idCategoria = c.idCategoria
                    WHERE p.StockProducto > 0
                    ORDER BY p.nombreProducto";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            Id = reader.GetInt32("idProducto"),
                            Nombre = reader.GetString("nombreProducto"),
                            Descripcion = reader.GetString("descripcionProducto"),
                            Precio = reader.GetDecimal("PrecioUnitario"),
                            Stock = reader.GetInt32("StockProducto"),
                            IdCategoria = reader.GetInt32("idCategoria"),
                            CategoriaNombre = reader.GetString("nombreCategoria")
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar productos para Venta: {ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return productos;
        }

        public List<Categoria> GetCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return categorias;

                string query = "SELECT idCategoria, nombreCategoria FROM CategoriasProducto ORDER BY nombreCategoria";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categorias.Add(new Categoria
                        {
                            Id = reader.GetInt32("idCategoria"),
                            Nombre = reader.GetString("nombreCategoria")
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar categorías para Venta: {ex.Message}", "Error DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return categorias;
        }


        /// <summary>
        /// Registra la venta y sus detalles en una transacción, y actualiza el stock.
        /// </summary>
        /// <param name="totalVenta">Monto total de la venta.</param>
        /// <param name="carritoItems">Lista de ítems vendidos.</param>
        /// <returns>True si la transacción fue exitosa.</returns>
        public bool RegistrarVenta(decimal totalVenta, IEnumerable<ItemCarrito> carritoItems)
        {
            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                transaction = conn.BeginTransaction();
                int idVenta = 0;

                // 1. Insertar en la tabla Venta
                string ventaQuery = @"
                    INSERT INTO Venta (TotalVenta, FechaVenta, HoraVenta) 
                    VALUES (@total, CURDATE(), CURTIME());
                    SELECT LAST_INSERT_ID();"; // Obtener el ID de la venta recién insertada

                using (MySqlCommand ventaCmd = new MySqlCommand(ventaQuery, conn, transaction))
                {
                    ventaCmd.Parameters.AddWithValue("@total", totalVenta);
                    // Ejecutar y obtener el ID
                    idVenta = Convert.ToInt32(ventaCmd.ExecuteScalar());
                }

                // 2. Insertar DetalleVenta y actualizar Stock
                string detalleQuery = @"
                    INSERT INTO DetalleVenta (idVenta, idProducto, cantidadVendida, precioUnitarioVenta, subtotal) 
                    VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @subtotal);";

                string stockUpdateQuery = @"
                    UPDATE Productos SET StockProducto = StockProducto - @cantidad 
                    WHERE idProducto = @idProducto;";

                foreach (var item in carritoItems)
                {
                    // a) Insertar Detalle
                    using (MySqlCommand detalleCmd = new MySqlCommand(detalleQuery, conn, transaction))
                    {
                        detalleCmd.Parameters.AddWithValue("@idVenta", idVenta);
                        detalleCmd.Parameters.AddWithValue("@idProducto", item.Producto.Id);
                        detalleCmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                        detalleCmd.Parameters.AddWithValue("@precioUnitario", item.Producto.Precio);
                        detalleCmd.Parameters.AddWithValue("@subtotal", item.Subtotal);
                        detalleCmd.ExecuteNonQuery();
                    }

                    // b) Actualizar Stock
                    using (MySqlCommand stockCmd = new MySqlCommand(stockUpdateQuery, conn, transaction))
                    {
                        stockCmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                        stockCmd.Parameters.AddWithValue("@idProducto", item.Producto.Id);
                        stockCmd.ExecuteNonQuery();
                    }
                }

                // 3. Confirmar la transacción
                transaction.Commit();
                return true;
            }
            catch (MySqlException ex)
            {
                // Si ocurre un error, revertir todos los cambios
                transaction?.Rollback();
                MessageBox.Show($"Error al registrar la venta. La transacción fue revertida.\nDetalle: {ex.Message}", "Error de Venta", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Error inesperado al registrar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }
    }
}