using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TOO_SanBenito.Data;
using TOO_SanBenito.Models;

namespace TOO_SanBenito.UI
{
    public partial class Productos : Page
    {
        private readonly ProductoService _productoService;
        private readonly NotificacionService _notificacionService;
        private List<Producto> _productosOriginal;
        private List<Producto> _productosFiltrados;
        private List<Categoria> _categoriasList;
        private int? _productoEditandoId;

        public Productos()
        {
            InitializeComponent();
            _productoService = new ProductoService();
            _notificacionService = new NotificacionService();

            _productosOriginal = new List<Producto>();
            _productosFiltrados = new List<Producto>();

            LoadData();
        }

        private void LoadData()
        {
            ConfigurarFiltros();
            CargarProductos();
        }

        private void ConfigurarFiltros()
        {
            _categoriasList = _productoService.GetCategorias();

            // 1. Configurar ComboBox de Filtro (con opción "Todas")
            var filtroCategorias = new List<Categoria>(_categoriasList);
            filtroCategorias.Insert(0, new Categoria { Id = 0, Nombre = "Todas las Categorías" });

            CmbFiltroCategoria.ItemsSource = filtroCategorias;
            CmbFiltroCategoria.DisplayMemberPath = "Nombre";
            CmbFiltroCategoria.SelectedValuePath = "Id";
            CmbFiltroCategoria.SelectedIndex = 0;

            // 2. Configurar ComboBox del Formulario (solo categorías reales)
            CmbCategoria.ItemsSource = _categoriasList;
            CmbCategoria.DisplayMemberPath = "Nombre";
            CmbCategoria.SelectedValuePath = "Id";
        }

        private void CargarProductos()
        {
            _productosOriginal = _productoService.GetProductos();
            AplicarFiltros();
            ActualizarEstadisticas();
        }

        private void AplicarFiltros()
        {
            var productosFiltradosTemp = _productosOriginal.AsEnumerable();

            // 1. Filtrar por Categoría
            if (CmbFiltroCategoria.SelectedValue is int selectedCategoryId && selectedCategoryId != 0)
            {
                productosFiltradosTemp = productosFiltradosTemp.Where(p => p.IdCategoria == selectedCategoryId);
            }

            // 2. Filtrar por Búsqueda de Texto
            string searchText = TxtBuscar.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                productosFiltradosTemp = productosFiltradosTemp.Where(p =>
                    p.Nombre.ToLower().Contains(searchText) ||
                    p.Descripcion.ToLower().Contains(searchText));
            }

            _productosFiltrados = productosFiltradosTemp.ToList();
            DgProductos.ItemsSource = _productosFiltrados;
        }

        private void ActualizarEstadisticas()
        {
            TxtTotalProductos.Text = _productosOriginal.Count.ToString();
            TxtStockBajo.Text = _productosOriginal.Count(p => p.StockBajo).ToString();
            decimal valorInventario = _productosOriginal.Sum(p => p.Precio * p.Stock);
            TxtValorInventario.Text = valorInventario.ToString("C2");
        }

        // ====================================================================
        // LÓGICA DE NOTIFICACIONES Y ALERTAS ESPECIALES
        // ====================================================================

        private void RevisarYGenerarNotificaciones(Producto producto)
        {
            string descripcionNoti = string.Empty;
            string nombreProducto = producto.Nombre;

            // 1. Revisar Stock
            if (producto.Stock == 0)
            {
                descripcionNoti = $"⚠️ Alerta: Stock Cero para '{nombreProducto}'. Requiere reposición inmediata.";
                _notificacionService.AgregarNotificacion(descripcionNoti);
            }
            else if (producto.StockBajo) // Stock < 10
            {
                descripcionNoti = $"⚠️ Alerta: Stock Bajo para '{nombreProducto}'. Quedan {producto.Stock} unidades.";
                _notificacionService.AgregarNotificacion(descripcionNoti);
            }

            // 2. Revisar Caducidad
            string estadoCad = producto.EstadoCaducidad;
            if (estadoCad == "Vencido")
            {
                descripcionNoti = $"❌ CRÍTICO: Producto VENCIDO: '{nombreProducto}'. Retire del inventario.";
                _notificacionService.AgregarNotificacion(descripcionNoti);
            }
            else if (estadoCad == "PorVencer")
            {
                if (producto.FechaCaducidad.HasValue)
                {
                    TimeSpan diasRestantes = producto.FechaCaducidad.Value.Date - DateTime.Today;
                    descripcionNoti = $"⏳ Advertencia: El producto '{nombreProducto}' CADUCA en {diasRestantes.Days} días.";
                    _notificacionService.AgregarNotificacion(descripcionNoti);
                }
            }
        }

