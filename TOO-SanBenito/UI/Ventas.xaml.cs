using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOO_SanBenito.Data;
using TOO_SanBenito.Models;
using TOO_SanBenito.UI.Modals;
using System.Windows.Input;

namespace TOO_SanBenito.UI
{
    public partial class Ventas : Page
    {
        private readonly VentaService _ventaService;
        private readonly Dictionary<int, ItemCarrito> _carrito;
        private List<Producto> _productosOriginal;
        private List<Producto> _productosFiltrados;
        private List<Categoria> _categoriasList;

        public Ventas()
        {
            InitializeComponent();
            _ventaService = new VentaService();
            _carrito = new Dictionary<int, ItemCarrito>();
            _productosOriginal = new List<Producto>();
            _productosFiltrados = new List<Producto>();
            _categoriasList = new List<Categoria>();

            LoadData();
            ActualizarCarrito();
        }

        private void LoadData()
        {
            ConfigurarFiltros();
            CargarProductos();
        }

        private void ConfigurarFiltros()
        {
            _categoriasList = _ventaService.GetCategorias();

            var categorias = new List<Categoria>(_categoriasList);
            categorias.Insert(0, new Categoria { Id = 0, Nombre = "Todas" });

            CmbCategoriaFiltro.ItemsSource = categorias;
            CmbCategoriaFiltro.DisplayMemberPath = "Nombre";
            CmbCategoriaFiltro.SelectedValuePath = "Id";
            CmbCategoriaFiltro.SelectedIndex = 0;
        }

        private void CargarProductos()
        {
            _productosOriginal = _ventaService.GetProductosDisponibles();
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var productosFiltradosTemp = _productosOriginal.AsEnumerable();

            // 1. Filtrar por Categoría
            if (CmbCategoriaFiltro.SelectedValue is int selectedCategoryId && selectedCategoryId != 0)
            {
                productosFiltradosTemp = productosFiltradosTemp.Where(p => p.IdCategoria == selectedCategoryId);
            }

            // 2. Filtrar por Búsqueda de Texto
            string searchText = TxtBuscarProducto.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                productosFiltradosTemp = productosFiltradosTemp.Where(p =>
                    p.Nombre.ToLower().Contains(searchText) ||
                    p.Descripcion.ToLower().Contains(searchText));
            }

            _productosFiltrados = productosFiltradosTemp.ToList();
            ListaProductos.ItemsSource = _productosFiltrados;
        }

        // ====================================================================
        // LÓGICA DE CARRITO: EVENTOS DINÁMICOS Y ACTUALIZACIÓN
        // ====================================================================

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                Producto producto = _productosOriginal.FirstOrDefault(p => p.Id == productoId);

                if (producto == null) return;

                int stockDisponible = producto.Stock;

                int cantidadEnCarrito = _carrito.ContainsKey(productoId) ? _carrito[productoId].Cantidad : 0;
                int maxStockPermitido = stockDisponible + cantidadEnCarrito;

                if (maxStockPermitido <= 0)
                {
                    MessageBox.Show($"El producto '{producto.Nombre}' no tiene stock disponible para la venta.", "Stock Agotado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var cantidadModal = new CantidadModal(producto.Nombre, maxStockPermitido);

                if (cantidadModal.ShowDialog() == true)
                {
                    int cantidad = cantidadModal.CantidadSeleccionada;

                    if (cantidad <= 0) return;

                    if (_carrito.ContainsKey(productoId))
                    {
                        _carrito[productoId].Cantidad = cantidad;
                    }
                    else
                    {
                        _carrito.Add(productoId, new ItemCarrito
                        {
                            Producto = producto,
                            Cantidad = cantidad
                        });
                    }

                    ActualizarCarrito();
                }
            }
        }

