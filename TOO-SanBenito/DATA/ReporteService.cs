using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TOO_SanBenito.Models;
using TOO_SanBenito.UI;

namespace TOO_SanBenito.Data
{
    // Modelo de datos interno para Reportes de Ventas por Tipo
    public class ReporteCategoria
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public int UnidadesVendidas { get; set; }
    }

    // Modelo de datos para Reportes por Hora
    public class ReporteVentaHora
    {
        public int Hora { get; set; } // 0 a 23
        public int CantidadVentas { get; set; }
    }

    // Modelo de datos para Reportes de Stock (usa el modelo ReporteStock de Reportes.xaml.cs)

    /// <summary>
    /// Servicio para manejar la obtención de datos analíticos para los reportes.
    /// </summary>
    public class ReporteService
    {
        // -------------------------------------------------------------------
        // MÉTODOS ANTERIORES: VENTAS POR CATEGORÍA
        // -------------------------------------------------------------------

        public List<ReporteCategoria> GetVentasPorCategoria()
        {
            List<ReporteCategoria> reporte = new List<ReporteCategoria>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return reporte;

                string query = @"
                    SELECT 
                        cp.nombreCategoria, 
                        SUM(dv.cantidadVendida) AS TotalUnidades
                    FROM DetalleVenta dv
                    JOIN Productos p ON dv.idProducto = p.idProducto
                    JOIN CategoriasProducto cp ON p.idCategoria = cp.idCategoria
                    GROUP BY cp.nombreCategoria
                    ORDER BY TotalUnidades DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reporte.Add(new ReporteCategoria
                        {
                            NombreCategoria = reader.GetString("nombreCategoria"),
                            UnidadesVendidas = reader.GetInt32("TotalUnidades")
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar ventas por categoría: {ex.Message}", "Error de Reportes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return reporte;
        }

        // -------------------------------------------------------------------
        // MÉTODOS ANTERIORES: VENTAS POR HORA
        // -------------------------------------------------------------------

        public List<ReporteVentaHora> GetVentasPorHora()
        {
            List<ReporteVentaHora> reporte = new List<ReporteVentaHora>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return reporte;

                string query = @"
                    SELECT 
                        HOUR(HoraVenta) AS Hora, 
                        COUNT(idVenta) AS CantidadVentas
                    FROM Venta
                    GROUP BY HOUR(HoraVenta)
                    ORDER BY Hora ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reporte.Add(new ReporteVentaHora
                        {
                            Hora = reader.GetInt32("Hora"),
                            CantidadVentas = reader.GetInt32("CantidadVentas")
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar ventas por hora: {ex.Message}", "Error de Reportes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return reporte;
        }

        // -------------------------------------------------------------------
        // NUEVO MÉTODO: REPORTE DE STOCK
        // -------------------------------------------------------------------

        /// <summary>
        /// Obtiene el listado completo de productos para el reporte de stock.
        /// </summary>
        public List<ReporteStock> GetReporteStock()
        {
            List<ReporteStock> reporte = new List<ReporteStock>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return reporte;

                // Consulta para obtener todos los productos y su categoría
                string query = @"
                    SELECT 
                        p.nombreProducto, 
                        p.StockProducto, 
                        p.PrecioUnitario,
                        cp.nombreCategoria
                    FROM Productos p
                    JOIN CategoriasProducto cp ON p.idCategoria = cp.idCategoria
                    ORDER BY p.StockProducto ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int stock = reader.GetInt32("StockProducto");
                        decimal precio = reader.GetDecimal("PrecioUnitario");

                        reporte.Add(new ReporteStock
                        {
                            Nombre = reader.GetString("nombreProducto"),
                            CategoriaNombre = reader.GetString("nombreCategoria"),
                            Stock = stock,
                            Precio = precio,
                            ValorTotal = stock * precio,
                            // StockBajo se calcula en Reportes.xaml.cs (o se podría poner aquí si se define el umbral)
                            // Por ahora, asumimos que el umbral es 10, tal como se usa en Productos.cs
                            StockBajo = stock < 10
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar reporte de stock: {ex.Message}", "Error de Reportes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return reporte;
        }
    }
}