        // NUEVA FUNCIÓN: Muestra alerta especial al guardar
        private void MostrarAlertaMedicamentoEspecial(int idCategoria)
        {
            // Buscar la categoría por ID en la lista local
            var categoria = _categoriasList.FirstOrDefault(c => c.Id == idCategoria);

            if (categoria == null) return;

            string nombreCategoria = categoria.Nombre;

            if (nombreCategoria == "Medicamentos - Con Receta" || nombreCategoria == "Medicamentos - Controlados")
            {
                string mensaje = $"Se ha ingresado un medicamento de tipo ESPECIAL ({nombreCategoria}). Verifique los registros de entrada y las regulaciones aplicables.";
                MessageBox.Show(mensaje, "Alerta de Medicamento Regulado", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        // ====================================================================
        // MANEJO DE EVENTOS DE LA UI: CREAR/EDITAR/ELIMINAR
        // ====================================================================

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            _productoEditandoId = null;
            TxtTituloFormulario.Text = "Nuevo Producto";
            BtnGuardar.Content = "Guardar";
            LimpiarFormulario();
            MostrarFormulario();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                Producto producto = _productoService.GetProductoById(productoId);

                if (producto != null)
                {
                    _productoEditandoId = producto.Id;
                    TxtTituloFormulario.Text = $"Editar Producto (ID: {producto.Id})";
                    BtnGuardar.Content = "Actualizar";

                    TxtNombre.Text = producto.Nombre;
                    TxtDescripcion.Text = producto.Descripcion;
                    DpFechaCaducidad.SelectedDate = producto.FechaCaducidad;
                    TxtPrecio.Text = producto.Precio.ToString();
                    TxtStock.Text = producto.Stock.ToString();

                    CmbCategoria.SelectedValue = producto.IdCategoria;

                    MostrarFormulario();
                }
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                var productoAEliminar = _productosOriginal.FirstOrDefault(p => p.Id == productoId);

                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar el producto '{productoAEliminar?.Nombre}' (ID {productoId})? Esta acción es irreversible.",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    if (_productoService.EliminarProducto(productoId))
                    {
                        MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarProductos();
                    }
                }
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            int selectedCategoryId = (int)CmbCategoria.SelectedValue;

            Producto productoData = new Producto
            {
                Nombre = TxtNombre.Text,
                Descripcion = TxtDescripcion.Text,
                IdCategoria = selectedCategoryId,
                FechaCaducidad = DpFechaCaducidad.SelectedDate,
                Precio = decimal.Parse(TxtPrecio.Text),
                Stock = int.Parse(TxtStock.Text)
            };

            bool success = false;

            if (_productoEditandoId == null)
            {
                // Modo Agregar
                success = _productoService.AgregarProducto(productoData);

                if (success)
                {
                    MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Llama a la alerta solo al AGREGAR
                    MostrarAlertaMedicamentoEspecial(selectedCategoryId);
                }
            }
            else
            {
                // Modo Editar
                productoData.Id = _productoEditandoId.Value;
                success = _productoService.ActualizarProducto(productoData);

                if (success)
                {
                    MessageBox.Show("Producto actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            if (success)
            {
                // Notificaciones de Stock/Caducidad se revisan después de un agregado/actualización
                RevisarYGenerarNotificaciones(productoData);

                OcultarFormulario();
                CargarProductos();
            }
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
            if (DgProductos != null && _productosOriginal != null)
                AplicarFiltros();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DgProductos != null && _productosOriginal != null)
                AplicarFiltros();
        }

        // ====================================================================
        // MÉTODOS DE SOPORTE Y VALIDACIÓN
        // ====================================================================

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
                MessageBox.Show("Debe seleccionar una categoría válida", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!DpFechaCaducidad.SelectedDate.HasValue)
            {
                MessageBox.Show("Debe seleccionar una fecha de caducidad.", "Validación",
                   MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrecio.Text, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.CurrentCulture, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número mayor a 0", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("El stock debe ser un número entero mayor o igual a 0", "Validación",
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
            DpFechaCaducidad.SelectedDate = null;
            TxtPrecio.Text = "";
            TxtStock.Text = "";
        }

        private void MostrarFormulario()
        {
            PanelFormulario.Visibility = Visibility.Visible;
            PanelPrincipal.IsEnabled = false;
        }

        private void OcultarFormulario()
        {
            PanelFormulario.Visibility = Visibility.Collapsed;
            LimpiarFormulario();
            PanelPrincipal.IsEnabled = true;
        }
    }
}