using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOO_SanBenito.Data;
using System.Linq;
using System.Windows.Shapes;

namespace TOO_SanBenito.UI
{
    // La clase se llama Page1 según tu archivo dashboard.xaml.cs
    public partial class Page1 : Page
    {
        private readonly DashboardService _dashboardService;

        public Page1()
        {
            InitializeComponent();
            _dashboardService = new DashboardService();
            Loaded += Dashboard_Loaded;
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }


        private void CargarDatos()
        {
            try
            {
                // Buscamos los recursos en el contexto de la Page (SOLUCIÓN AL ERROR)
                Brush successBrush = (Brush)this.FindResource("SuccessBrush");
                Brush dangerBrush = (Brush)this.FindResource("DangerBrush");
                Brush warningBrush = (Brush)this.FindResource("WarningBrush");
                Brush primaryBrush = (Brush)this.FindResource("PrimaryBrush");

                // Fallo de carga (Si el XAML está bien, esto no debería ejecutarse, pero es una protección)
                if (successBrush == null || dangerBrush == null)
                {
                    throw new Exception("Error al encontrar recursos de colores (SuccessBrush o DangerBrush). Verifique su dashboard.xaml.");
                }

                // -----------------------------------------------------------
                // 1. Métricas Principales (Tarjetas)
                // -----------------------------------------------------------

                decimal ventasHoy = _dashboardService.GetVentasHoy();
                TxtVentasHoy.Text = ventasHoy.ToString("C2");

                decimal ventasSemanales = _dashboardService.GetVentasSemanales();
                TxtVentasSemanales.Text = ventasSemanales.ToString("C2");

                int totalProductos = _dashboardService.GetTotalProductos();
                TxtTotalProductos.Text = totalProductos.ToString("N0");

                int stockBajoCount = _dashboardService.GetStockBajoCount();
                TxtStockBajo.Text = stockBajoCount.ToString("N0");

                // Actualizar mensaje de Stock Bajo
                if (stockBajoCount > 0)
                {
                    TxtStockBajoDesc.Text = $"¡{stockBajoCount} productos en alerta!";
                    TxtStockBajoDesc.Foreground = dangerBrush;
                }
                else
                {
                    TxtStockBajoDesc.Text = "Todo en orden. No hay alertas.";
                    TxtStockBajoDesc.Foreground = successBrush;
                }

                // -----------------------------------------------------------
                // 2. Contenido Adicional
                // -----------------------------------------------------------

                // Ventas por Categoría (Resumen)
                string resumenVentas = _dashboardService.GetVentasPorCategoriaSummary();
                TxtVentasCategoria.Text = resumenVentas;

                // -----------------------------------------------------------
                // 3. Actividad Reciente (Lista dinámica: Ventas y Alertas)
                // -----------------------------------------------------------

                PanelActividad.Children.Clear();
                var actividadReciente = _dashboardService.GetActividadReciente();

                Brush separatorBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235));

                if (!actividadReciente.Any())
                {
                    PanelActividad.Children.Add(new TextBlock
                    {
                        Text = "No hay actividad reciente registrada hoy.",
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 15, 0, 15)
                    });
                }
                else
                {
                    foreach (var item in actividadReciente)
                    {
                        // Crear un Grid para cada elemento de actividad
                        Grid actividadGrid = new Grid
                        {
                            Margin = new Thickness(0, 5, 0, 5),
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Auto },
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = GridLength.Auto }
                            }
                        };

                        // 1. Icono
                        TextBlock icono = new TextBlock
                        {
                            Text = item.Icono,
                            Foreground = item.Color, // Usa el Brush que viene del servicio
                            FontWeight = FontWeights.Bold,
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 10, 0)
                        };
                        Grid.SetColumn(icono, 0);
                        actividadGrid.Children.Add(icono);

                        // 2. Descripción
                        TextBlock descripcion = new TextBlock
                        {
                            Text = item.Descripcion,
                            Foreground = Brushes.Black,
                            FontSize = 13,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        Grid.SetColumn(descripcion, 1);
                        actividadGrid.Children.Add(descripcion);

                        // 3. Hora
                        TextBlock hora = new TextBlock
                        {
                            Text = item.FechaHora.ToString("HH:mm"),
                            Foreground = Brushes.Gray,
                            FontSize = 12,
                            HorizontalAlignment = HorizontalAlignment.Right
                        };
                        Grid.SetColumn(hora, 2);
                        actividadGrid.Children.Add(hora);

                        PanelActividad.Children.Add(actividadGrid);

                        // Separador (línea horizontal)
                        Rectangle separador = new Rectangle
                        {
                            Height = 1,
                            Fill = separatorBrush,
                            Margin = new Thickness(0, 5, 0, 5)
                        };
                        PanelActividad.Children.Add(separador);
                    }

                    // Eliminar el último separador
                    if (PanelActividad.Children.Count > 0)
                    {
                        PanelActividad.Children.RemoveAt(PanelActividad.Children.Count - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo de conexión a BD o de la lógica
                MessageBox.Show($"Error fatal al cargar el Dashboard: {ex.Message}", "Error de Sistema", MessageBoxButton.OK, MessageBoxImage.Error);

                // Mostrar mensajes de error en la UI
                TxtVentasHoy.Text = "$---";
                TxtVentasSemanales.Text = "$---";
                TxtTotalProductos.Text = "---";
                TxtStockBajo.Text = "---";
                TxtStockBajoDesc.Text = "Error de conexión a la BD";
                TxtStockBajoDesc.Foreground = Brushes.Red;
                TxtVentasCategoria.Text = $"Error: {ex.Message}";
                PanelActividad.Children.Clear();
            }
        }
    }
}
