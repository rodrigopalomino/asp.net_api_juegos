using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;
using tr1.v2.Models;
using System.ComponentModel.DataAnnotations;

namespace tr1.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>(); // Inicializar el PasswordHasher
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
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

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Cifrar la contraseña del usuario antes de guardarla
            usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Validar el modelo de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, Message = "Datos de inicio de sesión inválidos." });
            }

            // Buscar el usuario por su nombre de usuario
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (usuario == null)
            {
                return Unauthorized(new { success = false, Message = "Usuario no encontrado." });
            }

            // Verificar la contraseña
            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { success = false, Message = "Contraseña incorrecta." });
            }

            // Crear la cookie de autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Iniciar sesión y crear la cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Redirigir a la página de éxito o devolver un mensaje
            return Ok(new { success = true, Message = "Inicio de sesión exitoso." });
        }


        // POST: api/Usuarios/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Cierre de sesión exitoso." });
        }


        // DTO para el inicio de sesión
        public class LoginDto
        {
            [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            public string Password { get; set; }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
