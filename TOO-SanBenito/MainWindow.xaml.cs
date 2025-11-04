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
        private string _userRole;

        public MainWindow()
        {
            InitializeComponent();
            _userRole = "Admin"; // Default
            ConfigurarMenuPorRol();
            NavigateToDashboard();
            StartClock();
        }

        public MainWindow(string username, string role) : this()
        {
            _userRole = role;
            TxtUserName.Text = username;
            TxtUsuarioActual.Text = $"{role}: {username}";

            ConfigurarMenuPorRol();
            NavigateToDashboard();
        }

        private void ConfigurarMenuPorRol()
        {
            if (_userRole == "Vendedor")
            {
                // Vendedor solo puede ver: Dashboard de ventas y Ventas
                BtnProductos.Visibility = Visibility.Collapsed;
                BtnReportes.Visibility = Visibility.Collapsed;
                BtnConfiguracion.Visibility = Visibility.Collapsed;
            }
            else // Admin
            {
                // Admin puede ver todo
                BtnProductos.Visibility = Visibility.Visible;
                BtnReportes.Visibility = Visibility.Visible;
                BtnConfiguracion.Visibility = Visibility.Visible;
            }
        }

        private void NavigateToDashboard()
        {
            if (_userRole == "Vendedor")
            {
                MainFrame.Navigate(new UI.DashboardVendedor());
                TxtPageTitle.Text = "Mis Ventas";
            }
            else
            {
                MainFrame.Navigate(new UI.Page1());
                TxtPageTitle.Text = "Dashboard";
            }
            HighlightButton(BtnDashboard);
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Productos());
            TxtPageTitle.Text = "Productos";
            HighlightButton(BtnProductos);
        }

        private void BtnVentas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Ventas());
            TxtPageTitle.Text = "Ventas";
            HighlightButton(BtnVentas);
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Reportes());
            TxtPageTitle.Text = "Reportes";
            HighlightButton(BtnReportes);
        }

        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Configuracion());
            TxtPageTitle.Text = "Configuración";
            HighlightButton(BtnConfiguracion);
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
              "�Est�s seguro que deseas cerrar sesi�n?",
              "Cerrar Sesi�n",
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
            BtnVentas.Background = System.Windows.Media.Brushes.Transparent;
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
