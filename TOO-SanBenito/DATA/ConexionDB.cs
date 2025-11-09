using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace TOO_SanBenito.Data
{
    /// <summary>
    /// Clase estática para gestionar la conexión a la base de datos MySQL.
    /// NOTA: Necesita el paquete NuGet MySql.Data.
    /// </summary>
    public static class ConexionDB
    {
        // CADENA DE CONEXIÓN: ¡IMPORTANTE! REEMPLAZA "tu_contraseña_mysql" con tu contraseña real.
        // Asegúrate que tu servidor MySQL esté corriendo en localhost (127.0.0.1) y que el usuario 'root' tenga acceso.
        private static string connectionString = "Server=localhost;Database=farmaciadb;Uid=root;Pwd=;";

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos.
        /// </summary>
        /// <returns>Objeto MySqlConnection abierto, o null si falla.</returns>
        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
                // Muestra un error visible si la conexión falla (Generalmente por contraseña o servidor apagado)
                MessageBox.Show($"Error de conexión a la base de datos. Verifique sus credenciales y que MySQL esté corriendo.\nDetalle: {ex.Message}", "Error de Conexión CRÍTICO", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Cierra la conexión de forma segura.
        /// </summary>
        /// <param name="connection">La conexión a cerrar.</param>
        public static void CloseConnection(MySqlConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                try
                {
                    connection.Close();
                }
                catch (MySqlException)
                {
                    // Ignorar errores al cerrar la conexión
                }
            }
        }
    }
}
