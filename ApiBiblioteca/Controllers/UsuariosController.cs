using Microsoft.AspNetCore.Mvc;


using ApiBiblioteca.DTOs;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public UsuariosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> getUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Cedula = u.Cedula,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Correo = u.Correo,
                    Rol = u.IdRolNavigation.NombreRol,
                    FechaRegistro = u.FechaRegistro
                })
                .FirstOrDefaultAsync();

            if (usuario == null) return NotFound();

            return Ok(usuario);
        }



        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")] // Solo usuarios con roles Admin pueden acceder
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }
            _context.Entry(usuario).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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


        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
