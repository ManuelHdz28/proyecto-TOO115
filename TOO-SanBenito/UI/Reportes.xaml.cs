using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TOO_SanBenito.Data;
using TOO_SanBenito.Models; // Necesario para usar Producto

namespace TOO_SanBenito.UI
{
    public partial class Reportes : Page
    {
        private readonly ReporteService _reporteService;

        public Reportes()
        {
            InitializeComponent();
            _reporteService = new ReporteService();
            Loaded += Reportes_Loaded;
        }

        private void Reportes_Loaded(object sender, RoutedEventArgs e)
        {
            CargarReporteVentasPorTipo();
            CargarReporteHorariosPico();
            CargarReporteStock();
        }

        // -------------------------------------------------------------------
        // LÓGICA DE VENTAS POR TIPO (Anterior)
        // -------------------------------------------------------------------

        private void CargarReporteVentasPorTipo()
        {
            var datosVenta = _reporteService.GetVentasPorCategoria();
            int totalGlobalUnidades = datosVenta.Sum(d => d.UnidadesVendidas);

            var resultadosFinales = new List<ReporteVentasPorTipo>();

            int ventaLibreUnidades = 0;
            int conRecetaUnidades = 0;
            int controladosUnidades = 0;

            foreach (var item in datosVenta)
            {
                switch (item.NombreCategoria)
                {
                    case "Medicamentos - Venta Libre (OTC)":
                        ventaLibreUnidades = item.UnidadesVendidas;
                        break;
                    case "Medicamentos - Con Receta":
                        conRecetaUnidades = item.UnidadesVendidas;
                        break;
                    case "Medicamentos - Controlados":
                        controladosUnidades = item.UnidadesVendidas;
                        break;
                }

                double porcentaje = totalGlobalUnidades > 0
                                    ? ((double)item.UnidadesVendidas / totalGlobalUnidades) * 100
                                    : 0;

                resultadosFinales.Add(new ReporteVentasPorTipo
                {
                    Categoria = item.NombreCategoria,
                    Cantidad = item.UnidadesVendidas,
                    Porcentaje = $"{porcentaje:N1}%"
                });
            }

            TxtVentaLibre.Text = $"{ventaLibreUnidades:N0} unidades";
            TxtConReceta.Text = $"{conRecetaUnidades:N0} unidades";
            TxtControlados.Text = $"{controladosUnidades:N0} unidades";

            DgVentasPorTipo.ItemsSource = resultadosFinales;
        }

        // -------------------------------------------------------------------
        // LÓGICA DE HORARIOS PICO (Anterior)
        // -------------------------------------------------------------------

        private void CargarReporteHorariosPico()
        {
            var ventasPorHora = _reporteService.GetVentasPorHora();

            var horaPico = ventasPorHora.OrderByDescending(v => v.CantidadVentas).FirstOrDefault();
            var horaTranquila = ventasPorHora.OrderBy(v => v.CantidadVentas).FirstOrDefault();

            if (horaPico != null)
            {
                TxtHoraPico.Text = $"{horaPico.Hora:00}:00";
                TxtVentasHoraPico.Text = $"{horaPico.CantidadVentas} ventas";
            }
            else
            {
                TxtHoraPico.Text = "--:--";
                TxtVentasHoraPico.Text = "0 ventas";
            }

            if (horaTranquila != null)
            {
                TxtHoraTranquila.Text = $"{horaTranquila.Hora:00}:00";
                TxtVentasHoraTranquila.Text = $"{horaTranquila.CantidadVentas} ventas";
            }
            else
            {
                TxtHoraTranquila.Text = "--:--";
                TxtVentasHoraTranquila.Text = "0 ventas";
            }

            DibujarGraficoBarras(ventasPorHora);
        }

