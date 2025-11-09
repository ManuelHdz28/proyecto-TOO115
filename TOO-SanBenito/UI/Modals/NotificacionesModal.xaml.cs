using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOO_SanBenito.Data;

namespace TOO_SanBenito.UI.Modals
{
    public partial class NotificacionesModal : Window
    {
        private readonly NotificacionService _notificacionService;
        private List<NotificacionDisplay> _notificaciones;

        public NotificacionesModal(List<NotificacionDisplay> notificaciones, NotificacionService service)
        {
            InitializeComponent();
            _notificaciones = notificaciones;
            _notificacionService = service;
            CargarNotificaciones();
        }

        private void CargarNotificaciones()
        {
            PanelNotificaciones.Children.Clear();
            TxtTotalNotificaciones.Text = $"{_notificaciones.Count} Notificaciones Pendientes";

            if (!_notificaciones.Any())
            {
                PanelNotificaciones.Children.Add(new TextBlock
                {
                    Text = "No tienes alertas de stock ni caducidad pendientes.",
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10),
                    TextAlignment = TextAlignment.Center
                });
                return;
            }

            foreach (var noti in _notificaciones)
            {
                // Contenedor principal
                Border itemBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(0, 10, 0, 10)
                };

                Grid itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) }); // Icono
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Descripción
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) }); // Botón X

                // 1. Icono
                TextBlock icono = new TextBlock
                {
                    Text = noti.Icono,
                    Foreground = noti.Color,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(icono, 0);
                itemGrid.Children.Add(icono);

                // 2. Descripción y Fecha
                StackPanel descriptionPanel = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
                descriptionPanel.Children.Add(new TextBlock
                {
                    Text = noti.Descripcion,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold
                });
                descriptionPanel.Children.Add(new TextBlock
                {
                    Text = $"Hace {CalcularTiempoTranscurrido(noti.FechaHora)}",
                    FontSize = 10,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 3, 0, 0)
                });
                Grid.SetColumn(descriptionPanel, 1);
                itemGrid.Children.Add(descriptionPanel);

                // 3. Botón Marcar como Leída (X)
                Button btnLeida = new Button
                {
                    Content = "✕",
                    Tag = noti.Id,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Gray,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Width = 25,
                    Height = 25,
                    FontSize = 14
                };
                btnLeida.Click += BtnMarcarComoLeida_Click;
                Grid.SetColumn(btnLeida, 2);
                itemGrid.Children.Add(btnLeida);

                itemBorder.Child = itemGrid;
                PanelNotificaciones.Children.Add(itemBorder);
            }
        }

        private string CalcularTiempoTranscurrido(DateTime fecha)
        {
            TimeSpan tiempo = DateTime.Now - fecha;
            if (tiempo.TotalMinutes < 1) return "instantes";
            if (tiempo.TotalHours < 1) return $"{(int)tiempo.TotalMinutes} min";
            if (tiempo.TotalDays < 1) return $"{(int)tiempo.TotalHours} h";
            return $"{(int)tiempo.TotalDays} días";
        }

        private void BtnMarcarComoLeida_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int idNotificacion)
            {
                if (_notificacionService.MarcarComoLeida(idNotificacion))
                {
                    // Eliminar de la lista local y recargar la vista
                    _notificaciones.RemoveAll(n => n.Id == idNotificacion);
                    CargarNotificaciones();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la base de datos.", "Fallo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}