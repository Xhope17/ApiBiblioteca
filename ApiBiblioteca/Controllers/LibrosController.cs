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
                .Select(l => new LibroDto // Proyección a DTO
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

        // Aquí podrías agregar PUT y DELETE siguiendo el mismo patrón...
    }
}