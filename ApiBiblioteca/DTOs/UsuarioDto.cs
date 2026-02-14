namespace ApiBiblioteca.DTOs
{
    // 1. Para LEER datos (GET) - Lo que envías a Angular
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Cedula { get; set; } = null!;

        // FALTABAN ESTOS DOS CAMPOS QUE CAUSABAN EL ERROR:
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;

        public string NombreCompleto { get; set; } = null!;
        public string? Correo { get; set; }
        public string Rol { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
    }

    // 2. Para CREAR datos (POST) - Lo que recibes del formulario
    public class UsuarioCreacionDto
    {
        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Correo { get; set; }
        public int IdRol { get; set; } // 1: Bibliotecario, 2: Cliente
        public string? Password { get; set; } // Solo para Bibliotecarios
    }
}