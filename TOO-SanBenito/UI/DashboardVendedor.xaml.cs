using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOO_SanBenito.Data;
using System.Linq;
using TOO_SanBenito.UI.Modals; // Necesario para la ventana modal

namespace TOO_SanBenito.UI
{
    public partial class DashboardVendedor : Page
    {
        private readonly DashboardVendedorService _dashboardService;

        public DashboardVendedor()
        {
            InitializeComponent();
            _dashboardService = new DashboardVendedorService();
            Loaded += DashboardVendedor_Loaded;
        }

        private void DashboardVendedor_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var resumen = _dashboardService.GetVentasDelDia();

                // 1. Cargar el saludo y la fecha
                int hora = DateTime.Now.Hour;
                string saludo = hora < 12 ? "¡Buenos días" : (hora < 19 ? "¡Buenas tardes" : "¡Buenas noches");
                TxtSaludo.Text = $"{saludo}, Vendedor!";
                TxtFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM yyyy",
                    new System.Globalization.CultureInfo("es-ES"));

                // 2. Cargar métricas principales
                Brush successBrush = (Brush)this.FindResource("SuccessBrush");

                TxtVentasHoy.Text = resumen.TotalVenta.ToString("C2");
                TxtNumVentasHoy.Text = $"{resumen.NumTransacciones} transacciones";
                TxtProductosVendidos.Text = resumen.TotalProductosVendidos.ToString("N0");
                TxtPromedioVenta.Text = resumen.PromedioVenta.ToString("C2");
                TxtUltimaVenta.Text = $"Última venta: {resumen.UltimaVentaHora}";

                if (resumen.TotalVenta > 0)
                {
                    TxtVentasHoy.Foreground = successBrush;
                }

                // 3. Cargar historial de ventas (DataGrid)
                if (resumen.Ventas.Any())
                {
                    DgVentasHoy.ItemsSource = resumen.Ventas;
                    TxtSinVentas.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DgVentasHoy.ItemsSource = null;
                    TxtSinVentas.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fatal al cargar el Dashboard del Vendedor: {ex.Message}", "Error de Sistema", MessageBoxButton.OK, MessageBoxImage.Error);

                // Restablecer valores en caso de error de conexión
                TxtVentasHoy.Text = "$---";
                TxtNumVentasHoy.Text = "Error BD";
                TxtProductosVendidos.Text = "---";
                TxtPromedioVenta.Text = "$---";
                TxtUltimaVenta.Text = "Error de conexión";
                DgVentasHoy.ItemsSource = null;
                TxtSinVentas.Visibility = Visibility.Visible;
            }
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void BtnVerDetalles_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int ventaId)
            {
                // Buscar la venta en la lista local para obtener el total
                var venta = (DgVentasHoy.ItemsSource as System.Collections.Generic.List<VentaDia>)
                    ?.FirstOrDefault(v => v.Id == ventaId);

                if (venta != null)
                {
                    // Abrir la ventana modal con los detalles
                    var detallesModal = new DetallesVentaModal(ventaId, venta.Total);
                    detallesModal.ShowDialog();
                }
            }
        }
    }
}