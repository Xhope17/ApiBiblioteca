namespace ApiBiblioteca.DTOs
{
    public class LibroDto
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class LibroCreacionDto
    {
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
    }
}