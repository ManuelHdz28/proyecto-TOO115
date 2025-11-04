namespace TOO_SanBenito.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public int IdFarmacia { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
