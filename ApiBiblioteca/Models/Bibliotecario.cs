using System;
using System.Collections.Generic;

namespace ApiBiblioteca.Models;

public partial class Bibliotecario
{
    public int IdBibliotecario { get; set; }

    public int IdUsuario { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
