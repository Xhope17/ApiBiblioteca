namespace ApiBiblioteca.DTOs
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!; // Útil para mostrar en tablas
        public string? Correo { get; set; }
        public string Rol { get; set; } = null!; // Devolveremos "Bibliotecario" o "Cliente", no el número
        public DateTime? FechaRegistro { get; set; }
    }
}