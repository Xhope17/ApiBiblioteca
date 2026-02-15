namespace ApiBiblioteca.DTOs
{
    public class PrestamoDto
    {
        public int IdPrestamo { get; set; }
        public string Cliente { get; set; } = null!;
        public string Libro { get; set; } = null!;
        public string AtendidoPor { get; set; } = null!; // Nombre del Bibliotecario
        public DateTime? FechaPrestamo { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string? Estado { get; set; }
    }

    public class PrestamoCreacionDto
    {
        public int IdUsuario { get; set; }       // Cliente
        public int IdLibro { get; set; }         // Libro
        public int IdBibliotecario { get; set; } // Quién atiende
    }
}