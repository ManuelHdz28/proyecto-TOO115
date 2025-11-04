using System.Windows;
using System.Windows.Controls;

namespace TOO_SanBenito.UI
{
    public partial class Reportes : Page
    {
        public Reportes()
        {
            InitializeComponent();
            Loaded += Reportes_Loaded;
        }

        private void Reportes_Loaded(object sender, RoutedEventArgs e)
        {
            CargarReporteVentasPorTipo();
            CargarReporteHorariosPico();
            CargarReporteStock();
        }

        private void CargarReporteVentasPorTipo()
        {
            // TODO: Cargar ventas por categoría desde BD
            TxtVentaLibre.Text = "0 unidades";
            TxtConReceta.Text = "0 unidades";
            TxtControlados.Text = "0 unidades";

            DgVentasPorTipo.ItemsSource = new List<ReporteVentasPorTipo>();
        }

        private void CargarReporteHorariosPico()
        {
            // TODO: Cargar ventas por hora desde BD
            TxtHoraPico.Text = "--:--";
            TxtVentasHoraPico.Text = "0 ventas";
            TxtHoraTranquila.Text = "--:--";
            TxtVentasHoraTranquila.Text = "0 ventas";

            CanvasGrafico.Children.Clear();
        }

        private void CargarReporteStock()
        {
            // TODO: Cargar productos y calcular stock desde BD
            TxtStockNormal.Text = "0 productos";
            TxtStockBajoCount.Text = "0 productos";
            TxtValorTotal.Text = "$0.00";

            DgStock.ItemsSource = new List<ReporteStock>();
        }
    }

    public class ReporteVentasPorTipo
    {
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string Porcentaje { get; set; } = string.Empty;
    }

    public class ReporteStock
    {
        public string Nombre { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
        public decimal ValorTotal { get; set; }
        public bool StockBajo { get; set; }
    }
}
