using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TOO_SanBenito.Models;
using System.Text;

namespace TOO_SanBenito.Data
{
    // Modelo de datos para la Actividad Reciente del Dashboard
    public class ActividadReciente
    {
        public string Tipo { get; set; } // Ej: Venta, Stock Bajo, Vencimiento
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public Brush Color { get; set; } = Brushes.Gray;
        public string Icono { get; set; } = "•";
    }

    /// <summary>
    /// Servicio para obtener todas las métricas y datos necesarios para el Dashboard.
    /// </summary>
    public class DashboardService
    {
        // -------------------------------------------------------------------
        // Métricas Principales (Tarjetas)
        // -------------------------------------------------------------------

        public decimal GetVentasHoy()
        {
            decimal total = 0;
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return total;

                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string query = $"SELECT COALESCE(SUM(TotalVenta), 0) FROM Venta WHERE DATE(FechaVenta) = '{today}'";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar ventas de hoy: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return total;
        }

        public decimal GetVentasSemanales()
        {
            decimal total = 0;
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return total;

                // YEARWEEK(FechaVenta, 1) utiliza el Lunes como primer día de la semana
                string query = $@"
                    SELECT COALESCE(SUM(TotalVenta), 0) FROM Venta 
                    WHERE YEARWEEK(FechaVenta, 1) = YEARWEEK(NOW(), 1)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar ventas semanales: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return total;
        }

        public int GetTotalProductos()
        {
            int count = 0;
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return count;

                string query = "SELECT COUNT(idProducto) FROM Productos";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar total de productos: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return count;
        }

        public int GetStockBajoCount()
        {
            int count = 0;
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return count;

                // Umbral de stock bajo es 10
                string query = "SELECT COUNT(idProducto) FROM Productos WHERE StockProducto < 10";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar stock bajo: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return count;
        }

        // -------------------------------------------------------------------
        // Contenido Adicional (Texto y Actividad)
        // -------------------------------------------------------------------

        public string GetVentasPorCategoriaSummary()
        {
            // Reutilizamos el ReporteService para obtener el resumen de ventas
            var reporteService = new ReporteService();
            var datos = reporteService.GetVentasPorCategoria();

            if (!datos.Any())
            {
                return "Aún no hay ventas registradas en el sistema.";
            }

            var totalUnidades = datos.Sum(d => d.UnidadesVendidas);
            var resumen = new StringBuilder();

            resumen.AppendLine($"Total de unidades vendidas: {totalUnidades:N0}\n");

            foreach (var item in datos.OrderByDescending(d => d.UnidadesVendidas))
            {
                double porcentaje = (double)item.UnidadesVendidas / totalUnidades * 100;
                resumen.AppendLine($"- {item.NombreCategoria}: {item.UnidadesVendidas:N0} unidades ({porcentaje:N1}%)");
            }

            return resumen.ToString().Trim();
        }

        /// <summary>
        /// Obtiene las ventas más recientes del día y las alertas de la tabla Notificacion.
        /// </summary>
        public List<ActividadReciente> GetActividadReciente()
        {
            var actividades = new List<ActividadReciente>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return actividades;

                // 1. Obtener las últimas 5 ventas del día (Actividad de Venta)
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string queryVentas = $@"
                    SELECT idVenta, TotalVenta, FechaVenta, HoraVenta 
                    FROM Venta 
                    WHERE DATE(FechaVenta) = '{today}'
                    ORDER BY HoraVenta DESC 
                    LIMIT 5";

                using (MySqlCommand cmdVentas = new MySqlCommand(queryVentas, conn))
                using (MySqlDataReader readerVentas = cmdVentas.ExecuteReader())
                {
                    while (readerVentas.Read())
                    {
                        DateTime fechaVenta = readerVentas.GetDateTime("FechaVenta");
                        TimeSpan horaVenta = readerVentas.GetTimeSpan("HoraVenta");
                        DateTime fechaHora = fechaVenta.Date.Add(horaVenta);

                        actividades.Add(new ActividadReciente
                        {
                            Tipo = "Venta",
                            Descripcion = $"Venta #{readerVentas.GetInt32("idVenta")} por {readerVentas.GetDecimal("TotalVenta"):C2}",
                            FechaHora = fechaHora,
                            Icono = "🛒",
                            // Usamos el color hexadecimal directo para mayor robustez
                            Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) // SuccessBrush
                        });
                    }
                }

                // 2. Obtener Notificaciones (Alertas de Stock/Vencimiento) del día
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                string queryAlertas = $@"
                    SELECT DescripcionNoti, fechaHora 
                    FROM Notificacion 
                    WHERE DATE(fechaHora) = '{today}'
                    ORDER BY fechaHora DESC 
                    LIMIT 5";

                using (MySqlCommand cmdAlertas = new MySqlCommand(queryAlertas, conn))
                using (MySqlDataReader readerAlertas = cmdAlertas.ExecuteReader())
                {
                    while (readerAlertas.Read())
                    {
                        string desc = readerAlertas.GetString("DescripcionNoti");
                        Brush color;
                        string icono;

                        // Determinar color e icono basado en el contenido de la notificación
                        if (desc.Contains("CRÍTICO") || desc.Contains("VENCIDO"))
                        {
                            color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")); // DangerBrush
                            icono = "❌";
                        }
                        else if (desc.Contains("Stock Bajo") || desc.Contains("Advertencia"))
                        {
                            color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")); // WarningBrush
                            icono = "⚠️";
                        }
                        else
                        {
                            color = Brushes.Gray;
                            icono = "🔔";
                        }

                        actividades.Add(new ActividadReciente
                        {
                            Tipo = "Alerta",
                            Descripcion = desc,
                            FechaHora = readerAlertas.GetDateTime("fechaHora"),
                            Icono = icono,
                            Color = color
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar actividad reciente: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general en actividad: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            // Ordenar por hora/fecha y tomar los más recientes (ventas y alertas)
            return actividades.OrderByDescending(a => a.FechaHora).ToList();
        }
    }
}