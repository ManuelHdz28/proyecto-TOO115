using System; // Necesario para DateTime

namespace TOO_SanBenito.Models
{
    public class Producto
    {
        public int Id { get; set; }
        // La tabla Productos no tiene IdFarmacia, se omite.
        public int? IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime? FechaCaducidad { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Propiedades de navegación
        public string? CategoriaNombre { get; set; }

        // Propiedad calculada para alertas de stock
        public bool StockBajo => Stock < 10;

        /// <summary>
        /// Determina el estado de la caducidad.
        /// Retorna: "Vencido", "PorVencer", o "Perfecto".
        /// </summary>
        public string EstadoCaducidad
        {
            get
            {
                if (!FechaCaducidad.HasValue)
                {
                    return "Sin Fecha"; // Para productos sin fecha registrada
                }

                DateTime hoy = DateTime.Today;
                DateTime fechaCad = FechaCaducidad.Value.Date;
                TimeSpan diferencia = fechaCad - hoy;

                if (diferencia.TotalDays < 0)
                {
                    return "Vencido";
                }
                else if (diferencia.TotalDays <= 30) // Por vencer en 30 días o menos
                {
                    return "PorVencer";
                }
                else
                {
                    return "Perfecto";
                }
            }
        }
    }
}
