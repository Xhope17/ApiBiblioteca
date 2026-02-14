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
                .Select(u => new UsuarioDto // Aquí llenamos el DTO actualizado
                {
                    IdUsuario = u.IdUsuario,
                    Cedula = u.Cedula,
                    Nombre = u.Nombre,       // Ahora sí existe en el DTO
                    Apellido = u.Apellido,   // Ahora sí existe en el DTO
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Correo = u.Correo,
                    Rol = u.IdRolNavigation.NombreRol,
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
                // 1. Crear Usuario Base
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

                // 2. Si es Bibliotecario (Rol 1), guardar credenciales
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
    }
}