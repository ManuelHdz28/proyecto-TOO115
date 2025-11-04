using System.Windows;
using System.Windows.Controls;

namespace TOO_SanBenito.UI
{
    public partial class DashboardVendedor : Page
    {
        public DashboardVendedor()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            // TODO: Obtener solo las ventas de hoy desde BD

            // Métricas vacías hasta conectar BD
            TxtVentasHoy.Text = "$0.00";
            TxtNumVentasHoy.Text = "0 transacciones";
            TxtProductosVendidos.Text = "0";
            TxtPromedioVenta.Text = "$0.00";
            TxtUltimaVenta.Text = "Última venta: --:--";

            // Saludo según hora
            int hora = DateTime.Now.Hour;
            string saludo = hora < 12 ? "¡Buenos días" : (hora < 19 ? "¡Buenas tardes" : "¡Buenas noches");
            TxtSaludo.Text = $"{saludo}, Vendedor!";

            // Fecha actual
            TxtFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM yyyy",
                new System.Globalization.CultureInfo("es-ES"));

            // Sin ventas hasta conectar BD
            DgVentasHoy.ItemsSource = null;
            TxtSinVentas.Visibility = Visibility.Visible;
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void BtnVerDetalles_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int ventaId)
            {
                // TODO: Cargar detalles desde BD
                MessageBox.Show("Conecta la BD para ver detalles de ventas", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
