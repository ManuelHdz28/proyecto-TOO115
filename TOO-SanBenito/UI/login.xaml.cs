using System.Windows;

namespace TOO_SanBenito.UI
{
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
        }

        private async void BtnLoginAdmin_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            await Task.Delay(800); // Simular carga

            var mainWindow = new MainWindow("Administrador", "Admin");
            mainWindow.Show();
            this.Close();
        }

        private async void BtnLoginVendedor_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            await Task.Delay(800); // Simular carga

            var mainWindow = new MainWindow("Vendedor", "Vendedor");
            mainWindow.Show();
            this.Close();
        }

        private void ShowLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            BtnLoginAdmin.IsEnabled = !isLoading;
            BtnLoginVendedor.IsEnabled = !isLoading;
        }
    }
}
