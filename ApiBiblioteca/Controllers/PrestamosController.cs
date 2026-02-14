using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiBiblioteca.Models;
using ApiBiblioteca.DTOs;

namespace ApiBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public PrestamosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: api/Prestamos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.IdLibroNavigation)
                .Include(p => p.IdBibliotecarioNavigation)
                    .ThenInclude(b => b.IdUsuarioNavigation)
                .Select(p => new PrestamoDto
                {
                    IdPrestamo = p.IdPrestamo,
                    Cliente = $"{p.IdUsuarioNavigation.Nombre} {p.IdUsuarioNavigation.Apellido}",
                    Libro = p.IdLibroNavigation.Titulo,
                    AtendidoPor = p.IdBibliotecarioNavigation.IdUsuarioNavigation.Nombre,
                    FechaPrestamo = p.FechaPrestamo,
                    FechaDevolucion = p.FechaDevolucion,
                    Estado = p.Estado
                }).ToListAsync();

            return Ok(prestamos);
        }

        // POST: api/Prestamos (RESTA STOCK)
        [HttpPost]
        public async Task<ActionResult> PostPrestamo(PrestamoCreacionDto dto)
        {
            var libro = await _context.Libros.FindAsync(dto.IdLibro);

            if (libro == null) return NotFound("Libro no existe.");
            if (libro.Stock <= 0) return BadRequest("No hay stock disponible.");

            // LOGICA CRITICA: Restar Stock
            libro.Stock -= 1;

            var prestamo = new Prestamo
            {
                IdUsuario = dto.IdUsuario,
                IdLibro = dto.IdLibro,
                IdBibliotecario = dto.IdBibliotecario, // En el futuro vendrá del Token JWT
                FechaPrestamo = DateTime.Now,
                Estado = "Activo"
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Préstamo registrado", nuevoStock = libro.Stock });
        }

        // PUT: api/Prestamos/Devolver/5 (SUMA STOCK)
        [HttpPut("Devolver/{id}")]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();
            if (prestamo.Estado == "Devuelto") return BadRequest("Ya fue devuelto.");

            // LOGICA CRITICA: Sumar Stock
            var libro = await _context.Libros.FindAsync(prestamo.IdLibro);
            if (libro != null) libro.Stock += 1;

            prestamo.Estado = "Devuelto";
            prestamo.FechaDevolucion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Devolución exitosa" });
        }
    }
}