using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using TOO_SanBenito.Models;
using System.Text;

namespace TOO_SanBenito.Data
{
    // Modelo para cada línea de detalle
    public class DetalleVentaSimple
    {
        public int Cantidad { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    // Clase para estructurar los datos de la venta del día en el dashboard
    public class VentaDia
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int CantidadProductos { get; set; }
        public string HoraVenta => Fecha.ToString("HH:mm");
    }

    // Estructura de resumen para las tarjetas
    public class ResumenVentaDia
    {
        public decimal TotalVenta { get; set; } = 0.00m;
        public int NumTransacciones { get; set; } = 0;
        public int TotalProductosVendidos { get; set; } = 0;
        public decimal PromedioVenta { get; set; } = 0.00m;
        public string UltimaVentaHora { get; set; } = "--:--";
        public List<VentaDia> Ventas { get; set; } = new List<VentaDia>();
    }

    public class DashboardVendedorService
    {
        // -------------------------------------------------------------------
        // MÉTODOS ANTERIORES: GetVentasDelDia (Resumen)
        // -------------------------------------------------------------------

        public ResumenVentaDia GetVentasDelDia()
        {
            var resumen = new ResumenVentaDia();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return resumen;

                string today = DateTime.Now.ToString("yyyy-MM-dd");

                string queryVentas = $@"
                    SELECT 
                        v.idVenta, 
                        v.TotalVenta, 
                        v.FechaVenta, 
                        v.HoraVenta,
                        SUM(dv.cantidadVendida) AS ProductosVendidos
                    FROM Venta v
                    JOIN DetalleVenta dv ON v.idVenta = dv.idVenta
                    WHERE DATE(v.FechaVenta) = '{today}'
                    GROUP BY v.idVenta, v.TotalVenta, v.FechaVenta, v.HoraVenta
                    ORDER BY v.HoraVenta DESC";

                using (MySqlCommand cmdVentas = new MySqlCommand(queryVentas, conn))
                using (MySqlDataReader reader = cmdVentas.ExecuteReader())
                {
                    decimal totalGeneral = 0;
                    int totalItems = 0;
                    int numTransacciones = 0;
                    DateTime? ultimaHora = null;

                    while (reader.Read())
                    {
                        numTransacciones++;
                        int idVenta = reader.GetInt32("idVenta");

                        DateTime fechaVenta = reader.GetDateTime("FechaVenta");
                        TimeSpan horaVenta = reader.GetTimeSpan("HoraVenta");
                        DateTime fechaCompleta = fechaVenta.Date.Add(horaVenta);

                        decimal totalVenta = reader.GetDecimal("TotalVenta");
                        int productosVendidos = reader.GetInt32("ProductosVendidos");

                        totalGeneral += totalVenta;
                        totalItems += productosVendidos;

                        if (ultimaHora == null || fechaCompleta > ultimaHora)
                        {
                            ultimaHora = fechaCompleta;
                        }

                        resumen.Ventas.Add(new VentaDia
                        {
                            Id = idVenta,
                            Fecha = fechaCompleta,
                            Total = totalVenta,
                            CantidadProductos = productosVendidos,
                        });
                    }

                    resumen.TotalVenta = totalGeneral;
                    resumen.NumTransacciones = numTransacciones;
                    resumen.TotalProductosVendidos = totalItems;
                    resumen.PromedioVenta = numTransacciones > 0 ? totalGeneral / numTransacciones : 0.00m;
                    resumen.UltimaVentaHora = ultimaHora?.ToString("HH:mm") ?? "--:--";
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar ventas del vendedor: {ex.Message}", "Error de Dashboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return resumen;
        }

        // -------------------------------------------------------------------
        // NUEVO MÉTODO: Obtener detalles de una venta específica
        // -------------------------------------------------------------------

        /// <summary>
        /// Obtiene los productos y cantidades de una venta específica.
        /// </summary>
        public List<DetalleVentaSimple> GetDetallesVenta(int idVenta)
        {
            var detalles = new List<DetalleVentaSimple>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return detalles;

                string query = $@"
                    SELECT 
                        dv.cantidadVendida,
                        dv.precioUnitarioVenta,
                        p.nombreProducto
                    FROM DetalleVenta dv
                    JOIN Productos p ON dv.idProducto = p.idProducto
                    WHERE dv.idVenta = @idVenta";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idVenta", idVenta);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int cantidad = reader.GetInt32("cantidadVendida");
                            decimal precio = reader.GetDecimal("precioUnitarioVenta");
                            string nombre = reader.GetString("nombreProducto");

                            detalles.Add(new DetalleVentaSimple
                            {
                                Cantidad = cantidad,
                                NombreProducto = nombre,
                                PrecioUnitario = precio
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar detalles de la Venta {idVenta}: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return detalles;
        }
    }
}