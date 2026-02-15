using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiBiblioteca.Models;
using ApiBiblioteca.DTOs;

namespace ApiBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public UsuariosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Select(u => new UsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Cedula = u.Cedula,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Correo = u.Correo,
                    Rol = u.IdRolNavigation.NombreRol,
                    IdRol = u.IdRol,
                    FechaRegistro = u.FechaRegistro
                }).ToListAsync();

            return Ok(usuarios);
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult> PostUsuario(UsuarioCreacionDto usuarioDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Cedula == usuarioDto.Cedula))
                return BadRequest("La cédula ya existe.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = new Usuario
                {
                    Cedula = usuarioDto.Cedula,
                    Nombre = usuarioDto.Nombre,
                    Apellido = usuarioDto.Apellido,
                    Correo = usuarioDto.Correo,
                    IdRol = usuarioDto.IdRol,
                    FechaRegistro = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Si es Bibliotecario IdRol 1, guardar password
                if (usuarioDto.IdRol == 1)
                {
                    if (string.IsNullOrEmpty(usuarioDto.Password))
                        throw new Exception("El bibliotecario requiere contraseña.");

                    var biblio = new Bibliotecario
                    {
                        IdUsuario = usuario.IdUsuario,
                        PasswordHash = usuarioDto.Password
                    };
                    _context.Bibliotecarios.Add(biblio);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { mensaje = "Usuario registrado correctamente", id = usuario.IdUsuario });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Usuarios/5
        //crear usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] UsuarioCreacionDto usuarioDto)
        {
            // Buscamos el usuario en la BD
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Actualizamos SOLO los datos que vienen del formulario
            usuarioExistente.Cedula = usuarioDto.Cedula;
            usuarioExistente.Nombre = usuarioDto.Nombre;
            usuarioExistente.Apellido = usuarioDto.Apellido;
            usuarioExistente.Correo = usuarioDto.Correo;
            usuarioExistente.IdRol = usuarioDto.IdRol;

            // contraseña es Bibliotecario es opcional editar
            if (usuarioDto.IdRol == 1 && !string.IsNullOrEmpty(usuarioDto.Password))
            {
                var biblio = await _context.Bibliotecarios.FirstOrDefaultAsync(b => b.IdUsuario == id);
                if (biblio != null)
                {
                    biblio.PasswordHash = usuarioDto.Password; //para encriptar 
                }
                else
                {
                    // Si antes era Cliente y ahora es Bibliotecario
                    _context.Bibliotecarios.Add(new Bibliotecario { IdUsuario = id, PasswordHash = usuarioDto.Password });
                }
            }

            // Si cambia de Bibliotecario a Cliente, se borran los datos de bibliotecario
            if (usuarioDto.IdRol == 2)
            {
                var biblio = await _context.Bibliotecarios.FirstOrDefaultAsync(b => b.IdUsuario == id);
                if (biblio != null) _context.Bibliotecarios.Remove(biblio);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.IdUsuario == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }


        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            try
            {
                // Limpiar referencia de bibliotecario si existe antes de borrar usuario
                var biblio = await _context.Bibliotecarios.FirstOrDefaultAsync(b => b.IdUsuario == id);
                if (biblio != null) _context.Bibliotecarios.Remove(biblio);

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("No se puede eliminar: tiene préstamos asociados.");
            }

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}