using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TOO_SanBenito.Data;
using TOO_SanBenito.UI.Modals; // Necesario para la ventana modal
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes; // Necesario para Rectangle si lo usaras

namespace TOO_SanBenito
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private DispatcherTimer _notificationTimer; // Timer para notificaciones
        private NotificacionService _notificacionService;
        private string _userRole;

        public MainWindow()
        {
            InitializeComponent();
            _userRole = "Admin"; // Default
            _notificacionService = new NotificacionService();
            ConfigurarMenuPorRol();
            NavigateToDashboard();
            StartClock();
            StartNotificationChecker(); // Iniciar el verificador de notificaciones
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
                // BtnDashboard.Content = "MIS VENTAS"; // Revisa tu XAML para el contenido
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
                TxtPageTitle.Text = "MIS VENTAS";
            }
            else
            {
                MainFrame.Navigate(new UI.Page1());
                TxtPageTitle.Text = "DASHBOARD";
            }
            HighlightButton(BtnDashboard);
        }

        private void HighlightButton(Button activeButton)
        {
            // Resetear todos los botones
            BtnDashboard.Background = Brushes.Transparent;
            BtnProductos.Background = Brushes.Transparent;
            BtnVentas.Background = Brushes.Transparent;
            BtnReportes.Background = Brushes.Transparent;
            BtnConfiguracion.Background = Brushes.Transparent;

            // Resaltar el botón activo
            activeButton.Background = new SolidColorBrush(Color.FromRgb(51, 65, 85)); // #334155
        }

        // ------------------------------------------------------------------
        // LÓGICA DE NOTIFICACIONES (CAMPANA)
        // ------------------------------------------------------------------

        private void StartNotificationChecker()
        {
            // Chequea la base de datos cada 30 segundos
            _notificationTimer = new DispatcherTimer();
            _notificationTimer.Interval = TimeSpan.FromSeconds(30);
            _notificationTimer.Tick += (s, e) => { ActualizarContadorNotificaciones(); };
            _notificationTimer.Start();

            ActualizarContadorNotificaciones(); // Cargar al inicio
        }

        private void ActualizarContadorNotificaciones()
        {
            try
            {
                var notificaciones = _notificacionService.GetNotificacionesNoLeidas();
                int count = notificaciones.Count;

                TxtNotificacionesCount.Text = count.ToString();
                // Verifica que BadgeNotificaciones no sea nulo antes de acceder a Visibility
                if (BadgeNotificaciones != null)
                {
                    BadgeNotificaciones.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                if (BadgeNotificaciones != null)
                {
                    BadgeNotificaciones.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnNotificaciones_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtener notificaciones
            var notificaciones = _notificacionService.GetNotificacionesNoLeidas();

            // 2. Mostrar la ventana modal de notificaciones
            var modal = new UI.Modals.NotificacionesModal(notificaciones, _notificacionService);

            // 3. Abrir como ShowDialog para bloquear la ventana principal hasta que se cierre
            modal.ShowDialog();

            // 4. Refrescar el contador cuando se cierre la ventana modal
            ActualizarContadorNotificaciones();
        }


        // ------------------------------------------------------------------
        // EVENTOS DE NAVEGACIÓN Y RELOJ
        // ------------------------------------------------------------------

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Productos());
            TxtPageTitle.Text = "PRODUCTOS";
            HighlightButton(BtnProductos);
        }

        private void BtnVentas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Ventas());
            TxtPageTitle.Text = "VENTAS";
            HighlightButton(BtnVentas);
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Reportes());
            TxtPageTitle.Text = "REPORTES";
            HighlightButton(BtnReportes);
        }

        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UI.Configuracion());
            TxtPageTitle.Text = "CONFIGURACIÓN";
            HighlightButton(BtnConfiguracion);
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
            _notificationTimer?.Stop(); // Detener el temporizador de notificaciones
        }
    }
}