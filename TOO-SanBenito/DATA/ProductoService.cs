using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using TOO_SanBenito.Models;
using System;
using System.Data;

namespace TOO_SanBenito.Data
{
    /// <summary>
    /// Gestiona las operaciones CRUD para Productos y la recuperación de Categorias.
    /// Incluye métodos para obtener, agregar, actualizar y eliminar productos.
    /// </summary>
    public class ProductoService
    {
        // ====================================================================
        // MÉTODOS PARA CATEGORÍAS
        // ====================================================================

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
                MessageBox.Show($"Error al cargar categorías. Verifique que la tabla 'CategoriasProducto' exista.\nDetalle: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return categorias;
        }

        // ====================================================================
        // MÉTODOS PARA PRODUCTOS (CRUD)
        // ====================================================================

        public List<Producto> GetProductos()
        {
            List<Producto> productos = new List<Producto>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return productos;

                string query = @"
                    SELECT 
                        p.idProducto, 
                        p.nombreProducto, 
                        p.descripcionProducto, 
                        p.fechaCaducidad, 
                        p.PrecioUnitario, 
                        p.StockProducto, 
                        p.idCategoria,
                        c.nombreCategoria
                    FROM Productos p
                    INNER JOIN CategoriasProducto c ON p.idCategoria = c.idCategoria
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
                            FechaCaducidad = reader.IsDBNull(reader.GetOrdinal("fechaCaducidad")) ? (DateTime?)null : reader.GetDateTime("fechaCaducidad"),
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
                MessageBox.Show($"Error al cargar productos. Verifique que la tabla 'Productos' exista.\nDetalle: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return productos;
        }

        /// <summary>
        /// Obtiene un producto por su ID (usado para la edición).
        /// </summary>
        public Producto GetProductoById(int idProducto)
        {
            Producto producto = null;
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return null;

                string query = @"
                    SELECT 
                        idProducto, nombreProducto, descripcionProducto, fechaCaducidad, 
                        PrecioUnitario, StockProducto, idCategoria
                    FROM Productos
                    WHERE idProducto = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                Id = reader.GetInt32("idProducto"),
                                Nombre = reader.GetString("nombreProducto"),
                                Descripcion = reader.GetString("descripcionProducto"),
                                FechaCaducidad = reader.IsDBNull(reader.GetOrdinal("fechaCaducidad")) ? (DateTime?)null : reader.GetDateTime("fechaCaducidad"),
                                Precio = reader.GetDecimal("PrecioUnitario"),
                                Stock = reader.GetInt32("StockProducto"),
                                IdCategoria = reader.GetInt32("idCategoria")
                            };
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al obtener el producto (ID: {idProducto}): {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return producto;
        }

        public bool AgregarProducto(Producto nuevoProducto)
        {
            MySqlConnection conn = null;
            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                string query = @"
                    INSERT INTO Productos (nombreProducto, descripcionProducto, fechaCaducidad, PrecioUnitario, StockProducto, idCategoria) 
                    VALUES (@nombre, @descripcion, @fechaCaducidad, @precio, @stock, @idCategoria)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nuevoProducto.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", nuevoProducto.Descripcion);

                    if (nuevoProducto.FechaCaducidad.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fechaCaducidad", nuevoProducto.FechaCaducidad.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@fechaCaducidad", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@precio", nuevoProducto.Precio);
                    cmd.Parameters.AddWithValue("@stock", nuevoProducto.Stock);
                    cmd.Parameters.AddWithValue("@idCategoria", nuevoProducto.IdCategoria);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al agregar el producto: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }

        /// <summary>
        /// Actualiza un producto existente en la base de datos.
        /// </summary>
        public bool ActualizarProducto(Producto productoAEditar)
        {
            MySqlConnection conn = null;
            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                string query = @"
                    UPDATE Productos SET
                        nombreProducto = @nombre, 
                        descripcionProducto = @descripcion, 
                        fechaCaducidad = @fechaCaducidad, 
                        PrecioUnitario = @precio, 
                        StockProducto = @stock, 
                        idCategoria = @idCategoria
                    WHERE idProducto = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productoAEditar.Id);
                    cmd.Parameters.AddWithValue("@nombre", productoAEditar.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", productoAEditar.Descripcion);

                    if (productoAEditar.FechaCaducidad.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fechaCaducidad", productoAEditar.FechaCaducidad.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@fechaCaducidad", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@precio", productoAEditar.Precio);
                    cmd.Parameters.AddWithValue("@stock", productoAEditar.Stock);
                    cmd.Parameters.AddWithValue("@idCategoria", productoAEditar.IdCategoria);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al actualizar el producto: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }

        /// <summary>
        /// Elimina un producto por su ID.
        /// </summary>
        public bool EliminarProducto(int idProducto)
        {
            MySqlConnection conn = null;
            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                string query = "DELETE FROM Productos WHERE idProducto = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }
    }
}