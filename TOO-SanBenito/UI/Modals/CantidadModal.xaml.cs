using System.Windows;
using System.Windows.Controls;

namespace TOO_SanBenito.UI.Modals
{
    public partial class CantidadModal : Window
    {
        public int CantidadSeleccionada { get; private set; }
        private int _maxStock;
        private bool _isUpdating = false; // <<< BANDERA DE CONTROL AÑADIDA

        public CantidadModal(string productoNombre, int maxStock)
        {
            InitializeComponent();
            TxtTitulo.Text = $"Cantidad para: {productoNombre}";
            _maxStock = maxStock;
            TxtStockMaximo.Text = $"Stock máximo: {maxStock} unidades";

            // Si el maxStock es 0 o menos, ya debería haberse bloqueado en Ventas.xaml.cs,
            // pero si llega aquí, lo bloqueamos y establecemos 0.
            if (_maxStock <= 0)
            {
                TxtCantidad.Text = "0";
                BtnAceptar.IsEnabled = false;
            }
            else
            {
                TxtCantidad.Text = "1"; // Valor inicial seguro
                TxtCantidad.Focus();
                TxtCantidad.SelectAll();
            }
        }

        private void TxtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return; // Salir si estamos corrigiendo el valor
            _isUpdating = true; // Activar el interruptor

            if (int.TryParse(TxtCantidad.Text, out int cantidad))
            {
                if (cantidad > _maxStock)
                {
                    MessageBox.Show($"La cantidad máxima permitida es {_maxStock}.", "Stock Límite", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Corregir el valor sin volver a llamar a TextChanged (gracias a _isUpdating)
                    TxtCantidad.Text = _maxStock.ToString();
                    TxtCantidad.SelectAll();
                }
                else if (cantidad <= 0)
                {
                    // Si el valor es cero o menos, forzar a 1 (si hay stock disponible)
                    int valorMinimo = _maxStock > 0 ? 1 : 0;
                    TxtCantidad.Text = valorMinimo.ToString();
                    TxtCantidad.SelectAll();
                }
            }
            else if (!string.IsNullOrEmpty(TxtCantidad.Text))
            {
                // Si no es un número, forzar a 1
                TxtCantidad.Text = "1";
                TxtCantidad.SelectAll();
            }

            _isUpdating = false; // Desactivar el interruptor
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TxtCantidad.Text, out int cantidad) && cantidad > 0)
            {
                CantidadSeleccionada = cantidad;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida mayor a 0.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}