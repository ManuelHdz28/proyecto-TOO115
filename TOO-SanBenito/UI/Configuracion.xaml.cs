using System.Windows;
using System.Windows.Controls;

namespace TOO_SanBenito.UI
{
    public partial class Configuracion : Page
    {
        public Configuracion()
        {
            InitializeComponent();
        }

        private void BtnGuardarInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Información de la farmacia guardada exitosamente",
                "Éxito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnGuardarConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Configuración del sistema guardada exitosamente",
                "Éxito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
