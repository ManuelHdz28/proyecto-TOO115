using System.Windows;
using System.Windows.Controls;
using TOO_SanBenito.Models;

namespace TOO_SanBenito.UI
{
    public partial class Productos : Page
    {
        private List<Producto> _productosFiltrados;
        private int? _productoEditandoId;

        public Productos()
        {
            InitializeComponent();
            _productosFiltrados = new List<Producto>();
            ConfigurarFiltros();
            CargarProductos();
        }

        private void ConfigurarFiltros()
        {
            // TODO: Cargar categorías desde BD
            var categorias = new List<Categoria>
            {
                new() { Id = 0, Nombre = "Todas las Categorías" }
            };

            CmbFiltroCategoria.ItemsSource = categorias;
            CmbFiltroCategoria.DisplayMemberPath = "Nombre";
            CmbFiltroCategoria.SelectedValuePath = "Id";
            CmbFiltroCategoria.SelectedIndex = 0;

            CmbCategoria.ItemsSource = new List<Categoria>();
        }

        private void CargarProductos()
        {
            // TODO: Cargar productos desde BD
            _productosFiltrados = new List<Producto>();
            DgProductos.ItemsSource = _productosFiltrados;
            ActualizarEstadisticas();
        }

        private void AplicarFiltros()
        {
            // TODO: Aplicar filtros con datos de BD
            DgProductos.ItemsSource = _productosFiltrados;
        }

        private void ActualizarEstadisticas()
        {
            TxtTotalProductos.Text = "0";
            TxtStockBajo.Text = "0";
            TxtValorInventario.Text = "$0.00";
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            _productoEditandoId = null;
            TxtTituloFormulario.Text = "Nuevo Producto";
            LimpiarFormulario();
            MostrarFormulario();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                // TODO: Cargar producto desde BD
                MessageBox.Show("Conecta la BD para editar productos", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro de eliminar este producto?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    // TODO: Eliminar de BD
                    MessageBox.Show("Conecta la BD para eliminar productos", "Información",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            // TODO: Guardar en BD
            MessageBox.Show("Conecta la BD para guardar productos", "Información",
                MessageBoxButton.OK, MessageBoxImage.Information);

            OcultarFormulario();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OcultarFormulario();
        }

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarProductos();
        }

        private void CmbFiltroCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProductos != null)
                AplicarFiltros();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CmbCategoria.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar una categoría", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número mayor a 0", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("El stock debe ser un número mayor o igual a 0", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            TxtNombre.Text = "";
            TxtDescripcion.Text = "";
            CmbCategoria.SelectedIndex = -1;
            TxtPrecio.Text = "";
            TxtStock.Text = "";
        }

        private void MostrarFormulario()
        {
            PanelFormulario.Visibility = Visibility.Visible;
        }

        private void OcultarFormulario()
        {
            PanelFormulario.Visibility = Visibility.Collapsed;
            LimpiarFormulario();
        }
    }
}
