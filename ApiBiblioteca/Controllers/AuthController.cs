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

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Simulación Hardcoded
            UsuarioInfo usuario = null;

            // CASO 1: Es el Admin/Bibliotecario
            // Asumimos que en tu DB el Bibliotecario tiene ID 1 (lo creamos en el script)
            if (login.Username == "admin" && login.Password == "1234")
            {
                usuario = new UsuarioInfo
                {
                    Username = "admin",
                    Role = "Bibliotecario",
                    IdBibliotecario = 1 // <--- IMPORTANTE: Esto permite crear préstamos
                };
            }
            // CASO 2: Es un usuario cualquiera (opcional)
            else if (login.Username == "user" && login.Password == "1234")
            {
                usuario = new UsuarioInfo
                {
                    Username = "user",
                    Role = "Cliente",
                    IdBibliotecario = 0
                };
            }

            if (usuario == null)
                return Unauthorized("Credenciales incorrectas");

            var token = GenerateToken(usuario);

            // Devolvemos el token y los datos útiles para el Frontend
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