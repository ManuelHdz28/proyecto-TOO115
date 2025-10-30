using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TOO_SanBenito
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            // Inicializar con el dashboard
            NavigateToDashboard();

            // Iniciar reloj en el footer
            StartClock();
        }

        public MainWindow(string username) : this()
        {
            // Constructor que recibe el nombre de usuario desde login
            TxtUserName.Text = username;
            TxtUsuarioActual.Text = $"Usuario: {username}";
        }

        private void NavigateToDashboard()
        {
            MainFrame.Navigate(new UI.Page1());
            TxtPageTitle.Text = "Dashboard";
            HighlightButton(BtnDashboard);
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Page1());
            TxtPageTitle.Text = "Dashboard";
            HighlightButton(BtnDashboard);
        }

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Crear página de usuarios
            // MainFrame.Navigate(new UI.UsuariosPage());
            TxtPageTitle.Text = "Usuarios";
            HighlightButton(BtnProductos);
            MessageBox.Show("Página de Usuarios en construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Crear página de reportes
            // MainFrame.Navigate(new UI.ReportesPage());
            TxtPageTitle.Text = "Reportes";
            HighlightButton(BtnReportes);
            MessageBox.Show("Página de Reportes en construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Crear página de configuración
            // MainFrame.Navigate(new UI.ConfiguracionPage());
            TxtPageTitle.Text = "Configuración";
            HighlightButton(BtnConfiguracion);
            MessageBox.Show("Página de Configuración en construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
              "¿Estás seguro que deseas cerrar sesión?",
              "Cerrar Sesión",
                 MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Volver al login
                var loginWindow = new UI.login();
                loginWindow.Show();
                this.Close();
            }
        }

        private void HighlightButton(Button activeButton)
        {
            // Resetear todos los botones
            BtnDashboard.Background = System.Windows.Media.Brushes.Transparent;
            BtnProductos.Background = System.Windows.Media.Brushes.Transparent;
            BtnReportes.Background = System.Windows.Media.Brushes.Transparent;
            BtnConfiguracion.Background = System.Windows.Media.Brushes.Transparent;

            // Resaltar el botón activo
            activeButton.Background = new System.Windows.Media.SolidColorBrush(
          System.Windows.Media.Color.FromRgb(51, 65, 85)); // #334155
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
              {
                  TxtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
              };
            _timer.Start();

            // Actualizar inmediatamente
            TxtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _timer?.Stop();
        }
    }
}
