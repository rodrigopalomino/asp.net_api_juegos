using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;
using tr1.v2.Models;

namespace tr1.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Comentarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _context.Comentarios.ToListAsync();
        }

        // GET: api/Comentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return comentario;
        }

        // PUT: api/Comentarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentario(int id, Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return BadRequest();
            }

            _context.Entry(comentario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(id))
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

        
        // POST: api/Comentarios
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comentario>> PostComentario(Comentario comentario)
        {
            // Validar que el Usuario exista
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == comentario.UsuarioId);
            if (!usuarioExiste)
            {
                return NotFound(new { Message = "Usuario no encontrado." });
            }

            // Validar que el Juego exista
            var juego = await _context.Juegos.FindAsync(comentario.JuegoId);
            if (juego == null)
            {
                return NotFound(new { Message = "Juego no encontrado." });
            }

            // Asignar la fecha actual si no está especificada
            if (comentario.Fecha == DateTime.MinValue)
            {
                comentario.Fecha = DateTime.UtcNow;
            }

            // Añadir el comentario al contexto
            _context.Comentarios.Add(comentario);

            // Incrementar el contador de CantComentarios en el juego
            juego.CantComentarios += 1;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolver la respuesta con el comentario creado
            return CreatedAtAction("GetComentario", new { id = comentario.Id }, comentario);
        }



        // DELETE: api/Comentarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
