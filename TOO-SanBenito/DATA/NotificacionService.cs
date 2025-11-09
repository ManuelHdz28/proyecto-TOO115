using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TOO_SanBenito.Models; // Asumimos que los modelos están disponibles

namespace TOO_SanBenito.Data
{
    // Modelo simplificado para la notificación
    public class NotificacionDisplay
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public Brush Color { get; set; } = Brushes.Gray;
        public string Icono { get; set; } = "•";
        public bool Leida { get; set; }
    }

    /// <summary>
    /// Gestiona la inserción y consulta de notificaciones en la tabla Notificacion.
    /// </summary>
    public class NotificacionService
    {
        // Método de AgregarNotificacion (asumimos que ya existe)
        public bool AgregarNotificacion(string descripcion)
        {
            MySqlConnection conn = null;
            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                string query = @"
                    INSERT INTO Notificacion (DescripcionNoti, fechaHora, leida) 
                    VALUES (@descripcion, NOW(), FALSE)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al agregar notificación: {ex.Message}", "Error de Base de Datos (Notif)", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }


        // ------------------------------------------------------------------
        // MÉTODOS PARA EL BOTÓN DE CAMPANA
        // ------------------------------------------------------------------

        public List<NotificacionDisplay> GetNotificacionesNoLeidas()
        {
            var notificaciones = new List<NotificacionDisplay>();
            MySqlConnection conn = null;

            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return notificaciones;

                string query = @"
                    SELECT idNotificacion, DescripcionNoti, fechaHora 
                    FROM Notificacion 
                    WHERE leida = FALSE
                    ORDER BY fechaHora DESC 
                    LIMIT 20";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string desc = reader.GetString("DescripcionNoti");
                        Brush color;
                        string icono;

                        // Lógica para asignar color e icono basada en el tipo de alerta
                        if (desc.Contains("CRÍTICO") || desc.Contains("VENCIDO"))
                        {
                            color = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // DangerBrush
                            icono = "❌";
                        }
                        else if (desc.Contains("Stock Bajo") || desc.Contains("Advertencia"))
                        {
                            color = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // WarningBrush
                            icono = "⚠️";
                        }
                        else
                        {
                            color = new SolidColorBrush(Color.FromRgb(59, 130, 246)); // Accent/Info
                            icono = "🔔";
                        }

                        notificaciones.Add(new NotificacionDisplay
                        {
                            Id = reader.GetInt32("idNotificacion"),
                            Descripcion = desc,
                            FechaHora = reader.GetDateTime("fechaHora"),
                            Icono = icono,
                            Color = color
                        });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al cargar notificaciones no leídas: {ex.Message}", "Error de Notificaciones", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }

            return notificaciones;
        }

        public bool MarcarComoLeida(int idNotificacion)
        {
            MySqlConnection conn = null;
            try
            {
                conn = ConexionDB.GetConnection();
                if (conn == null) return false;

                string query = "UPDATE Notificacion SET leida = TRUE WHERE idNotificacion = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idNotificacion);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al marcar notificación {idNotificacion} como leída: {ex.Message}", "Error de Notificaciones", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                ConexionDB.CloseConnection(conn);
            }
        }
    }
}