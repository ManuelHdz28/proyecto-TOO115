using System.Windows;
using System.Windows.Controls;
using TOO_SanBenito.Models;

namespace TOO_SanBenito.UI
{
    public partial class Ventas : Page
    {
        private readonly Dictionary<int, ItemCarrito> _carrito;
        private List<Producto> _productosFiltrados;

        public Ventas()
        {
            InitializeComponent();
            _carrito = new Dictionary<int, ItemCarrito>();
            _productosFiltrados = new List<Producto>();

            ConfigurarFiltros();
            CargarProductos();
        }

        private void ConfigurarFiltros()
        {
            // TODO: Cargar categorías desde BD
            var categorias = new List<Categoria>
            {
                new() { Id = 0, Nombre = "Todas" }
            };

            CmbCategoriaFiltro.ItemsSource = categorias;
            CmbCategoriaFiltro.DisplayMemberPath = "Nombre";
            CmbCategoriaFiltro.SelectedValuePath = "Id";
            CmbCategoriaFiltro.SelectedIndex = 0;
        }

        private void CargarProductos()
        {
            // TODO: Cargar productos con stock disponible desde BD
            _productosFiltrados = new List<Producto>();
            ListaProductos.ItemsSource = _productosFiltrados;
        }

        private void AplicarFiltros()
        {
            // TODO: Aplicar filtros desde BD
            ListaProductos.ItemsSource = _productosFiltrados;
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Conecta la BD para agregar productos al carrito", "Información",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ActualizarCarrito()
        {
            PanelCarrito.Children.Clear();

            if (_carrito.Count == 0)
            {
                TxtCarritoVacio.Visibility = Visibility.Visible;
                TxtItemsCarrito.Text = "0 items";
                TxtSubtotal.Text = "$0.00";
                TxtTotal.Text = "$0.00";
                BtnCompletarVenta.IsEnabled = false;
                return;
            }

            TxtCarritoVacio.Visibility = Visibility.Collapsed;
            BtnCompletarVenta.IsEnabled = true;

            decimal total = _carrito.Values.Sum(item => item.Subtotal);
            int totalItems = _carrito.Values.Sum(item => item.Cantidad);

            TxtItemsCarrito.Text = $"{totalItems} items";
            TxtSubtotal.Text = $"${total:N2}";
            TxtTotal.Text = $"${total:N2}";
        }

        private void BtnCompletarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (_carrito.Count == 0)
                return;

            var resultado = MessageBox.Show(
                $"¿Confirmar venta por ${TxtTotal.Text}?",
                "Confirmar Venta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // TODO: Guardar venta en BD
                MessageBox.Show("Conecta la BD para completar ventas", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LimpiarCarrito();
            }
        }

        private void BtnLimpiarCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (_carrito.Count == 0)
                return;

            var resultado = MessageBox.Show(
                "¿Está seguro de limpiar el carrito?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
                LimpiarCarrito();
        }

        private void LimpiarCarrito()
        {
            _carrito.Clear();
            ActualizarCarrito();
        }

        private void TxtBuscarProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void CmbCategoriaFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaProductos != null)
                AplicarFiltros();
        }
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Subtotal => Producto.Precio * Cantidad;
    }
}
