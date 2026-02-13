using Microsoft.AspNetCore.Mvc;


using ApiBiblioteca.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBiblioteca.Controllers
{
    [Route("api/[controller]")] // <--- ¡ESTA LÍNEA ES LA QUE FALTA!
    [ApiController]
    public class BibliotecariosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public BibliotecariosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bibliotecario>>> GetGenres()
        {
            return await _context.Bibliotecarios.ToListAsync();
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bibliotecario>> GetBibliotecario(int id)
        {
            var bibliotecario = await _context.Bibliotecarios.FindAsync(id);

            if (bibliotecario == null)
            {
                return NotFound();
            }

            return bibliotecario;
        }

       
        

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdBibliotecario,IdUsuario,PasswordHash")] Bibliotecario bibliotecario)
        {
            if (id != bibliotecario.IdBibliotecario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bibliotecario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BibliotecarioExists(bibliotecario.IdBibliotecario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", bibliotecario.IdUsuario);
            return View(bibliotecario);
        }


        // POST: Bibliotecarios/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bibliotecario = await _context.Bibliotecarios.FindAsync(id);
            if (bibliotecario == null)
            {
                return NotFound();
            }
            _context.Bibliotecarios.Remove(bibliotecario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BibliotecarioExists(int id)
        {
            return _context.Bibliotecarios.Any(e => e.IdBibliotecario == id);
        }
    }
}
