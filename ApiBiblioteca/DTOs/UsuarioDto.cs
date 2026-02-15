namespace ApiBiblioteca.DTOs
{
    // Lo que se envia a Angular
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Correo { get; set; }

        public int IdRol { get; set; } // para Angular 

        public string Rol { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
    }

    public class UsuarioCreacionDto
    {
        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Correo { get; set; }
        public int IdRol { get; set; }
        public string? Password { get; set; }
    }
}