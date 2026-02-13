using System;
using System.Collections.Generic;

namespace ApiBiblioteca.Models;

public partial class Libro
{
    public int IdLibro { get; set; }

    public string Titulo { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public int Stock { get; set; }

    public string? ImagenUrl { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
