using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TOO_SanBenito.UI
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
            UsernameBox.Focus();

        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            ErrorMessage.Text = string.Empty;

            // Obtener credenciales
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Por favor ingresa tu usuario o email");
                UsernameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Por favor ingresa tu contraseña");
                PasswordBox.Focus();
                return;
            }

            ShowLoading(true);

            try
            {
                // Simular validación de credenciales (reemplazar con lógica real)
                await Task.Delay(1500); // Simular llamada a API/Base de datos

                // TODO: Logica real

                bool isValid = ValidateCredentials(username, password);

                if (isValid)
                {
                    // Login exitoso - Navegar al dashboard
                    var mainWindow = new MainWindow(username);
                    mainWindow.Show();
                    this.Close(); // Cerrar ventana de login
                }
                else
                {
                    ShowError("Usuario o contraseña incorrectos");
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error al iniciar sesión: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // TODO: Implementar validación real contra base de datos/API
            // Por ahora, credenciales de prueba:
            return username.Equals("admin", StringComparison.OrdinalIgnoreCase)
                && password == "admin123";
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void ShowLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            LoginButton.IsEnabled = !isLoading;
            UsernameBox.IsEnabled = !isLoading;
            PasswordBox.IsEnabled = !isLoading;
        }
    }
}
