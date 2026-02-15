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
        public async Task<ActionResult<IEnumerable<object>>> GetPrestamos()
        {
            // Verificamos que no sea nulo
            if (_context.Prestamos == null)
            {
                return NotFound();
            }

            var prestamos = await _context.Prestamos
                .Include(p => p.IdLibroNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Select(p => new
                {
                    idPrestamo = p.IdPrestamo,
                    fechaPrestamo = p.FechaPrestamo,
                    fechaDevolucion = p.FechaDevolucion,
                    idLibro = p.IdLibro,
                    idUsuario = p.IdUsuario,

                    // Objeto que espera el frontend para mostrar el libro
                    libro = new
                    {
                        titulo = p.IdLibroNavigation.Titulo,
                        autor = p.IdLibroNavigation.Autor,
                        imagenUrl = p.IdLibroNavigation.ImagenUrl
                    },

                    // información del usuario
                    usuario = new
                    {
                        nombre = p.IdUsuarioNavigation.Nombre,
                        apellido = p.IdUsuarioNavigation.Apellido,
                        cedula = p.IdUsuarioNavigation.Cedula
                    }
                })
                .OrderByDescending(p => p.fechaPrestamo)
                .ToListAsync();

            return Ok(prestamos);
        }

        // POST: api/Prestamos (RESTA STOCK)
        [HttpPost]
        public async Task<ActionResult> PostPrestamo(PrestamoCreacionDto dto)
        {
            var libro = await _context.Libros.FindAsync(dto.IdLibro);

            if (libro == null) return NotFound("Libro no existe.");
            if (libro.Stock <= 0) return BadRequest("No hay stock disponible.");

            // Restar Stock
            libro.Stock -= 1;

            var prestamo = new Prestamo
            {
                IdUsuario = dto.IdUsuario,
                IdLibro = dto.IdLibro,
                IdBibliotecario = dto.IdBibliotecario,
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

            // Sumar Stock
            var libro = await _context.Libros.FindAsync(prestamo.IdLibro);
            if (libro != null) libro.Stock += 1;

            prestamo.Estado = "Devuelto";
            prestamo.FechaDevolucion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Devolución exitosa" });
        }
    }
}