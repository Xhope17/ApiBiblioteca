using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiBiblioteca.Models;
using ApiBiblioteca.DTOs;

namespace ApiBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public LibrosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: api/Libros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
        {
            var libros = await _context.Libros
                .Select(l => new LibroDto 
                {
                    IdLibro = l.IdLibro,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Stock = l.Stock,
                    ImagenUrl = l.ImagenUrl
                }).ToListAsync();

            return Ok(libros);
        }

        // POST: api/Libros
        [HttpPost]
        public async Task<ActionResult> PostLibro(LibroCreacionDto libroDto)
        {
            var libro = new Libro
            {
                Titulo = libroDto.Titulo,
                Autor = libroDto.Autor,
                Stock = libroDto.Stock,
                ImagenUrl = libroDto.ImagenUrl,
                FechaIngreso = DateTime.Now
            };

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Libro creado", id = libro.IdLibro });
        }


        // editar un libro
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibro(int id, Libro libro)
        {
            if (id != libro.IdLibro)
            {
                return BadRequest("El ID del libro no coincide.");
            }

            _context.Entry(libro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Libros/5
        // eliminar un libro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            if (_context.Libros == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound("El libro no existe.");
            }

            try
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("No se puede eliminar este libro porque tiene préstamos asociados o historial.");
            }

            return NoContent();
        }

        // para verificar si un libro existe por ID
        private bool LibroExists(int id)
        {
            return (_context.Libros?.Any(e => e.IdLibro == id)).GetValueOrDefault();
        }
    }
}