        private void BtnEliminarItemCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                if (_carrito.ContainsKey(productoId))
                {
                    _carrito.Remove(productoId);
                    ActualizarCarrito();
                }
            }
        }

        private void BtnCambiarCantidad_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productoId)
            {
                if (!_carrito.ContainsKey(productoId)) return;

                ItemCarrito item = _carrito[productoId];
                bool isAdding = (string)btn.Content == "+";

                int nuevaCantidad = item.Cantidad + (isAdding ? 1 : -1);

                int maxStockPermitido = item.Producto.Stock + item.Cantidad;

                if (nuevaCantidad > maxStockPermitido)
                {
                    MessageBox.Show($"Solo hay {item.Producto.Stock} unidades disponibles en stock para añadir.", "Stock Agotado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (nuevaCantidad <= 0)
                {
                    var resultado = MessageBox.Show($"¿Desea eliminar '{item.Producto.Nombre}' del carrito?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultado == MessageBoxResult.Yes)
                    {
                        _carrito.Remove(productoId);
                    }
                }
                else
                {
                    item.Cantidad = nuevaCantidad;
                }

                ActualizarCarrito();
            }
        }

        private void ActualizarCarrito()
        {
            PanelCarrito.Children.Clear();

            // Cargar recursos
            Brush primaryBrush = (Brush)this.FindResource("PrimaryBrush");
            Brush dangerBrush = (Brush)this.FindResource("DangerBrush");
            Brush secondaryBrush = (Brush)this.FindResource("SecondaryBrush");
            Style qtyButtonStyle = (Style)this.FindResource("CarritoQtyButtonStyle");

            if (qtyButtonStyle == null || primaryBrush == null)
            {
                // Si falla, el error de MissingResource lo reporta.
                return;
            }

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

            decimal subtotal = _carrito.Values.Sum(item => item.Subtotal);
            int totalItems = _carrito.Values.Sum(item => item.Cantidad);

            // Generar la UI de los ítems del carrito dinámicamente
            foreach (var item in _carrito.Values)
            {

                Grid itemGrid = new Grid { Margin = new Thickness(0, 0, 0, 10) };
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) }); // Cantidad
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) }); // Eliminar

                Border itemBorder = new Border { BorderBrush = secondaryBrush, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(0, 10, 0, 10) };
                itemBorder.Child = itemGrid;

                // Columna 0: Detalles del Producto
                StackPanel detailsPanel = new StackPanel();
                detailsPanel.Children.Add(new TextBlock { Text = item.Producto.Nombre, FontWeight = FontWeights.SemiBold });
                detailsPanel.Children.Add(new TextBlock { Text = $"{item.Producto.Precio:C2} / unidad", FontSize = 11, Foreground = Brushes.Gray });

                StackPanel totalPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
                totalPanel.Children.Add(new TextBlock { Text = "Total: ", FontWeight = FontWeights.Bold });
                totalPanel.Children.Add(new TextBlock { Text = $"{item.Subtotal:C2}", FontWeight = FontWeights.Bold, Foreground = primaryBrush });
                detailsPanel.Children.Add(totalPanel);

                Grid.SetColumn(detailsPanel, 0);
                itemGrid.Children.Add(detailsPanel);

                // Columna 1: Control de Cantidad (+/-)
                StackPanel qtyPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

                Button btnRestar = new Button { Content = "-", Style = qtyButtonStyle, Tag = item.Producto.Id };
                btnRestar.Click += BtnCambiarCantidad_Click;

                TextBox txtQty = new TextBox { Text = item.Cantidad.ToString(), Width = 30, Height = 20, TextAlignment = TextAlignment.Center, IsReadOnly = true, Margin = new Thickness(0) };

                Button btnSumar = new Button { Content = "+", Style = qtyButtonStyle, Tag = item.Producto.Id };
                btnSumar.Click += BtnCambiarCantidad_Click;

                qtyPanel.Children.Add(btnRestar);
                qtyPanel.Children.Add(txtQty);
                qtyPanel.Children.Add(btnSumar);

                Grid.SetColumn(qtyPanel, 1);
                itemGrid.Children.Add(qtyPanel);

                // Columna 2: Botón Eliminar (X)
                Button btnEliminar = new Button
                {
                    Content = "X",
                    Background = Brushes.Transparent,
                    Foreground = dangerBrush,
                    BorderThickness = new Thickness(0),
                    Tag = item.Producto.Id,
                    VerticalAlignment = VerticalAlignment.Center,
                    Cursor = Cursors.Hand
                };
                btnEliminar.Click += BtnEliminarItemCarrito_Click;

                Grid.SetColumn(btnEliminar, 2);
                itemGrid.Children.Add(btnEliminar);

                PanelCarrito.Children.Add(itemBorder);
            }

            TxtItemsCarrito.Text = $"{totalItems} items";
            TxtSubtotal.Text = $"${subtotal:N2}";
            TxtTotal.Text = $"${subtotal:N2}";
        }

        // ====================================================================
        // MANEJO DE EVENTOS DE LA PÁGINA (GUARDAR VENTA)
        // ====================================================================

        private void BtnCompletarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (_carrito.Count == 0)
                return;

            decimal totalVenta = _carrito.Values.Sum(item => item.Subtotal);

            var resultado = MessageBox.Show(
                $"¿Confirmar venta por {totalVenta:C2}?",
                "Confirmar Venta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // LLAMADA AL SERVICIO DE PERSISTENCIA
                bool exito = _ventaService.RegistrarVenta(totalVenta, _carrito.Values);

                if (exito)
                {
                    MessageBox.Show("Venta registrada exitosamente. Inventario actualizado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarCarrito();
                    CargarProductos(); // Recargar productos para reflejar el nuevo stock
                }
                // Si no fue éxito, el servicio ya mostró un MessageBox con el error de la transacción.
            }
        }

        private void BtnLimpiarCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (_carrito.Count == 0)
                return;

            var resultado = MessageBox.Show(
                "¿Está seguro de limpiar el carrito? Los productos no se guardarán.",
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