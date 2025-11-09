using System.Windows;
using System.Linq;
using TOO_SanBenito.Data;

namespace TOO_SanBenito.UI.Modals
{
    public partial class DetallesVentaModal : Window
    {
        private readonly DashboardVendedorService _dashboardService;
        private readonly int _ventaId;

        public DetallesVentaModal(int ventaId, decimal totalVenta)
        {
            InitializeComponent();
            _dashboardService = new DashboardVendedorService();
            _ventaId = ventaId;

            TxtTituloModal.Text = $"Detalle de Venta #{ventaId}";
            TxtTotalVenta.Text = totalVenta.ToString("C2");
            CargarDetalles();
        }

        private void CargarDetalles()
        {
            var detalles = _dashboardService.GetDetallesVenta(_ventaId);

            if (detalles != null && detalles.Any())
            {
                DgDetalles.ItemsSource = detalles;
            }
            else
            {
                // Manejo si no se encuentran detalles
                MessageBox.Show("No se encontraron detalles para esta venta.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}