using System;
using System.Collections.Generic;

namespace ApiBiblioteca.Models;

public partial class Prestamo
{
    public int IdPrestamo { get; set; }

    public int IdUsuario { get; set; }

    public int IdLibro { get; set; }

    public int IdBibliotecario { get; set; }

    public DateTime? FechaPrestamo { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public string? Estado { get; set; }

    public virtual Bibliotecario IdBibliotecarioNavigation { get; set; } = null!;

    public virtual Libro IdLibroNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
