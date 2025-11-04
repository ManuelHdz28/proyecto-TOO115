namespace TOO_SanBenito.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public int IdFarmacia { get; set; }
        public int? IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Propiedades de navegación
        public string? CategoriaNombre { get; set; }

        // Propiedad calculada para alertas de stock
        public bool StockBajo => Stock < 10;
    }
}
