using System;
using System.Collections.Generic;

namespace ApiBiblioteca.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Correo { get; set; }

    public int IdRol { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Bibliotecario? Bibliotecario { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