        private void DibujarGraficoBarras(List<ReporteVentaHora> datos)
        {
            CanvasGrafico.Children.Clear();
            if (datos == null || !datos.Any()) return;

            // Constantes de diseño
            const double MARGEN = 30;
            double canvasWidth = CanvasGrafico.Width > 0 ? CanvasGrafico.Width : 600;
            double canvasHeight = CanvasGrafico.Height > 0 ? CanvasGrafico.Height : 300;
            double drawableWidth = canvasWidth - 2 * MARGEN;
            double drawableHeight = canvasHeight - 2 * MARGEN;

            int maxVentas = datos.Max(d => d.CantidadVentas);
            if (maxVentas == 0) maxVentas = 1;

            var datos24Horas = Enumerable.Range(0, 24)
                .GroupJoin(datos, h => h, d => d.Hora, (h, d) => new ReporteVentaHora { Hora = h, CantidadVentas = d.FirstOrDefault()?.CantidadVentas ?? 0 })
                .OrderBy(h => h.Hora)
                .ToList();

            int totalHoras = 24;
            double barWidth = (drawableWidth / totalHoras) - 4;

            for (int i = 0; i < totalHoras; i++)
            {
                var horaData = datos24Horas[i];
                double barHeight = (horaData.CantidadVentas / (double)maxVentas) * drawableHeight;
                double xPos = MARGEN + (i * (barWidth + 4));
                double yPos = canvasHeight - MARGEN - barHeight;

                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(37, 99, 235)); // PrimaryBrush
                if (horaData.CantidadVentas == maxVentas)
                    brush = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // SuccessBrush

                Rectangle bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = brush,
                    RadiusX = 2,
                    RadiusY = 2,
                    Tag = $"{horaData.Hora:00}:00 - {horaData.CantidadVentas} ventas"
                };

                Canvas.SetLeft(bar, xPos);
                Canvas.SetTop(bar, yPos);
                CanvasGrafico.Children.Add(bar);

                TextBlock horaLabel = new TextBlock
                {
                    Text = $"{horaData.Hora:00}",
                    FontSize = 9,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.SemiBold
                };
                Canvas.SetLeft(horaLabel, xPos + barWidth / 2 - 10);
                Canvas.SetTop(horaLabel, canvasHeight - MARGEN + 5);
                CanvasGrafico.Children.Add(horaLabel);
            }

            TextBlock yLabel = new TextBlock
            {
                Text = $"Ventas ({maxVentas})",
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Gray
            };
            Canvas.SetLeft(yLabel, 5);
            Canvas.SetTop(yLabel, 5);
            CanvasGrafico.Children.Add(yLabel);

            Line xAxis = new Line
            {
                X1 = MARGEN,
                Y1 = canvasHeight - MARGEN,
                X2 = canvasWidth - MARGEN,
                Y2 = canvasHeight - MARGEN,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };
            CanvasGrafico.Children.Add(xAxis);
        }

        // -------------------------------------------------------------------
        // NUEVA LÓGICA: REPORTE DE STOCK
        // -------------------------------------------------------------------

        private void CargarReporteStock()
        {
            // 1. Obtener todos los datos de stock
            var stockData = _reporteService.GetReporteStock();

            // 2. Calcular las métricas de resumen
            int totalProductos = stockData.Count;
            int stockBajoCount = stockData.Count(p => p.StockBajo);
            int stockNormalCount = totalProductos - stockBajoCount;
            decimal valorInventario = stockData.Sum(p => p.ValorTotal);

            // 3. Actualizar tarjetas
            TxtStockNormal.Text = $"{stockNormalCount:N0} productos";
            TxtStockBajoCount.Text = $"{stockBajoCount:N0} productos";
            TxtValorTotal.Text = valorInventario.ToString("C2");

            // 4. Actualizar DataGrid
            DgStock.ItemsSource = stockData;
        }
    }

    // Modelos de datos compartidos con ReporteService.cs
    public class ReporteVentasPorTipo
    {
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string Porcentaje { get; set; } = string.Empty;
    }

    // El modelo ReporteStock se usa tanto en el servicio como aquí para el DataGrid
    public class ReporteStock
    {
        public string Nombre { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
        public decimal ValorTotal { get; set; } // Stock * Precio
        public bool StockBajo { get; set; } // Stock < 10
    }
}