namespace TOO_SanBenito.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int IdFarmacia { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // Navegación
        public List<DetalleVenta> Detalles { get; set; } = new();
        public string? UsuarioNombre { get; set; }
    }

    public class DetalleVenta
    {
        public int Id { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Navegación
        public string? ProductoNombre { get; set; }

        // Calculado
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
