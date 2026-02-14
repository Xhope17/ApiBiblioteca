using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiBiblioteca.Controllers // <--- CAMBIADO AL NAMESPACE DE TU PROYECTO
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BibliotecaDbContext _context;

        public AuthController(IConfiguration configuration, BibliotecaDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            var bibliotecario = await _context.Bibliotecarios
                .Include(b => b.IdUsuarioNavigation)
                .ThenInclude(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(b =>
                    b.IdUsuarioNavigation.Correo == login.Username);

            if (bibliotecario == null)
                return Unauthorized("Usuario no encontrado");

            // Validación simple (luego puedes usar hash)
            if (bibliotecario.PasswordHash != login.Password)
                return Unauthorized("Contraseña incorrecta");

            var usuario = new UsuarioInfo
            {
                Username = bibliotecario.IdUsuarioNavigation.Nombre,
                Role = bibliotecario.IdUsuarioNavigation.IdRolNavigation.NombreRol,
                IdBibliotecario = bibliotecario.IdBibliotecario
            };

            var token = GenerateToken(usuario);

            return Ok(new
            {
                token,
                username = usuario.Username,
                role = usuario.Role,
                idBibliotecario = usuario.IdBibliotecario
            });
        }

        private string GenerateToken(UsuarioInfo usuario)
        {
            // Leemos la configuración del appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var keyString = jwtSettings.GetValue<string>("Key");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var minutes = jwtSettings.GetValue<int>("ExpireMinutes");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Role),
                // Guardamos el ID en el token para usarlo luego
                new Claim("IdBibliotecario", usuario.IdBibliotecario.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Clases internas para que no te den error
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private class UsuarioInfo
        {
            public string Username { get; set; }
            public string Role { get; set; }
            public int IdBibliotecario { get; set; }
        }
    }
}
