using System.Windows;
using System.Windows.Controls;

namespace TOO_SanBenito.UI
{
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            // TODO: Conectar con base de datos MySQL
            // Cargar ventas, productos, estadísticas, etc.

            TxtVentasHoy.Text = "$0.00";
            TxtVentasSemanales.Text = "$0.00";
            TxtTotalProductos.Text = "0";
            TxtStockBajo.Text = "0";
            TxtStockBajoDesc.Text = "Sin conexión a BD";
            TxtVentasCategoria.Text = "Conecta la base de datos para ver reportes";

            PanelActividad.Children.Clear();
        }
    }
}
